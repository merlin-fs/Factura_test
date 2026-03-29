using Game.Core;
using Game.Core.Common.Fsm;
using Game.Core.Units;
using UnityEngine;
namespace Game.Client.Units
{
    public sealed partial class EnemyUnit
    {
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
                if (target == null) { next = EnemyState.Chase; return false; }
                var origin    = ctx.Position;
                var toTarget  = target.Position - origin;
                var context   = new AttackContext(ctx, origin, toTarget.normalized, ctx.TargetMask, target);
                if (ctx.Skills.Get<IAttackSkill>().CanUse(context))
                {
                    next = EnemyState.Attack;
                    return true;
                }
                var dist = toTarget.magnitude;
                var dir  = toTarget / dist;
                ctx._transform.SetPositionAndRotation(
                    ctx.Position + dir * (ctx._config.MoveSpeed * dt),
                    Quaternion.LookRotation(dir));
                // Cull � enemy is far behind the player
                if (target.Position.z - ctx.Position.z > CullBehindDistance)
                    ctx._onOutOfRange?.Invoke(ctx);
                next = EnemyState.Chase;
                return false;
            }
        }
    }
}
