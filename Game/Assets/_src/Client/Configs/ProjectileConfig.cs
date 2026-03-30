using Game.Client.Views;
using UnityEngine;

namespace Game.Client.Config
{
    /// <summary>
    /// ScriptableObject-конфігурація снаряду.
    /// Визначає префаб, фізичні параметри та шкоду.
    /// </summary>
    [CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Game/Configs/Projectile Config")]
    public sealed class ProjectileConfig : ScriptableObject
    {
        /// <summary>Префаб вигляду снаряду.</summary>
        [field: SerializeField]
        public ProjectileView Prefab { get; private set; }

        /// <summary>Швидкість польоту снаряду у світових одиницях за секунду.</summary>
        [field: SerializeField, Min(0f)]
        public float Speed { get; private set; } = 12f;

        /// <summary>Радіус сфери перевірки влучання.</summary>
        [field: SerializeField, Min(0f)]
        public float HitRadius { get; private set; } = 0.15f;

        /// <summary>Час існування снаряду у секундах.</summary>
        [field: SerializeField, Min(0f)]
        public float Lifetime { get; private set; } = 2f;

        /// <summary>Шкода при влучанні.</summary>
        [field: SerializeField, Min(1)]
        public int Damage { get; private set; } = 10;
    }
}