using UnityEngine;

namespace Game.Client.Config
{
    /// <summary>
    /// ScriptableObject-конфігурація безкінечного тайлінгу сегментів землі.
    /// </summary>
    [CreateAssetMenu(fileName = "GroundLooperConfig", menuName = "Game/Configs/Ground Looper Config")]
    public sealed class GroundLooperConfig : ScriptableObject
    {
        /// <summary>Префаб одного сегмента землі.</summary>
        [field: SerializeField]
        public GameObject SegmentPrefab { get; private set; }

        /// <summary>Довжина одного сегмента вздовж осі Z.</summary>
        [field: SerializeField, Min(0.1f)]
        public float SegmentLength { get; private set; } = 20f;

        /// <summary>Кількість сегментів, що утримуються попереду автомобіля.</summary>
        [field: SerializeField, Min(1)]
        public int SegmentsAhead { get; private set; } = 5;

        /// <summary>Кількість сегментів, що утримуються позаду автомобіля.</summary>
        [field: SerializeField, Min(0)]
        public int SegmentsBehind { get; private set; } = 2;
    }
}
