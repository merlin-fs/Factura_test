using UnityEngine;

namespace Game.Core.Units
{
    /// <summary>
    /// Утиліта для визначення точки прицілювання юніта у світовому просторі.
    /// </summary>
    public static class AimPointUtility
    {
        /// <summary>
        /// Повертає точку прицілювання для заданого юніта.
        /// За замовчуванням — позиція юніта; розширюйте за потреби.
        /// </summary>
        /// <param name="unit">Юніт, для якого визначається точка прицілювання.</param>
        /// <returns>Точка прицілювання у світовому просторі.</returns>
        public static Vector3 Resolve(Unit unit) => unit.AimPoint;
    }
}
