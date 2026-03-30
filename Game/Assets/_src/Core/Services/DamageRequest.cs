using Game.Core.Units;

namespace Game.Core.Services
{
    /// <summary>
    /// Незмінна структура запиту на нанесення шкоди юніту.
    /// </summary>
    public readonly struct DamageRequest
    {
        /// <summary>Юніт, що завдає шкоди.</summary>
        public Unit         Source       { get; }
        /// <summary>Юніт, що отримує шкоду.</summary>
        public Unit         Target       { get; }
        /// <summary>Величина шкоди.</summary>
        public int          Amount       { get; }
        /// <summary>Тип джерела шкоди.</summary>
        public DamageSource DamageSource { get; }

        /// <summary>
        /// Створює новий запит на шкоду.
        /// </summary>
        /// <param name="source">Атакуючий юніт.</param>
        /// <param name="target">Юніт-ціль.</param>
        /// <param name="amount">Величина шкоди.</param>
        /// <param name="damageSource">Тип джерела шкоди.</param>
        public DamageRequest(Unit source, Unit target, int amount, DamageSource damageSource = DamageSource.Unknown)
        {
            Source       = source;
            Target       = target;
            Amount       = amount;
            DamageSource = damageSource;
        }
    }
}
