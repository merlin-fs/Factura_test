using Game.Core.Services;
using Game.Core.Units;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Client.Views
{
    public sealed class UnitColliderLink : MonoBehaviour
    {
        [SerializeField] private Collider unitCollider;

        private IUnitRegistry _unitRegistry;

        [Inject]
        private void Inject(IUnitRegistry unitRegistry)
        {
            _unitRegistry = unitRegistry;
        }

        public void Bind(Unit unit)
        {
            _unitRegistry.Register(unitCollider, unit);
        }

        public void Unbind()
        {
            _unitRegistry.Unregister(unitCollider);
        }
    }
}