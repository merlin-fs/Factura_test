using System;
using Game.Client.Config;
using Game.Client.Input;
using Game.Client.Views;
using Game.Core.Common;
using Game.Core.Services;
using Game.Core.Session;
using Game.Core.Units;
using UnityEngine;

namespace Game.Client.Units
{
    /// <summary>
    /// Контролер турелі гравця на чистому C#. Зареєстрований у <see cref="TickSystemRegistry"/>.
    /// Обробляє горизонтальне обертання та безперервну стрільбу, поки утримується кнопка «Вогонь».
    /// Обчислення прицілювання та лазерний приціл делеговані <see cref="TurretAimResolver"/>.
    /// </summary>
    public sealed class Turret : ITickSystem, IDisposable
    {
        private readonly TurretView            _view;
        private readonly TurretConfig          _config;
        private readonly IHorizontalDragInput  _dragInput;
        private readonly IFireInput            _fireInput;
        private readonly GameSession           _session;
        private readonly TickSystemRegistry    _tickRegistry;
        private readonly ProjectileAttackSkill _attackSkill;
        private readonly TurretAimResolver     _aimResolver;
        private readonly float                 _initialYaw;

        private float _fireTimer;

        /// <summary>
        /// Створює турель та реєструє її для тікання.
        /// </summary>
        public Turret(
            Unit                 playerUnit,
            TurretView           view,
            TurretConfig         config,
            IHorizontalDragInput dragInput,
            IFireInput           fireInput,
            GameSession          session,
            TickSystemRegistry   tickRegistry,
            ITargetsProvider     targetsProvider)
        {
            _view          = view;
            _config        = config;
            _dragInput     = dragInput;
            _fireInput     = fireInput;
            _session       = session;
            _tickRegistry  = tickRegistry;
            _fireTimer     = 0f;
            _initialYaw    = view.transform.eulerAngles.y;
            _aimResolver   = new TurretAimResolver(view, config, playerUnit, targetsProvider);
            _attackSkill   = session.Player.Skills.Has<ProjectileAttackSkill>()
                ? session.Player.Skills.Get<ProjectileAttackSkill>()
                : null;
            _tickRegistry.Register(this);
        }

        /// <summary>
        /// Скасовує реєстрацію турелі з реєстру систем.
        /// </summary>
        public void Dispose()
        {
            _tickRegistry.Unregister(this);
        }

        /// <inheritdoc/>
        public void Tick(float dt)
        {
            _aimResolver.Update();

            if (_session.IsPaused) return;
            if (!_fireInput.IsPressed()) return;

            var dx = _dragInput.ReadDeltaX();
            if (Mathf.Abs(dx) > 0.0001f)
            {
                _view.transform.Rotate(0f, dx * _config.RotateSpeed * dt, 0f, Space.World);
                if (_config.RotationLimit > 0f)
                {
                    var delta = Mathf.DeltaAngle(_initialYaw, _view.transform.eulerAngles.y);
                    delta = Mathf.Clamp(delta, -_config.RotationLimit, _config.RotationLimit);
                    var euler = _view.transform.eulerAngles;
                    euler.y = _initialYaw + delta;
                    _view.transform.eulerAngles = euler;
                }
            }

            _fireTimer -= dt;
            if (_fireTimer <= 0f)
            {
                _fireTimer = _config.FireCooldown;
                var context = new AttackContext(_session.Player, _view.Muzzle.position,
                    _aimResolver.FireDirection, _session.Player.TargetMask, null);
                _attackSkill?.Use(context);
            }
        }
    }
}
