using System;
using Game.Client.Common;
using Game.Core.Common;
using Game.Core.Services;
using Game.Core.Session;
using Game.Core.Units;
using Game.Client.DI;
using Game.Client.Input;
using Game.Client.Services;
using Game.Client.Units;
using Game.Client.Views;
using Game.Core;
using VContainer;
using UnityEngine;

namespace Game.Client.Session
{
    /// <summary>
    /// Кореневий будівник однієї ігрової сесії.
    /// Створює дочірній VContainer-скоуп з усіма сервісами сесії
    /// та повертає готовий до тікання <see cref="ISessionRunner"/>.
    /// </summary>
    public sealed class GameSessionBuilder : ISessionBuilder
    {
        private readonly IObjectResolver   _sceneContainer;
        private readonly GameplaySceneRefs _refs;
        private readonly Vector3    _carStartPosition;
        private readonly Quaternion _carStartRotation;
        private readonly Quaternion _turretStartRotation;

        /// <summary>
        /// Створює будівника сесії.
        /// </summary>
        /// <param name="sceneContainer">DI-контейнер рівня сцени.</param>
        /// <param name="refs">Посилання на об'єкти сцени.</param>
        public GameSessionBuilder(IObjectResolver sceneContainer, GameplaySceneRefs refs)
        {
            _sceneContainer      = sceneContainer;
            _refs                = refs;
            _carStartPosition    = refs.CarTransform.position;
            _carStartRotation    = refs.CarTransform.rotation;
            _turretStartRotation = refs.TurretTransform.rotation;
        }

        /// <inheritdoc/>
        public ISessionRunner NewSessionRunner()
        {
            ResetSceneTransforms();

            var sessionContainer = _sceneContainer.CreateScope(builder =>
            {
                builder.Register<GameSession>(Lifetime.Scoped);

                builder.Register<TickSystemRegistry>(Lifetime.Scoped);
                builder.Register<DamageService>(Lifetime.Scoped);
                builder.Register<IUnitRegistry, UnitRegistry>(Lifetime.Scoped);
                builder.Register<ITargetsProvider, TargetsProvider>(Lifetime.Scoped);
                builder.Register<HitService>(Lifetime.Scoped);

                builder.Register<GameStatistics>(_ => new GameStatistics(), Lifetime.Scoped);

                builder.Register<IEnemyFactory, EnemyFactory>(Lifetime.Scoped)
                    .WithParameter<Func<EnemyView, Transform, EnemyView>>(c => c.ScopedInstantiate)
                    .WithParameter("parent", _refs.UnitsRoot);

                builder.Register<ProjectileFactory>(Lifetime.Scoped)
                    .WithParameter<Func<ProjectileView, Transform, ProjectileView>>(c => c.ScopedInstantiate)
                    .WithParameter("parent", _refs.UnitsRoot);

                builder.Register<GameFlowService>(Lifetime.Scoped)
                    .WithParameter(_refs.LevelConfig.LevelLength);

                builder.Register<RoadSpawnService>(Lifetime.Scoped)
                    .WithParameter(_refs.LevelConfig);

                builder.Register<GroundLooperService>(Lifetime.Scoped)
                    .WithParameter(_refs.GroundLooperConfig);

                builder.Register<CameraFollowService>(Lifetime.Scoped);
            });

            Initialize(sessionContainer);
            return new GameSessionRunner(sessionContainer);
        }

        private void Initialize(IObjectResolver sessionContainer)
        {
            var session      = sessionContainer.Resolve<GameSession>();
            var tickRegistry = sessionContainer.Resolve<TickSystemRegistry>();
            var statistics   = sessionContainer.Resolve<GameStatistics>();

            var carView = _refs.CarTransform.GetComponent<CarView>();
            var player  = new PlayerUnit(_refs.PlayerConfig, carView, tickRegistry, sessionContainer);
            session.Initialize(player);

            var carColliderLink = carView.GetComponent<UnitColliderLink>();
            if (carColliderLink != null)
            {
                sessionContainer.Inject(carColliderLink);
                carColliderLink.Bind(player);
            }

            var turretView = _refs.TurretTransform.GetComponent<TurretView>();
            _ = new Turret(
                player,
                turretView,
                _refs.PlayerConfig.TurretConfig,
                _sceneContainer.Resolve<IHorizontalDragInput>(),
                _sceneContainer.Resolve<IFireInput>(),
                session,
                tickRegistry,
                sessionContainer.Resolve<ITargetsProvider>());

            statistics.Reset(_refs.LevelConfig.LevelLength);
            session.SetState(GameState.WaitingToStart);
        }

        private void ResetSceneTransforms()
        {
            _refs.CarTransform.SetPositionAndRotation(_carStartPosition, _carStartRotation);
            _refs.TurretTransform.rotation = _turretStartRotation;
        }
    }
}
