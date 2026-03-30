using System;
using System.Collections.Generic;
using System.Linq;
using FTg.Common;
using UnityEngine;

namespace Game.Core.Units
{
    /// <summary>
    /// Базова ScriptableObject-конфігурація юніта.
    /// Зберігає статистики та навички, що клонуються при створенні кожного екземпляра юніта.
    /// </summary>
    public abstract class UnitConfig : ScriptableObject
    {
        [SerializeReference, SelectInstance(typeof(UnitStat))]
        private UnitStat[] stats = Array.Empty<UnitStat>();

        [SerializeReference, SelectInstance(typeof(ISkill))]
        private ISkill[] skills  = Array.Empty<ISkill>();
        
        /// <summary>Маска шарів Unity для визначення цілей цього юніта.</summary>
        [field: SerializeField]
        public LayerMask TargetMask { get; private set; } = default;

        /// <summary>
        /// Повертає клони всіх статистик для нового екземпляра юніта.
        /// </summary>
        public IEnumerable<UnitStat> CreateStats()
            => from stat in stats where stat != null select stat.Clone();

        /// <summary>
        /// Повертає клони всіх навичок для нового екземпляра юніта.
        /// </summary>
        public IEnumerable<ISkill> CreateSkills()
            => from skill in skills where skill != null select skill.Clone();
    }
}
