using UnityEngine;

namespace Game.Client.Config
{
    /// <summary>
    /// ScriptableObject-конфігурація турелі гравця.
    /// Визначає швидкість обертання, обмеження, кулдаун та параметри прицілювання.
    /// </summary>
    [CreateAssetMenu(fileName = "TurretConfig", menuName = "Game/Configs/Turret Config")]
    public sealed class TurretConfig : ScriptableObject
    {
        /// <summary>Швидкість горизонтального обертання турелі.</summary>
        [field: SerializeField]
        public float RotateSpeed { get; private set; } = 0.3f;

        /// <summary>Обмеження кута повороту турелі в градусах (0 — без обмежень).</summary>
        [field: SerializeField, Min(0f)]
        public float RotationLimit { get; private set; } = 60f;

        /// <summary>Кулдаун між пострілами у секундах.</summary>
        [field: SerializeField, Min(0f)]
        public float FireCooldown { get; private set; } = 0.5f;

        /// <summary>Дальність лазерного прицілу у світових одиницях.</summary>
        [field: SerializeField, Min(1f)]
        public float AimRange { get; private set; } = 80f;

        /// <summary>Радіус сфери пошуку цілей для прицілювання.</summary>
        [field: SerializeField, Min(0.05f)]
        public float AimRadius { get; private set; } = 0.6f;

        /// <summary>
        /// Якщо <c>true</c> — лазер і снаряд залипають на центрі ворога;
        /// якщо <c>false</c> — лазер прямий, снаряд Y-скоригований.
        /// </summary>
        [field: SerializeField]
        public bool LockOnTarget { get; private set; } = false;

        /// <summary>Координата Y точки прицілювання при відсутності цілі (у світовому просторі).</summary>
        [field: SerializeField]
        public float FallbackAimY { get; private set; } = 0.5f;
    }
}
