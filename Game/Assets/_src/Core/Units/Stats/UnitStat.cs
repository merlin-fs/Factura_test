using System;

namespace Game.Core.Units
{
    [Serializable]
    public abstract class UnitStat
    {
        public float Value { get; protected set; }
        public abstract UnitStat Clone();
    }
}
