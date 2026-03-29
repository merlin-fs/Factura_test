using Game.Core.Common;
using Game.Core.Services;
using Game.Core.Session;
using Game.Client.DI;
using Game.Client.Input;
using Game.Client.Services;
using Game.Client.UI;
using Game.Client.Units;
using Game.Client.Views;
using Game.Core;
using Game.Core.Events;
using Game.Core.Units;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Injectors;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Game.Client.Bootstrap
{
    /// <summary>
    /// Composition root for one gameplay session.
    /// Creates the Reflex child scope, wires up all services and MonoBehaviour controllers,
    /// then hands off a ready-to-tick ISessionRunner.
    /// </summary>
    public sealed class GameSessionBuilder : ISessionBuilder
    {
        private Container         _sceneContainer;
        private GameplaySceneRefs _refs;

        private Vector3    _carStartPosition;
        private Quaternion _carStartRotation;
        private Quaternion _turretStartRotation;

        [Inject]
        private void Inject(Container sceneContainer, GameplaySceneRefs refs)
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

            var sessionContainer = _sceneContainer.Scope(builder =>
            {
                var events = _sceneContainer.Resolve<GameEvents>(); 
                var session = new GameSession(events);
                builder.RegisterValue(session);

                // --- Pools ---
                builder.RegisterFactory<PrefabPool<EnemyView>>(c => new PrefabPool<EnemyView>((prefab, pos, rot) =>
                {
                    var go = Object.Instantiate(prefab, pos, rot);
                    GameObjectInjector.InjectRecursive(go.gameObject, c);
                    return go;
                }), Lifetime.Scoped, Resolution.Eager);
                
                builder.RegisterFactory<PrefabPool<ProjectileView>>(c => new PrefabPool<ProjectileView>((prefab, pos,
                    rot) =>
                {
                    var go = Object.Instantiate(prefab, pos, rot);
                    GameObjectInjector.InjectRecursive(go.gameObject, c);
                    return go;
                }), Lifetime.Scoped, Resolution.Eager);
                
                // --- Registry (kill tracking) ---
                builder.RegisterFactory<EnemyRegistry>(_ => new EnemyRegistry(events), Lifetime.Scoped, Resolution.Eager);

                // --- Factories ---
                builder.RegisterType<IEnemyFactory, EnemyFactory>(Lifetime.Scoped, Resolution.Eager);
                builder.RegisterType<ProjectileFactory>(Lifetime.Scoped, Resolution.Eager);

                // --- Runtime services ---
                builder.RegisterFactory<GameFlowService>(_ => new GameFlowService(_refs.LevelConfig.LevelLength),
                    Lifetime.Scoped, Resolution.Eager);
                builder.RegisterFactory<RoadSpawnService>(c => new RoadSpawnService(_refs.LevelConfig),
                     Lifetime.Scoped, Resolution.Eager);

                builder.RegisterFactory<GroundLooperService>(c => new GroundLooperService(
                    _refs.GroundLooperConfig, _refs, c.Resolve<GameSession>()),
                    Lifetime.Scoped, Resolution.Eager);

                builder.RegisterType<CameraFollowService>(Lifetime.Scoped);

                // --- HUD ---
                /*
                builder.RegisterFactory<HudSystem>(c => new HudSystem(
                    _refs.WinPanel,
                    _refs.LosePanel,
                    events,
                    ratio => { if (_refs.HpBarFill != null) _refs.HpBarFill.fillAmount = ratio; },
                    null), Lifetime.Scoped, Resolution.Eager);
                */
            });

            Initialize(sessionContainer);
            return new GameSessionRunner(sessionContainer);
        }

        // ------------------------------------------------------------------ private

        private void Initialize(Container sessionContainer)
        {
            var session       = sessionContainer.Resolve<GameSession>();

            // --- PlayerUnit: pure C#; CarView is the MonoBehaviour host ---
            var carView = _refs.CarTransform.GetComponent<CarView>();
            var player  = new PlayerUnit(_refs.PlayerConfig, carView);
            AttributeInjector.Inject(player, sessionContainer);

            session.Initialize(player);

            // --- TurretController (чистий C#, реєструється у TickSystemRegistry) ---
            var turretView = _refs.TurretTransform.GetComponent<TurretView>();
            var _ = new Turret(
                player,
                turretView,
                _refs.PlayerConfig.TurretConfig,
                _sceneContainer.Resolve<IHorizontalDragInput>(),
                _sceneContainer.Resolve<IFireInput>(),
                session,
                sessionContainer.Resolve<TickSystemRegistry>());

            // --- Initial HUD state ---
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