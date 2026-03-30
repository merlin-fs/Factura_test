using System.Threading;
using System.Threading.Tasks;
using Game.Client.Views;
using Game.Core;
using Game.Core.Common.Fsm;
using Game.Core.Services;

namespace Game.Client.Units
{
    public sealed partial class EnemyUnit
    {
        /// <summary>
        /// Стан загибелі ворога. Відв'язує колайдер, відтворює VFX,
        /// обирає анімацію залежно від джерела шкоди та сповіщає фабрику після завершення анімації.
        /// </summary>
        private sealed class DeadState : IState<EnemyContext, EnemyState>
        {
            private const string VfxBloodBurstId = "BloodBurst";

            public EnemyState Id => EnemyState.Dead;

            /// <summary>
            /// Обирає анімацію смерті: Projectile → Dead-анімація з очікуванням; інакше миттєве сповіщення.
            /// </summary>
            public async Task Enter(EnemyContext ctx, CancellationToken ct)
            {
                ctx.View.GetComponent<UnitColliderLink>()?.Unbind();
                ctx.View.PlayVfx(VfxBloodBurstId);
                if (ctx.KillSource == DamageSource.Projectile)
                {
                    ctx.View.SetAnimationState(EnemyState.Dead);
                    await ctx.View.WaitForStateComplete(EnemyState.Dead, ct);
                }
                ctx.Unit._onDied?.Invoke(ctx.Unit);
            }

            public Task Exit(EnemyContext ctx, CancellationToken ct) => Task.CompletedTask;

            public bool Tick(EnemyContext ctx, float dt, out EnemyState next)
            {
                next = EnemyState.Dead;
                return false;
            }
        }
    }
}
