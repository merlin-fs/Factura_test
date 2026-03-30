using System;
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
    /// <summary>
    /// Фабрика ворожих юнітів. Керує пулом <see cref="EnemyView"/> та реєструє
    /// статистику спауну/загибелі у <see cref="GameStatistics"/>.
    /// </summary>
    public sealed class EnemyFactory : IEnemyFactory, IDisposable
    {
        private readonly PrefabPool<EnemyView> _pool;
        private readonly GameStatistics _registry;
        private readonly GameSession _session;
        private readonly TickSystemRegistry _tickRegistry;
        private readonly IObjectResolver _container;
        private readonly DamageService _damageService;
    
        public EnemyFactory(
            Func<EnemyView, Transform, EnemyView> factoryMethod,
            Transform parent,
            GameStatistics registry,
            GameSession session,
            TickSystemRegistry tickRegistry,
            DamageService damageService,
            IObjectResolver container)
        {
            _pool = new PrefabPool<EnemyView>(factoryMethod, parent);
            _registry = registry;
            _session = session;
            _tickRegistry = tickRegistry;
            _damageService = damageService;
            _container = container;
        }

        public void Spawn(UnitConfig config, Vector3 position, Quaternion rotation)
        {
            var enemyConfig = (EnemyConfig)config;
            var view = _pool.Get(enemyConfig.Prefab, position, rotation);
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
                session: _session,
                tickSystemRegistry: _tickRegistry,
                damageService: _damageService,
                container: _container);

            collideLink?.Bind(agent);
            _registry.TrackSpawn();
        }

        public void Dispose()
        {
            _pool?.Dispose();
        }
    }
}