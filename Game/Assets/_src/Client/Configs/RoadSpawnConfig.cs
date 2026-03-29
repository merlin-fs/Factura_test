using System;
using UnityEngine;

namespace Game.Client.Config
{
    [CreateAssetMenu(fileName = "RoadSpawnConfig", menuName = "Game/Configs/Road Spawn Config")]
    public sealed class RoadSpawnConfig : ScriptableObject
    {
        [field: SerializeField, Min(0f)]
        public float InitialSpawnDistance { get; private set; } = 8f;

        [field: SerializeField, Min(1f)]
        public float MinSpawnAheadDistance { get; private set; } = 18f;

        [field: SerializeField, Min(1f)]
        public float MaxSpawnAheadDistance { get; private set; } = 28f;

        [field: SerializeField, Min(1f)]
        public float MinDistanceBetweenGroups { get; private set; } = 10f;

        [field: SerializeField, Min(1f)]
        public float MaxDistanceBetweenGroups { get; private set; } = 18f;

        [field: SerializeField, Min(1)]
        public int MaxAliveEnemies { get; private set; } = 15;

        [field: SerializeField]
        public float SpawnY { get; private set; } = 0f;

        [field: SerializeField]
        public float[] LaneOffsetsX { get; private set; } = new[] { -3.5f, 0f, 3.5f };

        [SerializeField] private RoadSpawnPatternData[] patterns = Array.Empty<RoadSpawnPatternData>();

        public RoadSpawnPatternData[] Patterns => patterns ?? Array.Empty<RoadSpawnPatternData>();
    }
}