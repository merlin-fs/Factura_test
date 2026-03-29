using Game.Core;
using Game.Core.Common.Fsm;
using Game.Core.Units;
using UnityEngine;
namespace Game.Client.Units
{
    public sealed partial class EnemyUnit
    {
        private sealed class WanderState : IState<EnemyUnit, EnemyState>
        {
            public EnemyState Id => EnemyState.Wander;
            private Vector3 _direction;
            private float   _timeLeft;
            private float   _steerTimer;
            public void Enter(EnemyUnit ctx)
            {
                _direction  = RandomFlatDirection();
                _timeLeft   = Random.Range(ctx._config.WanderMinDuration, ctx._config.WanderMaxDuration);
                _steerTimer = Random.Range(0.6f, 1.4f);
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
                _steerTimer -= dt;
                if (_steerTimer <= 0f)
                {
                    var angle = Random.Range(-50f, 50f);
                    _direction  = Quaternion.Euler(0f, angle, 0f) * _direction;
                    _steerTimer = Random.Range(0.6f, 1.4f);
                }
                var targetRotation = Quaternion.LookRotation(_direction);
                ctx._transform.SetPositionAndRotation(
                    ctx.Position + _direction * (ctx._config.WanderSpeed * dt),
                    Quaternion.Slerp(ctx._transform.rotation, targetRotation, dt * 3f));
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
