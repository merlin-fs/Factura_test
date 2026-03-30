using UnityEngine;

namespace Game.Core.Units
{
    /// <summary>
    /// Фабрика ворогів. Реалізується у шарі Client (<c>EnemyFactory</c>).
    /// Дозволяє Core-шару спауніти ворогів без залежності від клієнтських ScriptableObject.
    /// </summary>
    public interface IEnemyFactory
    {
        /// <summary>
        /// Спаунить ворога у вказаній позиції та орієнтації.
        /// </summary>
        /// <param name="config">Конфігурація юніта.</param>
        /// <param name="position">Початкова позиція у світовому просторі.</param>
        /// <param name="rotation">Початкова орієнтація.</param>
        void Spawn(UnitConfig config, Vector3 position, Quaternion rotation);
    }
}
