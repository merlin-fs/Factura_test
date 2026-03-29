using Game.Client.Views;
using Game.Core.Units;
using UnityEngine;

namespace Game.Client.Config
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Game/Configs/Player Config")]
    public sealed class PlayerConfig : UnitConfig
    {
        [field: SerializeField]
        public CarBaseView Prefab { get; private set; }

        [field: SerializeField, Min(0f)]
        public float MoveSpeed { get; private set; } = 4f;

        [field: SerializeField]
        public TurretConfig TurretConfig { get; private set; }
    }
}
