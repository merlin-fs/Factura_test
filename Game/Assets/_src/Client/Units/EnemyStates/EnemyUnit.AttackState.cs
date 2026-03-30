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
        /// <summary>
        /// Стан атаки ворога. Виконує атаку через навичку і чекає на завершення анімації при виході.
        /// </summary>
        private sealed class AttackState : IState<EnemyContext, EnemyState>
        {
            public EnemyState Id => EnemyState.Attack;

            public Task Enter(EnemyContext ctx, CancellationToken ct)
            {
                ctx.View.SetAnimationState(EnemyState.Attack);
                return Task.CompletedTask;
            }

            /// <summary>
            /// Очікує повного програвання анімації атаки перед переходом до наступного стану.
            /// </summary>
            public Task Exit(EnemyContext ctx, CancellationToken ct)
                => ctx.View.WaitForStateComplete(EnemyState.Attack, ct).AsTask();

            public bool Tick(EnemyContext ctx, float dt, out EnemyState next)
            {
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
