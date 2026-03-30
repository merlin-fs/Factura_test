using System.Collections.Generic;
using Game.Core.Units;
using UnityEngine;

namespace Game.Core.Services
{
    /// <summary>
    /// Реалізація <see cref="IUnitRegistry"/> на основі словника.
    /// Зберігає відповідність між колайдерами Unity та юнітами гри.
    /// </summary>
    public sealed class UnitRegistry : IUnitRegistry
    {
        private readonly Dictionary<Collider, Unit> _unitsByCollider = new();

        /// <inheritdoc/>
        public void Register(Collider collider, Unit unit)
        {
            _unitsByCollider[collider] = unit;
        }

        /// <inheritdoc/>
        public void Unregister(Collider collider)
        {
            _unitsByCollider.Remove(collider);
        }

        /// <inheritdoc/>
        public bool TryGet(Collider collider, out Unit unit)
        {
            return _unitsByCollider.TryGetValue(collider, out unit);
        }
    }
}