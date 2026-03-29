using System;
using System.Collections.Generic;
using System.Linq;
using FTg.Common;
using UnityEngine;

namespace Game.Core.Units
{
    public abstract class UnitConfig : ScriptableObject
    {
        [SerializeReference, SelectInstance(typeof(UnitStat))]
        private UnitStat[] stats = Array.Empty<UnitStat>();

        [SerializeReference, SelectInstance(typeof(ISkill))]
        private ISkill[] skills  = Array.Empty<ISkill>();
        
        [field: SerializeField]
        public LayerMask TargetMask { get; private set; } = default;

        public IEnumerable<UnitStat> CreateStats()
            => from stat in stats where stat != null select stat.Clone();

        public IEnumerable<ISkill> CreateSkills()
            => from skill in skills where skill != null select skill.Clone();
    }
}
