using UnityEngine;

namespace Game.Client.Config
{
    [CreateAssetMenu(fileName = "TurretConfig", menuName = "Game/Configs/Turret Config")]
    public sealed class TurretConfig : ScriptableObject
    {
        [field: SerializeField]
        public float RotateSpeed { get; private set; } = 0.3f;

        [field: SerializeField, Min(0f)]
        public float RotationLimit { get; private set; } = 60f;

        [field: SerializeField, Min(0f)]
        public float FireCooldown { get; private set; } = 0.5f;

        [field: SerializeField, Min(1f)]
        public float AimRange { get; private set; } = 80f;

        [field: SerializeField, Min(0.05f)]
        public float AimRadius { get; private set; } = 0.6f;

        [field: SerializeField]
        [field: Tooltip("true — лазер і снаряд залипають на центрі ворога; false — лазер прямий, снаряд Y-скоригований")]
        public bool LockOnTarget { get; private set; } = false;
    }
}

