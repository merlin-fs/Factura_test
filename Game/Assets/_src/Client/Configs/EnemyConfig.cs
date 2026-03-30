using Game.Client.Views;
using Game.Core.Units;
using UnityEngine;

namespace Game.Client.Config
{
    /// <summary>
    /// ScriptableObject-конфігурація ворожого юніта.
    /// Визначає параметри руху, агресії, атаки та блукання.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Game/Configs/Enemy Config")]
    public sealed class EnemyConfig : UnitConfig
    {
        /// <summary>Префаб вигляду ворога.</summary>
        [field: SerializeField]
        public EnemyView Prefab { get; private set; }

        /// <summary>Швидкість руху під час переслідування гравця.</summary>
        [field: SerializeField, Min(0f)]
        public float MoveSpeed { get; private set; } = 4f;

        /// <summary>Радіус, у якому ворог виявляє гравця та переходить у Chase.</summary>
        [field: SerializeField, Min(0f)]
        public float AggroRadius { get; private set; } = 15f;

        /// <summary>Інтервал між атаками у секундах.</summary>
        [field: SerializeField, Min(0f)]
        public float AttackInterval { get; private set; } = 1f;

        /// <summary>Радіус влучання атаки ближнього бою.</summary>
        [field: SerializeField, Min(0f)]
        public float HitRadius { get; private set; } = 0.5f;

        [Header("Wander (Zombie Idle)")]
        /// <summary>Швидкість руху під час блукання.</summary>
        [field: SerializeField, Min(0f)]
        public float WanderSpeed { get; private set; } = 1.2f;

        /// <summary>Мінімальна тривалість одного циклу блукання.</summary>
        [field: SerializeField, Min(0f)]
        public float WanderMinDuration { get; private set; } = 2f;

        /// <summary>Максимальна тривалість одного циклу блукання.</summary>
        [field: SerializeField, Min(0f)]
        public float WanderMaxDuration { get; private set; } = 5f;

        /// <summary>Мінімальна тривалість простою між циклами блукання.</summary>
        [field: SerializeField, Min(0f)]
        public float WanderIdleMinDuration { get; private set; } = 1f;

        /// <summary>Максимальна тривалість простою між циклами блукання.</summary>
        [field: SerializeField, Min(0f)]
        public float WanderIdleMaxDuration { get; private set; } = 3f;
    }
}
