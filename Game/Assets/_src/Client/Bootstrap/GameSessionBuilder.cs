using Game.Core.Common;
using Game.Core.Services;
using Game.Core.Session;
using Game.Core.Events;
using Game.Core.Units;
using Game.Client.Input;
using Game.Client.Services;
using Game.Client.Units;
using Game.Client.Views;
using Game.Core;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Game.Client.Bootstrap
{
    /// <summary>
    /// Composition root for one gameplay session.
    /// Creates a VContainer child scope with all session-scoped services,
    /// then returns a ready-to-tick ISessionRunner.
    /// </summary>
    public sealed class GameSessionBuilder : ISessionBuilder
    {
        private readonly IObjectResolver   _sceneContainer;
        private readonly GameplaySceneRefs _refs;
        private readonly Vector3    _carStartPosition;
        private readonly Quaternion _carStartRotation;
        private readonly Quaternion _turretStartRotation;

        public GameSessionBuilder(IObjectResolver sceneContainer, GameplaySceneRefs refs)
        {
            _sceneContainer      = sceneContainer;
            _refs                = refs;
            _carStartPosition    = refs.CarTransform.position;
            _carStartRotation    = refs.CarTransform.rotation;
            _turretStartRotation = refs.TurretTransform.rotation;
        }

        public ITickSystem NewSessionRunner()
        {
            ResetSceneTransforms();

            var sessionContainer = _sceneContainer.CreateScope(builder =>
            {
                // --- Session-scoped events bus ---
                var events = new GameEvents();
                builder.RegisterInstance(events);
                builder.RegisterInstance(new GameSession(events));

                // --- Session-scoped core services ---
                builder.Register<TickSystemRegistry>(Lifetime.Scoped);
                builder.Register<DamageService>(Lifetime.Scoped);
                builder.Register<IUnitRegistry, UnitRegistry>(Lifetime.Scoped);
                builder.Register<ITargetsProvider, TargetsProvider>(Lifetime.Scoped);
                builder.Register<HitService>(Lifetime.Scoped);

                // --- Pools ---
                builder.Register<PrefabPool<EnemyBaseView>>(c => new PrefabPool<EnemyBaseView>((prefab, pos, rot) =>
                {
                    var go = Object.Instantiate(prefab, pos, rot);
                    c.InjectGameObject(go.gameObject);
                    return go;
                }), Lifetime.Scoped);

                builder.Register<PrefabPool<ProjectileBaseView>>(c => new PrefabPool<ProjectileBaseView>((prefab, pos, rot) =>
                {
                    var go = Object.Instantiate(prefab, pos, rot);
                    c.InjectGameObject(go.gameObject);
                    return go;
                }), Lifetime.Scoped);

                // --- Registry ---
                builder.Register<EnemyRegistry>(_ => new EnemyRegistry(events), Lifetime.Scoped);

                // --- Factories (pure constructor injection) ---
                builder.Register<IEnemyFactory, EnemyFactory>(Lifetime.Scoped);
                builder.Register<ProjectileFactory>(Lifetime.Scoped);

                // --- Runtime services (non-DI params passed via WithParameter) ---
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

        // ------------------------------------------------------------------ private

        private void Initialize(IObjectResolver sessionContainer)
        {
            var session      = sessionContainer.Resolve<GameSession>();
            var tickRegistry = sessionContainer.Resolve<TickSystemRegistry>();

            var carView = _refs.CarTransform.GetComponent<CarBaseView>();
            var player  = new PlayerUnit(_refs.PlayerConfig, carView, tickRegistry, sessionContainer);
            session.Initialize(player);

            var turretView = _refs.TurretTransform.GetComponent<TurretBaseView>();
            _ = new Turret(
                player,
                turretView,
                _refs.PlayerConfig.TurretConfig,
                _sceneContainer.Resolve<IHorizontalDragInput>(),
                _sceneContainer.Resolve<IFireInput>(),
                session,
                tickRegistry);

            session.Events.CarHealthRatioChanged.Invoke(1f);
            session.Events.EnemyKilled.Invoke(0);
            session.SetState(GameState.WaitingToStart);
        }

        private void ResetSceneTransforms()
        {
            _refs.CarTransform.SetPositionAndRotation(_carStartPosition, _carStartRotation);
            _refs.TurretTransform.rotation = _turretStartRotation;
        }
    }
}
