using Game.Client.Config;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


namespace Game.Client.Bootstrap
{
    public sealed class GameplaySceneRefs : MonoBehaviour
    {
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
        [Tooltip("Корневой объект под которым создаются сегменты земли. X/Y определяют выравнивание полосы.")]
        [SerializeField] private Transform groundRoot;
        [SerializeField] private GroundLooperConfig groundLooperConfig;

        [Header("Level")]
        [SerializeField] private LevelConfig levelConfig;

        [Header("Camera")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Vector3 cameraOffset = new(0f, 4f, -8f);
        [SerializeField] private Vector3 cameraAngle  = new(26f, 0f, 0f);
        [SerializeField] private float cameraFollowLerp = 10f;
        [Tooltip("Смещение позиции камеры вперёд/назад вдоль дороги.\n> 0 — камера вперёд → машина ниже по экрану\n< 0 — камера назад → машина выше по экрану")]
        [SerializeField] private float cameraLookOffsetForward = 5f;
        [Tooltip("Скорость лерпа вдоль дороги (меньше = больше инерции вперёд/назад).")]
        [SerializeField, Min(0.1f)] private float cameraLookLerp = 5f;
        [Tooltip("Амплитуда случайного покачивания камеры во время движения (world units).")]
        [SerializeField, Min(0f)] private float cameraSwayAmplitude = 0.06f;
        [Tooltip("Скорость смены покачивания (зависит от пройденного расстояния).")]
        [SerializeField, Min(0f)] private float cameraSwaySpeed = 0.4f;

        [Header("UI")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;
        [SerializeField] private Image hpBarFill;

        public Transform           GroundRoot           => groundRoot;
        public GroundLooperConfig  GroundLooperConfig   => groundLooperConfig;

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

        public GameObject WinPanel => winPanel;
        public GameObject LosePanel => losePanel;
        public Image HpBarFill => hpBarFill;
    }
}