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
        private sealed class WanderState : IState<EnemyContext, EnemyState>
        {
            public EnemyState Id => EnemyState.Wander;
            private Vector3 _direction;
            private float   _timeLeft;
            private float   _steerTimer;
            public Task Enter(EnemyContext ctx, CancellationToken ct)
            {
                ctx.View.SetAnimationState(EnemyState.Wander);
                _direction  = RandomFlatDirection();
                _timeLeft   = Random.Range(ctx.Unit._config.WanderMinDuration, ctx.Unit._config.WanderMaxDuration);
                _steerTimer = Random.Range(0.6f, 1.4f);
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
                _steerTimer -= dt;
                if (_steerTimer <= 0f)
                {
                    var angle = Random.Range(-50f, 50f);
                    _direction  = Quaternion.Euler(0f, angle, 0f) * _direction;
                    _steerTimer = Random.Range(0.6f, 1.4f);
                }
                var targetRotation = Quaternion.LookRotation(_direction);
                ctx.Unit._transform.SetPositionAndRotation(
                    ctx.Unit.Position + _direction * (ctx.Unit._config.WanderSpeed * dt),
                    Quaternion.Slerp(ctx.Unit._transform.rotation, targetRotation, dt * 3f));
                _timeLeft -= dt;
                if (_timeLeft <= 0f) { next = EnemyState.Idle; return true; }
                next = EnemyState.Wander;
                return false;
            }
            private static Vector3 RandomFlatDirection()
            {
                var rad = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
            }
        }
    }
}
