using System.Threading;
using System.Threading.Tasks;
using Game.Client.Config;
using Game.Client.Views;
using Game.Core;
using Game.Core.Common;
using Game.Core.Common.Fsm;
using Game.Core.Units;
using VContainer;
using UnityEngine;

namespace Game.Client.Units
{
    /// <summary>
    /// Юніт гравця на чистому C#. Рухається вперед по осі Z та переходить у Dead при втраті HP.
    /// </summary>
    public sealed class PlayerUnit : BaseUnit
    {
        /// <inheritdoc/>
        public override Vector3 Position => _transform.position;
        /// <inheritdoc/>
        public override Vector3 AimPoint => _transform.position;

        private readonly float                              _speed;
        private readonly Transform                          _transform;
        private readonly StateMachine<PlayerUnit, CarState> _fsm;

        /// <summary>
        /// Створює юніта гравця та запускає FSM.
        /// </summary>
        public PlayerUnit(PlayerConfig config, CarView view,
            TickSystemRegistry tickSystemRegistry, IObjectResolver container) : base(config, container)
        {
            _transform = view.transform;
            _speed     = config.MoveSpeed;

            _fsm = new StateMachine<PlayerUnit, CarState>(this)
                .Add(new DrivingState())
                .Add(new DeadState());
            _fsm.Start(CarState.Driving);

            tickSystemRegistry.Register(_fsm);
        }

        /// <summary>Стан руху вперед. Переходить у Dead при втраті HP.</summary>
        private sealed class DrivingState : IState<PlayerUnit, CarState>
        {
            public CarState Id => CarState.Driving;
            public Task Enter(PlayerUnit ctx, CancellationToken ct) => Task.CompletedTask;
            public Task Exit(PlayerUnit ctx, CancellationToken ct) => Task.CompletedTask;

            public bool Tick(PlayerUnit ctx, float dt, out CarState next)
            {
                if (!ctx.Stats.Get<HpStat>().IsAlive)
                {
                    next = CarState.Dead;
                    return true;
                }

                var pos = ctx._transform.position;
                pos.z += ctx._speed * dt;
                ctx._transform.position = pos;

                next = CarState.Driving;
                return false;
            }
        }

        /// <summary>Стан загибелі гравця. Нічого не робить.</summary>
        private sealed class DeadState : IState<PlayerUnit, CarState>
        {
            public CarState Id => CarState.Dead;
            public Task Enter(PlayerUnit ctx, CancellationToken ct) => Task.CompletedTask;
            public Task Exit(PlayerUnit ctx, CancellationToken ct) => Task.CompletedTask;

            public bool Tick(PlayerUnit ctx, float dt, out CarState next)
            {
                next = CarState.Dead;
                return false;
            }
        }
    }
}
