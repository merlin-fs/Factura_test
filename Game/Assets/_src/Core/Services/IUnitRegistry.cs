using Game.Core.Units;
using UnityEngine;

namespace Game.Core.Services
{
    public interface IUnitRegistry
    {
        void Register(Collider collider, Unit unit);
        void Unregister(Collider collider);
        bool TryGet(Collider collider, out Unit unit);
    }
}