using System;
using FTg.Common.Observables;
using UnityEngine;

namespace Game.Core.Units
{
    /// <summary>
    /// Базова статистика юніта (HP, броня тощо).
    /// Зберігає поточне значення, максимум та сповіщає підписників про зміни.
    /// </summary>
    [Serializable]
    public abstract class UnitStat
    {
        [SerializeField, Min(1f)]
        protected float max = 100f;

        [NonSerialized]
        protected ObservableEvent<UnitStat> _onChange = new();

        /// <summary>Подія, що спрацьовує при зміні значення статистики.</summary>
        public IObservable<UnitStat> OnChange => _onChange;
        /// <summary>Поточне значення статистики.</summary>
        public float Value { get; protected set; }
        /// <summary>Максимальне значення статистики.</summary>
        public float Max   => max;
        /// <summary>Відношення поточного значення до максимального (0..1).</summary>
        public float Ratio => max > 0f ? Value / max : 0f;

        /// <summary>
        /// Повертає незалежну копію статистики для нового юніта.
        /// </summary>
        public abstract UnitStat Clone();
    }
}
