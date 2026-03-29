using Game.Core.Common;
using Game.Client.Config;
using Game.Client.Views;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

namespace Game.Client.Units
{
    public sealed class ProjectileFactory
    {
        private PrefabPool<ProjectileView> _pool;
        private TickSystemRegistry _tickRegistry;
        private Container _container;

        [Inject]
        private void Inject(Container container, PrefabPool<ProjectileView> pool, TickSystemRegistry tickRegistry)
        {
            _container     = container;
            _pool          = pool;
            _tickRegistry  = tickRegistry;
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
                });
            AttributeInjector.Inject(projectile, _container);
            _tickRegistry.Register(projectile);
            return projectile;
        }
    }
}