using UnityEngine;

namespace Game.Core.Units
{
    /// <summary>
    /// Базовий клас для всіх ігрових юнітів (гравець, вороги).
    /// Містить статистики, навички та маску цілей.
    /// </summary>
    public abstract class Unit
    {
        /// <summary>Набір статистик юніта (HP тощо).</summary>
        public readonly UnitStats  Stats;
        /// <summary>Набір навичок юніта.</summary>
        public readonly UnitSkills Skills;
        /// <summary>Маска шарів Unity для виявлення цілей.</summary>
        public readonly LayerMask  TargetMask;

        /// <summary>Поточна позиція юніта у світовому просторі.</summary>
        public abstract Vector3 Position { get; }
        /// <summary>Точка прицілювання для ворогів і снарядів.</summary>
        public abstract Vector3 AimPoint { get; }
        /// <summary>Напрямок руху юніта вперед.</summary>
        public virtual  Vector3 Forward  => Vector3.forward;

        /// <summary>
        /// Ініціалізує юніт на основі конфігурації.
        /// </summary>
        /// <param name="config">Конфігурація юніта.</param>
        protected Unit(UnitConfig config)
        {
            Stats      = new UnitStats(config.CreateStats());
            Skills     = new UnitSkills(config.CreateSkills());
            TargetMask = config.TargetMask;
        }
    }
}
