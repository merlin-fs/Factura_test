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
        private sealed class ChaseState : IState<EnemyContext, EnemyState>
        {
            public EnemyState Id => EnemyState.Chase;
            public Task Enter(EnemyContext ctx, CancellationToken ct)
            {
                ctx.View.SetAnimationState(EnemyState.Chase);
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
                if (target == null) { next = EnemyState.Chase; return false; }
                var origin    = ctx.Unit.Position;
                var toTarget  = target.Position - origin;
                var context   = new AttackContext(ctx.Unit, origin, toTarget.normalized, ctx.Unit.TargetMask, target);
                if (ctx.Unit.Skills.Get<IAttackSkill>().CanUse(context))
                {
                    next = EnemyState.Attack;
                    return true;
                }
                var dist = toTarget.magnitude;
                var dir  = toTarget / dist;
                ctx.Unit._transform.SetPositionAndRotation(
                    ctx.Unit.Position + dir * (ctx.Unit._config.MoveSpeed * dt),
                    Quaternion.LookRotation(dir));
                // Cull � enemy is far behind the player
                if (target.Position.z - ctx.Unit.Position.z > CullBehindDistance)
                    ctx.Unit._onOutOfRange?.Invoke(ctx.Unit);
                next = EnemyState.Chase;
                return false;
            }
        }
    }
}
