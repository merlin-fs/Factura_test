using Game.Core.Units;
using UnityEngine;

namespace Game.Core.Services
{
    /// <summary>
    /// Реєстр відповідності колайдерів Unity до ігрових юнітів.
    /// Використовується для ідентифікації юніта за колайдером при фізичних перевірках.
    /// </summary>
    public interface IUnitRegistry
    {
        /// <summary>
        /// Прив'язує колайдер до юніта.
        /// </summary>
        /// <param name="collider">Колайдер юніта.</param>
        /// <param name="unit">Юніт, пов'язаний із колайдером.</param>
        void Register(Collider collider, Unit unit);

        /// <summary>
        /// Видаляє прив'язку колайдера.
        /// </summary>
        /// <param name="collider">Колайдер для видалення.</param>
        void Unregister(Collider collider);

        /// <summary>
        /// Намагається знайти юніт за колайдером.
        /// </summary>
        /// <param name="collider">Колайдер для пошуку.</param>
        /// <param name="unit">Знайдений юніт або <c>null</c>.</param>
        /// <returns><c>true</c>, якщо юніт знайдено.</returns>
        bool TryGet(Collider collider, out Unit unit);
    }
}