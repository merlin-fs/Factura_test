using Game.Client.Views;
using Game.Core.Units;
using UnityEngine;

namespace Game.Client.Config
{
    /// <summary>
    /// ScriptableObject-конфігурація юніта гравця.
    /// Визначає префаб, швидкість і конфігурацію турелі.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Game/Configs/Player Config")]
    public sealed class PlayerConfig : UnitConfig
    {
        /// <summary>Префаб вигляду автомобіля гравця.</summary>
        [field: SerializeField]
        public CarView Prefab { get; private set; }

        /// <summary>Швидкість руху автомобіля вздовж осі Z.</summary>
        [field: SerializeField, Min(0f)]
        public float MoveSpeed { get; private set; } = 4f;

        /// <summary>Конфігурація турелі гравця.</summary>
        [field: SerializeField]
        public TurretConfig TurretConfig { get; private set; }
    }
}
