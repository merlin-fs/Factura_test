using System;
using UnityEngine;

namespace Game.Client.Config
{
    [Serializable]
    public sealed class RoadSpawnPatternData
    {
        [SerializeField, Min(1)] private int weight = 1;
        [SerializeField] private RoadSpawnEntryData[] entries = Array.Empty<RoadSpawnEntryData>();

        public int Weight => weight;
        public RoadSpawnEntryData[] Entries => entries ?? Array.Empty<RoadSpawnEntryData>();
    }
}