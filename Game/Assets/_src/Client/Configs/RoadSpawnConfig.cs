using System;
using UnityEngine;

namespace Game.Client.Config
{
    /// <summary>
    /// ScriptableObject-конфігурація системи спауну ворогів на дорозі.
    /// </summary>
    [CreateAssetMenu(fileName = "RoadSpawnConfig", menuName = "Game/Configs/Road Spawn Config")]
    public sealed class RoadSpawnConfig : ScriptableObject
    {
        /// <summary>Відстань від старту, на якій з'являється перша група ворогів.</summary>
        [field: SerializeField, Min(0f)]
        public float InitialSpawnDistance { get; private set; } = 8f;

        /// <summary>Мінімальна відстань попереду автомобіля, де спауниться наступна група.</summary>
        [field: SerializeField, Min(1f)]
        public float MinSpawnAheadDistance { get; private set; } = 18f;

        /// <summary>Максимальна відстань попереду автомобіля, де спауниться наступна група.</summary>
        [field: SerializeField, Min(1f)]
        public float MaxSpawnAheadDistance { get; private set; } = 28f;

        /// <summary>Мінімальна відстань між послідовними групами ворогів.</summary>
        [field: SerializeField, Min(1f)]
        public float MinDistanceBetweenGroups { get; private set; } = 10f;

        /// <summary>Максимальна відстань між послідовними групами ворогів.</summary>
        [field: SerializeField, Min(1f)]
        public float MaxDistanceBetweenGroups { get; private set; } = 18f;

        /// <summary>Максимальна кількість живих ворогів одночасно на рівні.</summary>
        [field: SerializeField, Min(1)]
        public int MaxAliveEnemies { get; private set; } = 15;

        /// <summary>Координата Y для позиції спауну.</summary>
        [field: SerializeField]
        public float SpawnY { get; private set; } = 0f;

        /// <summary>Зміщення по X для кожної смуги дороги.</summary>
        [field: SerializeField]
        public float[] LaneOffsetsX { get; private set; } = new[] { -3.5f, 0f, 3.5f };

        [SerializeField] private RoadSpawnPatternData[] patterns = Array.Empty<RoadSpawnPatternData>();

        /// <summary>Масив доступних паттернів спауну.</summary>
        public RoadSpawnPatternData[] Patterns => patterns ?? Array.Empty<RoadSpawnPatternData>();
    }
}