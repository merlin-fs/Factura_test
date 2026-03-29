using Game.Client.Views;
using Game.Core.Units;
using UnityEngine;

namespace Game.Client.Config
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Game/Configs/Enemy Config")]
    public sealed class EnemyConfig : UnitConfig
    {
        [field: SerializeField]
        public EnemyView Prefab { get; private set; }

        [field: SerializeField, Min(0f)]
        public float MoveSpeed { get; private set; } = 4f;

        [field: SerializeField, Min(0f)]
        public float AggroRadius { get; private set; } = 15f;

        [field: SerializeField, Min(0f)]
        public float AttackInterval { get; private set; } = 1f;

        [field: SerializeField, Min(0f)]
        public float HitRadius { get; private set; } = 0.5f;

        [Header("Wander (Zombie Idle)")]
        [field: SerializeField, Min(0f)]
        public float WanderSpeed { get; private set; } = 1.2f;

        [field: SerializeField, Min(0f)]
        public float WanderMinDuration { get; private set; } = 2f;

        [field: SerializeField, Min(0f)]
        public float WanderMaxDuration { get; private set; } = 5f;

        [field: SerializeField, Min(0f)]
        public float WanderIdleMinDuration { get; private set; } = 1f;

        [field: SerializeField, Min(0f)]
        public float WanderIdleMaxDuration { get; private set; } = 3f;
    }
}

