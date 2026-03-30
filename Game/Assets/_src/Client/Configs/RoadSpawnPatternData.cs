using System;
using UnityEngine;

namespace Game.Client.Config
{
    /// <summary>
    /// Паттерн спауну групи ворогів. Має вагу для зваженого випадкового вибору.
    /// </summary>
    [Serializable]
    public sealed class RoadSpawnPatternData
    {
        /// <summary>Вага паттерну при випадковому виборі. Більше значення — вища ймовірність.</summary>
        [SerializeField, Min(1)] private int weight = 1;
        /// <summary>Масив записів ворогів у цьому паттерні.</summary>
        [SerializeField] private RoadSpawnEntryData[] entries = Array.Empty<RoadSpawnEntryData>();

        /// <summary>Вага паттерну.</summary>
        public int Weight => weight;
        /// <summary>Записи ворогів паттерну.</summary>
        public RoadSpawnEntryData[] Entries => entries ?? Array.Empty<RoadSpawnEntryData>();
    }
}