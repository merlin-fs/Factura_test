using System;
using System.Collections.Generic;

namespace Game.Core.Units
{
    /// <summary>
    /// Typed container for a unit's runtime stats (HP, speed, etc.).
    /// Built once from UnitConfig.CreateStats() and owned by Unit.
    /// </summary>
    public sealed class UnitStats
    {
        private readonly Dictionary<Type, UnitStat> _items = new();
        private readonly List<UnitStat>             _unique = new();

        internal UnitStats(IEnumerable<UnitStat> stats)
        {
            foreach (var s in stats)
            {
                _items[s.GetType()] = s;
                _unique.Add(s);
            }
        }

        /// <summary>All unique stat instances in insertion order.</summary>
        public IReadOnlyList<UnitStat> All => _unique;

        public T Get<T>() where T : UnitStat
        {
            if (_items.TryGetValue(typeof(T), out var s))
                return (T)s;
            throw new InvalidOperationException($"Stat '{typeof(T).Name}' is not registered on unit.");
        }

        public bool Has<T>() where T : UnitStat => _items.ContainsKey(typeof(T));
    }
}
