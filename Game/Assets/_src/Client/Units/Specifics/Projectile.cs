using System;
using Game.Client.Config;
using Game.Client.Views;
using Game.Core.Common;
using Game.Core.Services;
using Game.Core.Units;
using UnityEngine;

namespace Game.Client.Units
{
    public sealed class Projectile : ITickSystem, IHitHandler
    {
        private readonly ProjectileConfig _config;
        private readonly ProjectileBaseView _baseView;
        private readonly Action<Projectile> _onExpire;
        private readonly LayerMask _hitMask;
        private readonly DamageService _damageService;
        private readonly HitService _hitService;

        private Unit _source;
        private Unit _target;
        private Vector3 _direction;
        private float _lifeRemaining;

        public Projectile(
            ProjectileConfig config,
            ProjectileBaseView baseView,
            LayerMask hitMask,
            Action<Projectile> onExpire,
            DamageService damageService,
            HitService hitService)
        {
            _config = config;
            _baseView = baseView;
            _hitMask = hitMask;
            _onExpire = onExpire;
            _damageService = damageService;
            _hitService = hitService;
        }

        public void Activate(Unit source, Unit target, Vector3 direction)
        {
            _source = source;
            _target = target;
            _direction = direction.normalized;
            _lifeRemaining = _config.Lifetime;

            _baseView.gameObject.SetActive(true);
        }

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
                var toTarget = _target.Position - _baseView.transform.position;
                if (toTarget.sqrMagnitude > 0.0001f)
                    _direction = toTarget.normalized;
            }

            var origin = _baseView.transform.position;
            var distance = _config.Speed * dt;
            var query = new HitQuery(
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

            _baseView.transform.position = origin + _direction * distance;
        }

        public void Handle(Unit source, Unit target)
        {
            _damageService.Apply(new DamageRequest(source, target, _config.Damage));
        }

        private void Despawn()
        {
            _baseView.gameObject.SetActive(false);
            _onExpire?.Invoke(this);
        }

    }
}