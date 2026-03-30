using Game.Client.Views;
using Game.Core.Services;

namespace Game.Client.Units
{
    /// <summary>
    /// Контекст FSM-станів <see cref="EnemyUnit"/>.
    /// Надає станам доступ до логіки юніта та його вигляду.
    /// </summary>
    public sealed class EnemyContext
    {
        /// <summary>Логічний юніт ворога.</summary>
        public EnemyUnit Unit { get; }
        /// <summary>Візуальне представлення ворога.</summary>
        public EnemyView View { get; }

        /// <summary>
        /// Джерело шкоди, що спричинило загибель юніта.
        /// Встановлюється в <see cref="EnemyUnit"/> перед переходом у стан Dead.
        /// <c>DeadState</c> використовує це значення для вибору анімації.
        /// </summary>
        public DamageSource KillSource { get; set; } = DamageSource.Unknown;

        /// <summary>
        /// Створює контекст.
        /// </summary>
        /// <param name="unit">Логічний юніт.</param>
        /// <param name="view">Візуальне представлення.</param>
        public EnemyContext(EnemyUnit unit, EnemyView view)
        {
            Unit = unit;
            View = view;
        }
    }
}
