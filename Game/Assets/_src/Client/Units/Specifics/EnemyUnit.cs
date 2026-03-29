using System;
using Game.Core.Common;
using Game.Core.Common.Fsm;
using Game.Core.Session;
using Game.Core.Units;
using Game.Client.Config;
using Game.Client.Views;
using Game.Core;
using VContainer;
using UnityEngine;

namespace Game.Client.Units
{
    /// <summary>
    /// Pure C# enemy unit. FSM states are defined in separate partial-class files:
    /// EnemyUnit.IdleState.cs, WanderState.cs, ChaseState.cs, AttackState.cs, DeadState.cs
    /// </summary>
    public sealed partial class EnemyUnit : BaseUnit, IDisposable
    {
        private const float CullBehindDistance = 10f;
        public override Vector3 Position => _transform.position;

        private readonly Transform                              _transform;
        private readonly Action<EnemyUnit>                     _onDied;
        private readonly Action<EnemyUnit>                     _onOutOfRange;
        private readonly StateMachine<EnemyContext, EnemyState> _fsm;
        private readonly EnemyConfig                           _config;
        private          TickSystemRegistry                    _tickSystemRegistry;
        private          GameSession                           _session;
        private          float                                 _nextAttackTime;

        public EnemyUnit(
            EnemyConfig        config,
            EnemyView      view,
            Action<EnemyUnit>  onDied,
            Action<EnemyUnit>  onOutOfRange,
            GameSession        session,
            TickSystemRegistry tickSystemRegistry,
            IObjectResolver    container) : base(config, container)
        {
            _transform    = view.transform;
            _config       = config;
            _onDied       = onDied;
            _onOutOfRange = onOutOfRange;

            var context = new EnemyContext(this, view);

            _fsm = new StateMachine<EnemyContext, EnemyState>(context)
                .Add(new IdleState())
                .Add(new WanderState())
                .Add(new ChaseState())
                .Add(new AttackState())
                .Add(new DeadState());
            _fsm.Start(EnemyState.Idle);

            // Skills and stats are injected by BaseUnit via container.Inject delegate.

            _session            = session;
            _tickSystemRegistry = tickSystemRegistry;
            _tickSystemRegistry.Register(_fsm);
        }


        public void Dispose()
        {
            _tickSystemRegistry.Unregister(_fsm);
            _fsm.Dispose();
        }
    }
}


