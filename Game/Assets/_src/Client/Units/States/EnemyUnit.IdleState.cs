using Game.Core;
using Game.Core.Common.Fsm;
using Game.Core.Units;
using UnityEngine;

namespace Game.Client.Units
{
    public sealed partial class EnemyUnit
    {
        private sealed class IdleState : IState<EnemyUnit, EnemyState>
        {
            public EnemyState Id => EnemyState.Idle;

            private float _timeLeft;

            public void Enter(EnemyUnit ctx)
            {
                _timeLeft = Random.Range(
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
                if (target != null &&
                    Vector3.Distance(ctx._transform.position, target.Position) <= ctx._config.AggroRadius)
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
    }
}

