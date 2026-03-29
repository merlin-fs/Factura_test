using Game.Core;
using Game.Core.Common.Fsm;
namespace Game.Client.Units
{
    public sealed partial class EnemyUnit
    {
        private sealed class DeadState : IState<EnemyUnit, EnemyState>
        {
            public EnemyState Id => EnemyState.Dead;
            public void Enter(EnemyUnit ctx) => ctx._onDied?.Invoke(ctx);
            public void Exit(EnemyUnit ctx)  { }
            public bool Tick(EnemyUnit ctx, float dt, out EnemyState next)
            {
                next = EnemyState.Dead;
                return false;
            }
        }
    }
}
