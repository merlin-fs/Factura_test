using System;
using FTg.Common.Observables;
using UnityEngine;

namespace Game.Core.Units
{
    [Serializable]
    public sealed class HpStat : UnitStat
    {
        [SerializeField, Min(1f)]
        private float max = 100f;
        public float Max     => max;
        public float Ratio   => max > 0f ? Value / max : 0f;
        public bool  IsAlive => Value > 0f;

        [NonSerialized]
        public readonly ObservableEvent<float> OnChange = new();

        public HpStat() { }
        public HpStat(float max)
        {
            this.max  = max;
            Value = max;
        }

        public override UnitStat Clone() => new HpStat(max);

        internal void Apply(float delta)
        {
            Value = Mathf.Clamp(Value + delta, 0f, max);
            OnChange.Invoke(Value);
        }
    }
}
