using Game.Core.Common;
using Game.Core.Services;
using Game.Client.Config;
using Game.Client.Views;
using Game.Core.Units;
using Game.Core.Session;
using VContainer;
using UnityEngine;

namespace Game.Client.Units
{
    public sealed class EnemyFactory : IEnemyFactory
    {
        private readonly PrefabPool<EnemyView> _pool;
        private readonly EnemyRegistry         _registry;
        private readonly GameSession           _session;
        private readonly TickSystemRegistry    _tickRegistry;
        private readonly IObjectResolver       _container;

        public EnemyFactory(
            PrefabPool<EnemyView> pool,
            EnemyRegistry         registry,
            GameSession           session,
            TickSystemRegistry    tickRegistry,
            IObjectResolver       container)
        {
            _pool         = pool;
            _registry     = registry;
            _session      = session;
            _tickRegistry = tickRegistry;
            _container    = container;
        }

        public void Spawn(UnitConfig config, Vector3 position, Quaternion rotation)
        {
            var enemyConfig = (EnemyConfig)config;
            var view        = _pool.Get(enemyConfig.Prefab, position, rotation);
            var collideLink = view.GetComponent<UnitColliderLink>();

            var agent = new EnemyUnit(
                enemyConfig, view,
                onDied: a =>
                {
                    _registry.ReportKill();
                    a.Dispose();
                    _pool.Return(view);
                    collideLink?.Unbind();
                },
                onOutOfRange: a =>
                {
                    _registry.TrackRelease();
                    a.Dispose();
                    _pool.Return(view);
                    collideLink?.Unbind();
                },
                session:           _session,
                tickSystemRegistry: _tickRegistry,
                container:          _container);

            collideLink?.Bind(agent);
            _registry.TrackSpawn();
        }
    }
}