using System.Threading;
using System.Threading.Tasks;
using Game.Core;
using Game.Core.Common.Fsm;
namespace Game.Client.Units
{
    public sealed partial class EnemyUnit
    {
        private sealed class DeadState : IState<EnemyContext, EnemyState>
        {
            public EnemyState Id => EnemyState.Dead;

            /// <summary>
            /// Запускает анимацию смерти и ждёт её завершения.
            /// Только после этого вызывает _onDied — враг убирается со сцены.
            /// </summary>
            public async Task Enter(EnemyContext ctx, CancellationToken ct)
            {
                ctx.View.SetAnimationState(EnemyState.Dead);
                await ctx.View.WaitForAnimationComplete(ct);
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
