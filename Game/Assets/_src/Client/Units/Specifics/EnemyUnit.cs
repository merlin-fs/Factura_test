using System;
using FTg.Common.Observables;
using Game.Core.Common;
using Game.Core.Common.Fsm;
using Game.Core.Services;
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
    /// Ворожий юніт на чистому C#. FSM-стани визначені у окремих partial-файлах:
    /// IdleState, WanderState, ChaseState, AttackState, DeadState.
    /// </summary>
    public sealed partial class EnemyUnit : BaseUnit, IDisposable
    {
        private const float CullBehindDistance = 10f;

        /// <inheritdoc/>
        public override Vector3 Position => _view.transform.position;
        /// <inheritdoc/>
        public override Vector3 AimPoint => _view.AimPoint.position;
        /// <inheritdoc/>
        public override Vector3 Forward  => _view.transform.forward;

        private readonly EnemyView                              _view;
        private readonly Action<EnemyUnit>                     _onDied;
        private readonly Action<EnemyUnit>                     _onOutOfRange;
        private readonly StateMachine<EnemyContext, EnemyState> _fsm;
        private readonly EnemyConfig                           _config;
        private readonly DamageService                         _damageService;
        private readonly IDisposable                           _diedSubscription;
        private readonly TickSystemRegistry                    _tickSystemRegistry;
        private readonly GameSession                           _session;
        private readonly EnemyContext                          _context;
        private          float                                 _nextAttackTime;
        private          bool                                  _isDead;

        /// <summary>
        /// Створює ворожого юніта, запускає FSM і підписується на подію загибелі.
        /// </summary>
        public EnemyUnit(
            EnemyConfig        config,
            EnemyView          view,
            Action<EnemyUnit>  onDied,
            Action<EnemyUnit>  onOutOfRange,
            GameSession        session,
            TickSystemRegistry tickSystemRegistry,
            DamageService      damageService,
            IObjectResolver    container) : base(config, container)
        {
            _view         = view;
            _config       = config;
            _onDied       = onDied;
            _onOutOfRange = onOutOfRange;

            _context = new EnemyContext(this, view);

            _fsm = new StateMachine<EnemyContext, EnemyState>(_context)
                .Add(new IdleState())
                .Add(new WanderState())
                .Add(new ChaseState())
                .Add(new AttackState())
                .Add(new DeadState());
            _fsm.Start(EnemyState.Idle);

            _session            = session;
            _tickSystemRegistry = tickSystemRegistry;
            _tickSystemRegistry.Register(_fsm);

            _diedSubscription = damageService.Died.Subscribe(OnDied);
        }

        private void OnDied((Unit unit, DamageSource source) data)
        {
            if (data.unit != this) return;
            if (_isDead) return;

            _isDead = true;
            _context.KillSource = data.source;
            _fsm.ForceTransitionTo(EnemyState.Dead);
        }

        /// <summary>
        /// Скасовує підписки, видаляє FSM з реєстру та звільняє ресурси.
        /// </summary>
        public void Dispose()
        {
            _diedSubscription?.Dispose();
            _tickSystemRegistry.Unregister(_fsm);
            _fsm.Dispose();
        }
    }
}
