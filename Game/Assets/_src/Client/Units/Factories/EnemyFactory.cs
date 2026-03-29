using Game.Core.Common;
using Game.Core.Services;
using Game.Client.Config;
using Game.Client.Views;
using Game.Core.Units;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

namespace Game.Client.Units
{
    public sealed class EnemyFactory : IEnemyFactory
    {
        private PrefabPool<EnemyView>   _pool;
        private EnemyRegistry           _registry;
        private Container               _container;

        [Inject]
        private void Inject(
            PrefabPool<EnemyView>  pool,
            EnemyRegistry          registry,
            IUnitRegistry          unitRegistry,
            Container              container)
        {
            _registry      = registry;
            _pool          = pool;
            _container     = container;
        }

        public void Spawn(UnitConfig config, Vector3 position, Quaternion rotation)
        {
            var enemyConfig = (EnemyConfig)config;
            var view   = _pool.Get(enemyConfig.Prefab, position, rotation);
            var collideLink = view.GetComponent<UnitColliderLink>();

            var agent = new EnemyUnit(enemyConfig, view,
                onDied: a =>
                {
                    _registry.ReportKill(); 
                    a.Dispose(); 
                    _pool.Return(view);
                    collideLink?.Unbind();
                },
                onOutOfRange: a =>
                {
                    a.Dispose(); 
                    _pool.Return(view);
                    collideLink?.Unbind();
                });
            
           collideLink?.Bind(agent);
           AttributeInjector.Inject(agent, _container);
        }
    }
}