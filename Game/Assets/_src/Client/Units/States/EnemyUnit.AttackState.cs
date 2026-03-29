using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Core.Common.Fsm;
using Game.Core.Units;
using UnityEngine;
namespace Game.Client.Units
{
    public sealed partial class EnemyUnit
    {
        private sealed class AttackState : IState<EnemyContext, EnemyState>
        {
            public EnemyState Id => EnemyState.Attack;

            public Task Enter(EnemyContext ctx, CancellationToken ct)
            {
                ctx.View.SetAnimationState(EnemyState.Attack);
                return Task.CompletedTask;
            }

            /// <summary>
            /// Ждёт, пока анимация атаки не проиграет до конца,
            /// прежде чем позволить StateMachine перейти к следующему состоянию.
            /// </summary>
            public Task Exit(EnemyContext ctx, CancellationToken ct)
                => ctx.View.WaitForAnimationComplete(ct).AsTask();

            public bool Tick(EnemyContext ctx, float dt, out EnemyState next)
            {
                if (!ctx.Unit.Stats.Get<HpStat>().IsAlive)
                {
                    next = EnemyState.Dead;
                    return true;
                }
                var skill     = ctx.Unit.Skills.Get<IAttackSkill>();
                var target    = ctx.Unit._session.Player;
                var origin    = ctx.Unit.Position;
                var direction = (target.Position - origin).normalized;
                var context   = new AttackContext(ctx.Unit, origin, direction, ctx.Unit.TargetMask, target);
                if (!skill.CanUse(context))
                {
                    next = EnemyState.Chase;
                    return true;
                }
                if (Time.time >= ctx.Unit._nextAttackTime)
                {
                    ctx.Unit._nextAttackTime = Time.time + ctx.Unit._config.AttackInterval;
                    skill.Use(context);
                }
                next = EnemyState.Attack;
                return false;
            }
        }
    }
}
