using System;
using Game.Client.Config;
using Game.Client.Input;
using Game.Client.Views;
using Game.Core.Common;
using Game.Core.Session;
using Game.Core.Units;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Client.Units
{
    /// <summary>
    /// Чистий C# клас (не MonoBehaviour). Реєструється у TickSystemRegistry.
    /// Обертання башти (перетягування під час утримання) + безперервна стрільба (утримання).
    /// Обертання обмежено ±TurretConfig.RotationLimit градусів від початкового напрямку.
    /// Лазерний приціл: завжди видимий, червоний — при захопленні цілі (з корекцією Y), жовтий — при пошуку.
    /// </summary>
    public sealed class Turret : ITickSystem, IDisposable
    {
        private readonly TurretView            _view;
        private readonly TurretConfig          _config;
        private readonly IHorizontalDragInput  _dragInput;
        private readonly IFireInput            _fireInput;
        private readonly GameSession           _session;
        private readonly Unit                  _playerUnit;
        private readonly TickSystemRegistry    _tickRegistry;
        private readonly ProjectileAttackSkill _attackSkill;

        private float _fireTimer;
        private float _initialYaw;

        // --- Лазерний приціл ---
        private LineRenderer          _aimLine;
        private readonly RaycastHit[] _aimBuffer = new RaycastHit[1];

        // Кешований результат прицілювання за кадр (уникає подвійного SphereCast у кадрі пострілу)
        private Vector3 _aimPoint;      // кінець лазера (візуальний)
        private Vector3 _fireDirection; // напрямок снаряда (може відрізнятись від лазера при LockOnTarget == false)
        private bool    _hasAimTarget;

        private static readonly Color ColorOnTarget  = new Color(1f, 0.15f, 0.15f, 1f);
        private static readonly Color ColorSearching = new Color(1f, 0.85f, 0f,   0.7f);

        public Turret(
            Unit                 playerUnit,
            TurretView           view,
            TurretConfig         config,
            IHorizontalDragInput dragInput,
            IFireInput           fireInput,
            GameSession          session,
            TickSystemRegistry   tickRegistry)
        {
            _view         = view;
            _config       = config;
            _dragInput    = dragInput;
            _fireInput    = fireInput;
            _session      = session;
            _playerUnit   = playerUnit;
            _tickRegistry = tickRegistry;
            _fireTimer    = 0f;
            _initialYaw   = view.transform.eulerAngles.y;

            _attackSkill = session.Player.Skills.Has<ProjectileAttackSkill>()
                ? session.Player.Skills.Get<ProjectileAttackSkill>()
                : null;

            SetupAimLine();
            _tickRegistry.Register(this);
        }

        public void Dispose()
        {
            _tickRegistry.Unregister(this);
        }

        // ---------------------------------------------------------------
        //  Лазерний приціл
        // ---------------------------------------------------------------

        private void SetupAimLine()
        {
            _aimLine = _view.gameObject.GetComponent<LineRenderer>();
            if (_aimLine == null)
                _aimLine = _view.gameObject.AddComponent<LineRenderer>();

            _aimLine.positionCount     = 2;
            _aimLine.useWorldSpace     = true;
            _aimLine.startWidth        = 0.06f;
            _aimLine.endWidth          = 0.02f;
            _aimLine.shadowCastingMode = ShadowCastingMode.Off;
            _aimLine.receiveShadows    = false;
            _aimLine.textureMode       = LineTextureMode.Tile;

            // Запасний unlit-матеріал — у Інспекторі можна призначити правильний URP-матеріал
            if (_aimLine.sharedMaterial == null || _aimLine.sharedMaterial.name == "Default-Line")
                _aimLine.material = new Material(Shader.Find("Sprites/Default"));
        }

        /// <summary>
        /// SphereCast у площині XZ від дула.
        /// Якщо потрапляємо у колайдер ворога — <see cref="_fireDirection"/> коригується з урахуванням різниці по Y.
        /// <para>
        /// <see cref="TurretConfig.LockOnTarget"/> == true : лазер і снаряд "залипають" на центрі ворога.<br/>
        /// <see cref="TurretConfig.LockOnTarget"/> == false: лазер завжди прямий (XZ), снаряд все одно влучає (Y-корекція прихована).
        /// </para>
        /// </summary>
        private void ComputeAim()
        {
            var muzzlePos   = _view.Muzzle.position;
            var flatForward = new Vector3(_view.Muzzle.forward.x, 0f, _view.Muzzle.forward.z);

            if (flatForward.sqrMagnitude < 0.0001f)
            {
                _aimPoint      = muzzlePos + _view.Muzzle.forward * _config.AimRange;
                _fireDirection = _view.Muzzle.forward;
                _hasAimTarget  = false;
                return;
            }

            flatForward.Normalize();

            // Кастуємо у площині XZ незалежно від висоти дула
            var flatOrigin = new Vector3(muzzlePos.x, 0f, muzzlePos.z);

            var hitCount = Physics.SphereCastNonAlloc(
                flatOrigin, _config.AimRadius, flatForward,
                _aimBuffer, _config.AimRange, _playerUnit.TargetMask);

            if (hitCount > 0)
            {
                var enemyCenter = _aimBuffer[0].collider.bounds.center;

                // Лазер залишається прямим (false) або залипає на цілі (true)
                _aimPoint = _config.LockOnTarget
                    ? enemyCenter
                    : muzzlePos + flatForward * _config.AimRange;

                // Напрямок снаряда завжди Y-скоригований: влучаємо в центр ворога
                _fireDirection = (enemyCenter - muzzlePos).normalized;
                _hasAimTarget  = true;
            }
            else
            {
                _aimPoint      = muzzlePos + flatForward * _config.AimRange;
                _fireDirection = flatForward;
                _hasAimTarget  = false;
            }
        }

        private void UpdateAimLine()
        {
            _aimLine.enabled = true;
            _aimLine.SetPosition(0, _view.Muzzle.position);
            _aimLine.SetPosition(1, _aimPoint);

            var color = _hasAimTarget ? ColorOnTarget : ColorSearching;
            _aimLine.startColor = color;
            _aimLine.endColor   = new Color(color.r, color.g, color.b, 0f); // затухає до прозорого
        }

        // ---------------------------------------------------------------
        //  ITickSystem
        // ---------------------------------------------------------------

        public void Tick(float dt)
        {
            // Оновлюємо приціл та лазер щотіку (навіть коли пауза — для коректного відображення)
            ComputeAim();
            UpdateAimLine();

            if (_session.IsPaused) return;
            if (!_fireInput.IsPressed()) return;

            // --- Обертання башти (лише під час утримання) ---
            var dx = _dragInput.ReadDeltaX();
            if (Mathf.Abs(dx) > 0.0001f)
            {
                _view.transform.Rotate(0f, dx * _config.RotateSpeed * dt, 0f, Space.World);

                // Обмежуємо обертання в межах ±RotationLimit градусів від початкового напрямку
                if (_config.RotationLimit > 0f)
                {
                    var delta = Mathf.DeltaAngle(_initialYaw, _view.transform.eulerAngles.y);
                    delta = Mathf.Clamp(delta, -_config.RotationLimit, _config.RotationLimit);
                    var euler = _view.transform.eulerAngles;
                    euler.y = _initialYaw + delta;
                    _view.transform.eulerAngles = euler;
                }
            }

            // --- Безперервна стрільба з кулдауном ---
            _fireTimer -= dt;
            if (_fireTimer <= 0f)
            {
                _fireTimer = _config.FireCooldown;

                // _fireDirection вже враховує зміщення по Y до центру ворога (з ComputeAim)
                var context = new AttackContext(
                    _playerUnit, _view.Muzzle.position, _fireDirection, _playerUnit.TargetMask, null);
                _attackSkill?.Use(context);
            }
        }
    }
}
