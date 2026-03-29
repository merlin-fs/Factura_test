using Game.Client.Views;
using UnityEngine;

namespace Game.Client.Config
{
    [CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Game/Configs/Projectile Config")]
    public sealed class ProjectileConfig : ScriptableObject
    {
        [field: SerializeField]
        public ProjectileView Prefab { get; private set; }

        [field: SerializeField, Min(0f)]
        public float Speed { get; private set; } = 12f;

        [field: SerializeField, Min(0f)]
        public float HitRadius { get; private set; } = 0.15f;

        [field: SerializeField, Min(0f)]
        public float Lifetime { get; private set; } = 2f;

        [field: SerializeField, Min(1)]
        public int Damage { get; private set; } = 10;
    }
}