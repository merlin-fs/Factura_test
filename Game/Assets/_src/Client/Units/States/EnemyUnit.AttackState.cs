using Game.Core;
using Game.Core.Common.Fsm;
using Game.Core.Units;
using UnityEngine;
namespace Game.Client.Units
{
    public sealed partial class EnemyUnit
    {
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
                var skill     = ctx.Skills.Get<IAttackSkill>();
                var target    = ctx._session.Player;
                var origin    = ctx.Position;
                var direction = (target.Position - origin).normalized;
                var context   = new AttackContext(ctx, origin, direction, ctx.TargetMask, target);
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
    }
}
