using System;
using Game.Client.Config;
using Game.Client.Input;
using Game.Client.Views;
using Game.Core.Common;
using Game.Core.Session;
using Game.Core.Units;
using UnityEngine;

namespace Game.Client.Units
{
    /// <summary>
    /// Pure C# turret controller. Registered in TickSystemRegistry.
    /// Handles yaw rotation and continuous fire while Fire input is held.
    /// Aim computation and laser sight are delegated to TurretAimResolver.
    /// </summary>
    public sealed class Turret : ITickSystem, IDisposable
    {
        private readonly TurretBaseView            _baseView;
        private readonly TurretConfig          _config;
        private readonly IHorizontalDragInput  _dragInput;
        private readonly IFireInput            _fireInput;
        private readonly GameSession           _session;
        private readonly TickSystemRegistry    _tickRegistry;
        private readonly ProjectileAttackSkill _attackSkill;
        private readonly TurretAimResolver     _aimResolver;
        private float _fireTimer;
        private float _initialYaw;
        public Turret(
            Unit                 playerUnit,
            TurretBaseView           baseView,
            TurretConfig         config,
            IHorizontalDragInput dragInput,
            IFireInput           fireInput,
            GameSession          session,
            TickSystemRegistry   tickRegistry)
        {
            _baseView          = baseView;
            _config        = config;
            _dragInput     = dragInput;
            _fireInput     = fireInput;
            _session       = session;
            _tickRegistry  = tickRegistry;
            _fireTimer     = 0f;
            _initialYaw    = baseView.transform.eulerAngles.y;
            _aimResolver = new TurretAimResolver(baseView, config, playerUnit);
            _attackSkill = session.Player.Skills.Has<ProjectileAttackSkill>()
                ? session.Player.Skills.Get<ProjectileAttackSkill>()
                : null;
            _tickRegistry.Register(this);
        }
        public void Dispose()
        {
            _tickRegistry.Unregister(this);
        }
        public void Tick(float dt)
        {
            // Aim + laser: update every frame, even while paused
            _aimResolver.Update();
            if (_session.IsPaused) return;
            if (!_fireInput.IsPressed()) return;
            // --- Yaw rotation (only while Fire is held) ---
            var dx = _dragInput.ReadDeltaX();
            if (Mathf.Abs(dx) > 0.0001f)
            {
                _baseView.transform.Rotate(0f, dx * _config.RotateSpeed * dt, 0f, Space.World);
                if (_config.RotationLimit > 0f)
                {
                    var delta = Mathf.DeltaAngle(_initialYaw, _baseView.transform.eulerAngles.y);
                    delta = Mathf.Clamp(delta, -_config.RotationLimit, _config.RotationLimit);
                    var euler = _baseView.transform.eulerAngles;
                    euler.y = _initialYaw + delta;
                    _baseView.transform.eulerAngles = euler;
                }
            }
            // --- Continuous fire with cooldown ---
            _fireTimer -= dt;
            if (_fireTimer <= 0f)
            {
                _fireTimer = _config.FireCooldown;
                var context = new AttackContext(
                    _session.Player, _baseView.Muzzle.position,
                    _aimResolver.FireDirection, _session.Player.TargetMask, null);
                _attackSkill?.Use(context);
            }
        }
    }
}
