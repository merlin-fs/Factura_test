using System;
using Game.Core.Common;
using Game.Core.Services;
using Game.Client.Input;
using Game.Client.Services;
using Game.Core.Session;
using VContainer;

namespace Game.Client.Session
{
    /// <summary>
    /// Керує циклом оновлення однієї ігрової сесії.
    /// Порядок тікання: <see cref="TickSystemRegistry"/> (рух юнітів/снарядів) →
    /// <see cref="GameFlowService"/> → <see cref="RoadSpawnService"/> → камера.
    /// </summary>
    public sealed class GameSessionRunner : ISessionRunner, IDisposable
    {
        /// <inheritdoc/>
        public GameSession Session => _session;

        private readonly IObjectResolver     _sessionContainer;
        private readonly TickSystemRegistry  _tickRegistry;
        private readonly GameFlowService     _flowService;
        private readonly RoadSpawnService    _roadSpawn;
        private readonly GroundLooperService _groundLooper;
        private readonly CameraFollowService _cameraFollow;
        private readonly IFireInput          _fireInput;
        private readonly GameSession         _session;

        /// <summary>
        /// Створює раннер і резолвить усі сервіси зі скоупу сесії.
        /// </summary>
        /// <param name="sessionContainer">DI-контейнер сесії.</param>
        public GameSessionRunner(IObjectResolver sessionContainer)
        {
            _sessionContainer = sessionContainer;
            _session          = sessionContainer.Resolve<GameSession>();
            _tickRegistry     = sessionContainer.Resolve<TickSystemRegistry>();
            _flowService      = sessionContainer.Resolve<GameFlowService>();
            _roadSpawn        = sessionContainer.Resolve<RoadSpawnService>();
            _groundLooper     = sessionContainer.Resolve<GroundLooperService>();
            _cameraFollow     = sessionContainer.Resolve<CameraFollowService>();
            _fireInput        = sessionContainer.Resolve<IFireInput>();
        }

        /// <inheritdoc/>
        public void Tick(float deltaTime)
        {
            if (_fireInput.WasPressedThisFrame())
                _flowService.RegisterTap();
            _cameraFollow.Tick(deltaTime);
            _flowService.Tick(deltaTime);
            _roadSpawn.Tick(deltaTime);
            _groundLooper.Tick(deltaTime);

            if (_session.IsPaused) return;

            _tickRegistry.Tick(deltaTime);
        }

        /// <summary>
        /// Очищає реєстр систем та звільняє DI-скоуп сесії.
        /// </summary>
        public void Dispose()
        {
            _tickRegistry?.Clear();
            _sessionContainer?.Dispose();
        }
    }
}