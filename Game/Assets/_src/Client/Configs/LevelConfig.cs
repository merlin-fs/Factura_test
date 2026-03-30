using UnityEngine;

namespace Game.Client.Config
{
    /// <summary>
    /// ScriptableObject-конфігурація рівня.
    /// Визначає довжину рівня та налаштування спауну ворогів на дорозі.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Game/Configs/Level Config")]
    public sealed class LevelConfig : ScriptableObject
    {
        /// <summary>Довжина рівня вздовж осі Z у світових одиницях.</summary>
        [field: SerializeField, Min(1f)]
        public float LevelLength { get; private set; } = 200f;

        /// <summary>Конфігурація спауну ворогів на дорозі.</summary>
        [field: SerializeField]
        public RoadSpawnConfig RoadSpawn { get; private set; }
    }
}
