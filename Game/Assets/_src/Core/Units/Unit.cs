using UnityEngine;

namespace Game.Core.Units
{
    public abstract class Unit
    {
        public readonly UnitStats  Stats;
        public readonly UnitSkills Skills;
        public readonly LayerMask  TargetMask;

        public abstract Vector3 Position { get; }

        protected Unit(UnitConfig config)
        {
            Stats      = new UnitStats(config.CreateStats());
            Skills     = new UnitSkills(config.CreateSkills());
            TargetMask = config.TargetMask;
        }
    }
}
