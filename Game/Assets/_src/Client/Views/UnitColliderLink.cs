using Game.Core.Services;
using Game.Core.Units;
using VContainer;
using UnityEngine;

namespace Game.Client.Views
{
    /// <summary>
    /// Прив'язує колайдер GameObject до ігрового юніта через <see cref="IUnitRegistry"/>.
    /// Розміщується на тому самому GameObject, що й фізичний колайдер ворога або гравця.
    /// </summary>
    public sealed class UnitColliderLink : MonoBehaviour
    {
        [SerializeField] private Collider unitCollider;

        private IUnitRegistry _unitRegistry;

        [Inject]
        private void Inject(IUnitRegistry unitRegistry)
        {
            _unitRegistry = unitRegistry;
        }

        /// <summary>
        /// Реєструє колайдер у реєстрі та прив'язує його до вказаного юніта.
        /// </summary>
        /// <param name="unit">Юніт, якому належить цей колайдер.</param>
        public void Bind(Unit unit)
        {
            _unitRegistry.Register(unitCollider, unit);
        }

        /// <summary>
        /// Скасовує реєстрацію колайдера у реєстрі.
        /// </summary>
        public void Unbind()
        {
            _unitRegistry.Unregister(unitCollider);
        }
    }
}