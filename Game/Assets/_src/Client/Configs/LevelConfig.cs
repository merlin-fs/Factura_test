using UnityEngine;

namespace Game.Client.Config
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Game/Configs/Level Config")]
    public sealed class LevelConfig : ScriptableObject
    {
        [field: SerializeField, Min(1f)]
        public float LevelLength { get; private set; } = 200f;
        [field: SerializeField]
        public RoadSpawnConfig RoadSpawn { get; private set; }
    }
}

