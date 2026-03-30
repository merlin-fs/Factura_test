using Game.Client.Config;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Client.Common
{
    /// <summary>
    /// Централізований контейнер посилань на об'єкти сцени Gameplay.
    /// Використовується <see cref="Game.Client.Session.GameSessionBuilder"/> і DI-контейнером
    /// для отримання посилань без пошуку через <c>FindObjectOfType</c>.
    /// </summary>
    public sealed class GameplaySceneRefs : MonoBehaviour
    {
        [Header("World")]
        [SerializeField] private Transform unitsRoot;
        
        [Header("Car")]
        public PlayerConfig PlayerConfig;
        [SerializeField] private Transform carTransform;
        [SerializeField] private Transform turretTransform;
        [SerializeField] private float carSpeed = 8f;
        [SerializeField] private int carMaxHp = 100;

        [Header("Turret")]
        [SerializeField] private InputActionReference horizontalDragAction;
        [SerializeField] private InputActionReference fireAction;

        [Header("Projectile")]
        [SerializeField] private ProjectileConfig projectileConfig;

        [Header("Ground")]
        [Tooltip("Кореневий об'єкт, під яким створюються сегменти землі. X/Y визначають вирівнювання смуги.")]
        [SerializeField] private Transform groundRoot;
        [SerializeField] private GroundLooperConfig groundLooperConfig;

        [Header("Level")]
        [SerializeField] private LevelConfig levelConfig;

        [Header("Camera")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Vector3 cameraOffset = new(0f, 4f, -8f);
        [SerializeField] private Vector3 cameraAngle  = new(26f, 0f, 0f);
        [SerializeField] private float cameraFollowLerp = 10f;
        [Tooltip("Зміщення позиції камери вперед/назад вздовж дороги.\n> 0 — камера вперед → машина нижче по екрану\n< 0 — камера назад → машина вище по екрану")]
        [SerializeField] private float cameraLookOffsetForward = 5f;
        [Tooltip("Швидкість лерпу вздовж дороги (менше = більше інерції вперед/назад).")]
        [SerializeField, Min(0.1f)] private float cameraLookLerp = 5f;
        [Tooltip("Амплітуда випадкового похитування камери під час руху (у світових одиницях).")]
        [SerializeField, Min(0f)] private float cameraSwayAmplitude = 0.06f;
        [Tooltip("Швидкість зміни похитування (залежить від пройденої відстані).")]
        [SerializeField, Min(0f)] private float cameraSwaySpeed = 0.4f;

        [Header("UI")] 
        public GameObject BeginGamePanel;
        public GameObject WinPanel;
        public GameObject LosePanel;
        public GameObject HudGame;

        public Transform           GroundRoot           => groundRoot;
        public GroundLooperConfig  GroundLooperConfig   => groundLooperConfig;

        public Transform           UnitsRoot => unitsRoot;
        
        public Transform CarTransform    => carTransform;
        public Transform TurretTransform => turretTransform;

        public InputActionReference HorizontalDragAction => horizontalDragAction;
        public InputActionReference FireAction => fireAction;
        public LevelConfig LevelConfig => levelConfig;

        public Transform CameraTransform => cameraTransform;
        public Vector3   CameraOffset    => cameraOffset;
        public Quaternion CameraRotation => Quaternion.Euler(cameraAngle);
        public float CameraFollowLerp    => cameraFollowLerp;
        public float CameraLookOffsetForward => cameraLookOffsetForward;
        public float CameraLookLerp => cameraLookLerp;
        public float CameraSwayAmplitude => cameraSwayAmplitude;
        public float CameraSwaySpeed => cameraSwaySpeed;
    }
}