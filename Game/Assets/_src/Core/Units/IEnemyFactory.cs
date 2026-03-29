using UnityEngine;

namespace Game.Core.Units
{
    /// <summary>
    /// Spawns an enemy at the given world position.
    /// Implemented in the Client layer (EnemyFactory).
    /// Uses <see cref="IEnemyData"/> so Core doesn't depend on client ScriptableObjects.
    /// </summary>
    public interface IEnemyFactory
    {
        void Spawn(UnitConfig config, Vector3 position, Quaternion rotation);
    }
}

