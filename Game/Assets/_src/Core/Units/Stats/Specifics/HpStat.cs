using System;
using UnityEngine;

namespace Game.Core.Units
{
    /// <summary>
    /// Статистика здоров'я (HP) юніта.
    /// Забезпечує зміну значення та перевірку стану живий/мертвий.
    /// </summary>
    [Serializable]
    public sealed class HpStat : UnitStat
    {
        /// <summary>Повертає <c>true</c>, якщо юніт живий (HP > 0).</summary>
        public bool IsAlive => Value > 0f;

        /// <summary>Створює екземпляр HpStat із максимальним значенням за замовчуванням.</summary>
        public HpStat() { }

        /// <summary>
        /// Створює екземпляр HpStat із заданим максимумом, ініціалізуючи HP до максимуму.
        /// </summary>
        /// <param name="max">Максимальне значення HP.</param>
        public HpStat(float max)
        {
            this.max = max;
            Value    = max;
        }

        /// <inheritdoc/>
        public override UnitStat Clone() => new HpStat(max);

        /// <summary>
        /// Змінює поточне HP на задельту (від'ємна — шкода, додатня — лікування).
        /// Значення затискається в діапазоні [0, max].
        /// </summary>
        /// <param name="delta">Зміна HP.</param>
        internal void Apply(float delta)
        {
            Value = Mathf.Clamp(Value + delta, 0f, max);
            _onChange.Raise(this);
        }
    }
}
