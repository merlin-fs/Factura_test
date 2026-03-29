using System.Collections.Generic;
using Game.Core.Units;
using UnityEngine;

namespace Game.Core.Services
{
    public sealed class UnitRegistry: IUnitRegistry
    {
        private readonly Dictionary<Collider, Unit> _unitsByCollider = new();

        public void Register(Collider collider, Unit unit)
        {
            _unitsByCollider[collider] = unit;
        }

        public void Unregister(Collider collider)
        {
            _unitsByCollider.Remove(collider);
        }

        public bool TryGet(Collider collider, out Unit unit)
        {
            return _unitsByCollider.TryGetValue(collider, out unit);
        }
    }
}