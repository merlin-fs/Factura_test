using System;
using Game.Client.Config;
using Game.Client.Views;
using Game.Core.Common;
using Game.Core.Services;
using Game.Core.Units;
using UnityEngine;

namespace Game.Client.Units
{
    /// <summary>
    /// Снаряд на чистому C#. Рухається до цілі (або прямолінійно),
    /// перевіряє зіткнення через <see cref="HitService"/> і завдає шкоди при влучанні.
    /// Автоматично повертається до пулу при закінченні часу існування або влучанні.
    /// </summary>
    public sealed class Projectile : ITickSystem, IHitHandler
    {
        private readonly ProjectileConfig    _config;
        private readonly ProjectileView      _view;
        private readonly Action<Projectile>  _onExpire;
        private readonly LayerMask           _hitMask;
        private readonly DamageService       _damageService;
        private readonly HitService          _hitService;

        private Unit    _source;
        private Unit    _target;
        private Vector3 _direction;
        private float   _lifeRemaining;

        /// <summary>
        /// Створює снаряд.
        /// </summary>
        public Projectile(
            ProjectileConfig    config,
            ProjectileView      view,
            LayerMask           hitMask,
            Action<Projectile>  onExpire,
            DamageService       damageService,
            HitService          hitService)
        {
            _config        = config;
            _view          = view;
            _hitMask       = hitMask;
            _onExpire      = onExpire;
            _damageService = damageService;
            _hitService    = hitService;
        }

        /// <summary>
        /// Активує снаряд: встановлює джерело, ціль, напрямок і час існування.
        /// </summary>
        public void Activate(Unit source, Unit target, Vector3 direction)
        {
            _source        = source;
            _target        = target;
            _direction     = direction.normalized;
            _lifeRemaining = _config.Lifetime;

            _view.gameObject.SetActive(true);
        }

        /// <inheritdoc/>
        public void Tick(float dt)
        {
            _lifeRemaining -= dt;
            if (_lifeRemaining <= 0f)
            {
                Despawn();
                return;
            }

            if (_target != null)
            {
                var toTarget = _target.Position - _view.transform.position;
                if (toTarget.sqrMagnitude > 0.0001f)
                    _direction = toTarget.normalized;
            }

            var origin   = _view.transform.position;
            var distance = _config.Speed * dt;
            var query    = new HitQuery(
                HitQueryType.SphereCastXZ,
                _source,
                origin,
                _direction,
                distance,
                _config.HitRadius,
                _hitMask,
                1);

            if (_hitService.ProcessFirst(query, this))
            {
                Despawn();
                return;
            }

            var newPosition = origin + _direction * distance;
            _view.transform.SetPositionAndRotation(newPosition, Quaternion.LookRotation(_direction));
        }

        /// <inheritdoc/>
        public void Handle(Unit source, Unit target)
        {
            _damageService.Apply(new DamageRequest(source, target, _config.Damage, DamageSource.Projectile));
        }

        private void Despawn()
        {
            _view.gameObject.SetActive(false);
            _onExpire?.Invoke(this);
        }

    }
}