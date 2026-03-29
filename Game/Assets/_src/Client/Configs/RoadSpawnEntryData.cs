using System;
using UnityEngine;

namespace Game.Client.Config
{
    [Serializable]
    public struct RoadSpawnEntryData
    {
        [SerializeField] private EnemyConfig enemyConfig;

        [SerializeField] private bool useRandomLane;
        [SerializeField, Min(0)] private int laneIndex;

        [SerializeField] private float lateralOffset;
        [SerializeField, Min(0f)] private float lateralJitter;

        [SerializeField] private float forwardOffset;
        [SerializeField, Min(0f)] private float forwardJitter;

        public EnemyConfig EnemyConfig => enemyConfig;
        public bool UseRandomLane => useRandomLane;
        public int LaneIndex => laneIndex;

        public float LateralOffset => lateralOffset;
        public float LateralJitter => lateralJitter;

        public float ForwardOffset => forwardOffset;
        public float ForwardJitter => forwardJitter;
    }
}