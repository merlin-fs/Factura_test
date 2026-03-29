using Game.Core.Common;
using Game.Client.Config;
using Game.Client.Views;
using Game.Core.Services;
using UnityEngine;

namespace Game.Client.Units
{
    public sealed class ProjectileFactory
    {
        private readonly PrefabPool<ProjectileView> _pool;
        private readonly TickSystemRegistry         _tickRegistry;
        private readonly DamageService              _damageService;
        private readonly HitService                 _hitService;

        public ProjectileFactory(
            PrefabPool<ProjectileView> pool,
            TickSystemRegistry         tickRegistry,
            DamageService              damageService,
            HitService                 hitService)
        {
            _pool          = pool;
            _tickRegistry  = tickRegistry;
            _damageService = damageService;
            _hitService    = hitService;
        }

        public Projectile Spawn(ProjectileConfig projectileConfig, LayerMask hitMask, Vector3 origin, Vector3 direction)
        {
            var view       = _pool.Get(projectileConfig.Prefab, origin, Quaternion.LookRotation(direction));
            var projectile = new Projectile(
                projectileConfig,
                view,
                hitMask,
                onExpire: p =>
                {
                    _tickRegistry.Unregister(p);
                    _pool.Return(view);
                },
                _damageService,
                _hitService);

            _tickRegistry.Register(projectile);
            return projectile;
        }
    }
}