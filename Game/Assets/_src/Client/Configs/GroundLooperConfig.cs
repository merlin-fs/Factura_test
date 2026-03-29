using UnityEngine;

namespace Game.Client.Config
{
    [CreateAssetMenu(fileName = "GroundLooperConfig", menuName = "Game/Configs/Ground Looper Config")]
    public sealed class GroundLooperConfig : ScriptableObject
    {
        [Tooltip("Префаб одного сегмента земли.")]
        [field: SerializeField]
        public GameObject SegmentPrefab { get; private set; }

        [Tooltip("Длина одного сегмента по оси Z.")]
        [field: SerializeField, Min(0.1f)]
        public float SegmentLength { get; private set; } = 20f;

        [Tooltip("Сколько сегментов держать впереди машины.")]
        [field: SerializeField, Min(1)]
        public int SegmentsAhead { get; private set; } = 5;

        [Tooltip("Сколько сегментов держать позади машины.")]
        [field: SerializeField, Min(0)]
        public int SegmentsBehind { get; private set; } = 2;
    }
}

