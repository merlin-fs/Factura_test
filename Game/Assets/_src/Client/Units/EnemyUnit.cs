using System;
using Game.Core;
using Game.Core.Common;
using Game.Core.Common.Fsm;
using Game.Core.Session;
using Game.Core.Units;
using Game.Client.Config;
using Game.Client.Views;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

namespace Game.Client.Units
{
    public sealed class EnemyUnit : Unit, IDisposable
    {
        private const float CullBehindDistance = 10f;

        public override Vector3 Position => _transform.position;

        private readonly Transform _transform;
        private GameSession _session;
        private readonly Action<EnemyUnit> _onDied;
        private readonly Action<EnemyUnit> _onOutOfRange;
        private readonly StateMachine<EnemyUnit, EnemyState> _fsm;
        private TickSystemRegistry _tickSystemRegistry;
        private readonly EnemyConfig _config;

        private float _nextAttackTime;

        public EnemyUnit(EnemyConfig config, EnemyView view,
            Action<EnemyUnit>  onDied,
            Action<EnemyUnit>  onOutOfRange) : base(config)
        {
            _transform      = view.transform;
            _config = config;
            _onDied       = onDied;
            _onOutOfRange = onOutOfRange;

            _fsm = new StateMachine<EnemyUnit, EnemyState>(this)
                .Add(new IdleState())
                .Add(new WanderState())
                .Add(new ChaseState())
                .Add(new AttackState())
                .Add(new DeadState());
            _fsm.Start(EnemyState.Idle);
        }

        [Inject]
        private void Inject(Container sessionContainer, GameSession session, TickSystemRegistry tickSystemRegistry)
        {
            if (Skills.Has<IAttackSkill>()) AttributeInjector.Inject(Skills.Get<IAttackSkill>(), sessionContainer);
            _session = session;
            _tickSystemRegistry  = tickSystemRegistry;
            _tickSystemRegistry.Register(_fsm);
        }
        
        public void Dispose()
        {
            _tickSystemRegistry.Unregister(_fsm);
        }
        #region fsm states
        private sealed class IdleState : IState<EnemyUnit, EnemyState>
        {
            public EnemyState Id => EnemyState.Idle;

            private float _timeLeft;

            public void Enter(EnemyUnit ctx)
            {
                _timeLeft = UnityEngine.Random.Range(
                    ctx._config.WanderIdleMinDuration,
                    ctx._config.WanderIdleMaxDuration);
            }

            public void Exit(EnemyUnit ctx) { }

            public bool Tick(EnemyUnit ctx, float dt, out EnemyState next)
            {
                if (!ctx.Stats.Get<HpStat>().IsAlive)
                {
                    next = EnemyState.Dead;
                    return true;
                }

                var target = ctx._session.Player;
                if (target != null && Vector3.Distance(ctx._transform.position, target.Position) <= ctx._config.AggroRadius)
                {
                    next = EnemyState.Chase;
                    return true;
                }

                _timeLeft -= dt;
                if (_timeLeft <= 0f)
                {
                    next = EnemyState.Wander;
                    return true;
                }

                next = EnemyState.Idle;
                return false;
            }
        }

        private sealed class WanderState : IState<EnemyUnit, EnemyState>
        {
            public EnemyState Id => EnemyState.Wander;

            private Vector3 _direction;
            private float   _timeLeft;
            private float   _steerTimer;

            public void Enter(EnemyUnit ctx)
            {
                _direction  = RandomFlatDirection();
                _timeLeft   = UnityEngine.Random.Range(ctx._config.WanderMinDuration, ctx._config.WanderMaxDuration);
                _steerTimer = UnityEngine.Random.Range(0.6f, 1.4f);
            }

            public void Exit(EnemyUnit ctx) { }

