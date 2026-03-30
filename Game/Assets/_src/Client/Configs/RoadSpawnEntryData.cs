using System;
using UnityEngine;

namespace Game.Client.Config
{
    /// <summary>
    /// Дані одного запису ворога в паттерні спауну.
    /// Визначає конкретного ворога, його смугу та зміщення у групі.
    /// </summary>
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

        /// <summary>Конфігурація ворожого юніта для спауну.</summary>
        public EnemyConfig EnemyConfig => enemyConfig;
        /// <summary>Якщо <c>true</c> — смуга обирається випадково, інакше використовується <see cref="LaneIndex"/>.</summary>
        public bool UseRandomLane => useRandomLane;
        /// <summary>Індекс смуги для спауну (якщо <see cref="UseRandomLane"/> = false).</summary>
        public int LaneIndex => laneIndex;
        /// <summary>Бічне зміщення від центру смуги.</summary>
        public float LateralOffset => lateralOffset;
        /// <summary>Випадковий розкид бічного зміщення.</summary>
        public float LateralJitter => lateralJitter;
        /// <summary>Зміщення вздовж дороги відносно бази групи.</summary>
        public float ForwardOffset => forwardOffset;
        /// <summary>Випадковий розкид зміщення вздовж дороги.</summary>
        public float ForwardJitter => forwardJitter;
    }
}