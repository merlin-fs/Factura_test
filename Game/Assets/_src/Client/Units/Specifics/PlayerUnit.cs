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
    public sealed class PlayerUnit : BaseUnit
    {
        public override Vector3 Position => _transform.position;

        private readonly float                               _speed;
        private readonly Transform                           _transform;
        private readonly StateMachine<PlayerUnit, CarState>  _fsm;

        public PlayerUnit(PlayerConfig config, CarBaseView baseView,
            TickSystemRegistry tickSystemRegistry, IObjectResolver container) : base(config, container)
        {

            _transform = baseView.transform;
            _speed = config.MoveSpeed;

            _fsm = new StateMachine<PlayerUnit, CarState>(this)
                .Add(new DrivingState())
                .Add(new DeadState());
            _fsm.Start(CarState.Driving);

            tickSystemRegistry.Register(_fsm);
        }

        #region fsm states
        private sealed class DrivingState : IState<PlayerUnit, CarState>
        {
            public CarState Id => CarState.Driving;
            public void Enter(PlayerUnit ctx) { }
            public void Exit(PlayerUnit ctx)  { }

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

        private sealed class DeadState : IState<PlayerUnit, CarState>
        {
            public CarState Id => CarState.Dead;
            public void Enter(PlayerUnit ctx) { }
            public void Exit(PlayerUnit ctx)  { }

            public bool Tick(PlayerUnit ctx, float dt, out CarState next)
            {
                next = CarState.Dead;
                return false;
            }
        }
        #endregion
    }
}