            public bool Tick(EnemyUnit ctx, float dt, out EnemyState next)
            {
                if (!ctx.Stats.Get<HpStat>().IsAlive)
                {
                    next = EnemyState.Dead;
                    return true;
                }

                var target = ctx._session.Player;
                if (target != null && Vector3.Distance(ctx._transform.position, target.Position) <= ctx._config.AggroRadius)
                {
                    next = EnemyState.Chase;
                    return true;
                }

                // Случайное отклонение курса — характерное "шатание" зомби
                _steerTimer -= dt;
                if (_steerTimer <= 0f)
                {
                    var angle = UnityEngine.Random.Range(-50f, 50f);
                    _direction  = Quaternion.Euler(0f, angle, 0f) * _direction;
                    _steerTimer = UnityEngine.Random.Range(0.6f, 1.4f);
                }

                // Медленное движение с плавным поворотом (не мгновенным)
                var targetRotation = Quaternion.LookRotation(_direction);
                ctx._transform.SetPositionAndRotation(
                    ctx.Position + _direction * (ctx._config.WanderSpeed * dt),
                    Quaternion.Slerp(ctx._transform.rotation, targetRotation, dt * 3f));

                _timeLeft -= dt;
                if (_timeLeft <= 0f)
                {
                    next = EnemyState.Idle;
                    return true;
                }

                next = EnemyState.Wander;
                return false;
            }

            private static Vector3 RandomFlatDirection()
            {
                var rad = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
                return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
            }
        }

        private sealed class ChaseState : IState<EnemyUnit, EnemyState>
        {
            public EnemyState Id => EnemyState.Chase;
            public void Enter(EnemyUnit ctx) { }
            public void Exit(EnemyUnit ctx)  { }

            public bool Tick(EnemyUnit ctx, float dt, out EnemyState next)
            {
                if (!ctx.Stats.Get<HpStat>().IsAlive)
                {
                    next = EnemyState.Dead; 
                    return true;
                }

                var target = ctx._session.Player;
                if (target == null)
                {
                    next = EnemyState.Chase; 
                    return false;
                }

                
                var origin = ctx.Position;
                var toTarget = target.Position - origin;
                var context = new AttackContext(ctx, origin, toTarget.normalized, ctx.TargetMask, target);
                if (ctx.Skills.Get<IAttackSkill>().CanUse(context))
                {
                    next = EnemyState.Attack;
                    return true;
                }
                var dist = toTarget.magnitude;
                var dir = toTarget / dist;
                ctx._transform.SetPositionAndRotation(ctx.Position + dir * (ctx._config.MoveSpeed * dt),
                    Quaternion.LookRotation(dir));

                // Cull — enemy is far behind the player
                if (target.Position.z - ctx.Position.z > CullBehindDistance)
                    ctx._onOutOfRange?.Invoke(ctx);
                
                next = EnemyState.Chase;
                return false;
            }
        }

        private sealed class AttackState : IState<EnemyUnit, EnemyState>
        {
            public EnemyState Id => EnemyState.Attack;
            public void Enter(EnemyUnit ctx) { }
            public void Exit(EnemyUnit ctx)  { }

            public bool Tick(EnemyUnit ctx, float dt, out EnemyState next)
            {
                if (!ctx.Stats.Get<HpStat>().IsAlive)
                {
                    next = EnemyState.Dead; 
                    return true;
                }

                var skill = ctx.Skills.Get<IAttackSkill>();
                var target = ctx._session.Player;
                var origin = ctx.Position;
                var direction = (target.Position - origin).normalized;
                var context = new AttackContext(ctx, origin, direction, ctx.TargetMask, target);
                
                if (!skill.CanUse(context))
                {
                    next = EnemyState.Chase;
                    return true;
                }

                if (Time.time >= ctx._nextAttackTime)
                {
                    ctx._nextAttackTime = Time.time + ctx._config.AttackInterval;
                    skill.Use(context);
                }

                next = EnemyState.Attack;
                return false;
            }
        }

        private sealed class DeadState : IState<EnemyUnit, EnemyState>
        {
            public EnemyState Id => EnemyState.Dead;
            public void Enter(EnemyUnit ctx) => ctx._onDied?.Invoke(ctx);
            public void Exit(EnemyUnit ctx)  { }

            public bool Tick(EnemyUnit ctx, float dt, out EnemyState next)
            {
                next = EnemyState.Dead;
                return false;
            }
        }
        #endregion

    }
}
