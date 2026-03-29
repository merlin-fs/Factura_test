using System;
using Game.Core.Common;
using Game.Core.Services;
using Game.Client.Input;
using Game.Client.Services;
using Reflex.Core;

namespace Game.Client.Bootstrap
{
    /// <summary>
    /// Drives the update loop for one session.
    /// Tick order: TickSystemRegistry (units/projectiles move) → GameFlow → RoadSpawn → Camera.
    /// </summary>
    public sealed class GameSessionRunner : ITickSystem, IDisposable
    {
        private readonly Container                  _sessionContainer;
        private readonly TickSystemRegistry         _tickRegistry;
        private readonly GameFlowService            _flowService;
        private readonly RoadSpawnService           _roadSpawn;
        private readonly GroundLooperService        _groundLooper;
        private readonly CameraFollowService        _cameraFollow;
        private readonly IFireInput                 _fireInput;

        public GameSessionRunner(Container sessionContainer)
        {
            _sessionContainer = sessionContainer;
            _tickRegistry     = sessionContainer.Resolve<TickSystemRegistry>();
            _flowService      = sessionContainer.Resolve<GameFlowService>();
            _roadSpawn        = sessionContainer.Resolve<RoadSpawnService>();
            _groundLooper     = sessionContainer.Resolve<GroundLooperService>();
            _cameraFollow     = sessionContainer.Resolve<CameraFollowService>();
            _fireInput        = sessionContainer.Resolve<IFireInput>();
        }

        public void Tick(float deltaTime)
        {
            if (_fireInput.WasPressedThisFrame())
                _flowService.RegisterTap();

            // 1. Move all units & projectiles
            _tickRegistry.Tick(deltaTime);

            // 2. React to new positions (win/lose check, spawn, camera, ground)
            _flowService.Tick(deltaTime);
            _roadSpawn.Tick(deltaTime);
            _groundLooper.Tick(deltaTime);
            _cameraFollow.Tick(deltaTime);
        }

        public void Dispose()
        {
            _tickRegistry?.Clear();
            _sessionContainer?.Dispose();
        }
    }
}