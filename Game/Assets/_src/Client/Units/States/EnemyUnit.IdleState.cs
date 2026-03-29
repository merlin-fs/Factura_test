using System.Threading;
using System.Threading;
using System.Threading.Tasks;
using Game.Core;
using Game.Core.Common.Fsm;
using Game.Core.Units;
using UnityEngine;

namespace Game.Client.Units
{
    public sealed partial class EnemyUnit
    {
        private sealed class IdleState : IState<EnemyContext, EnemyState>
        {
            public EnemyState Id => EnemyState.Idle;

            private float _timeLeft;

            public Task Enter(EnemyContext ctx, CancellationToken ct)
            {
                ctx.View.SetAnimationState(EnemyState.Idle);
                _timeLeft = Random.Range(
                    ctx.Unit._config.WanderIdleMinDuration,
                    ctx.Unit._config.WanderIdleMaxDuration);
                return Task.CompletedTask;
            }

            public Task Exit(EnemyContext ctx, CancellationToken ct) => Task.CompletedTask;

            public bool Tick(EnemyContext ctx, float dt, out EnemyState next)
            {
                if (!ctx.Unit.Stats.Get<HpStat>().IsAlive)
                {
                    next = EnemyState.Dead;
                    return true;
                }

                var target = ctx.Unit._session.Player;
                if (target != null &&
                    Vector3.Distance(ctx.Unit._transform.position, target.Position) <= ctx.Unit._config.AggroRadius)
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
