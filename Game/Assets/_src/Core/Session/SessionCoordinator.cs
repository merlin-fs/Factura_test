using System;
using Game.Core.Common;

namespace Game.Core.Session
{
    public sealed class SessionCoordinator : IDisposable
    {
        private readonly ISessionBuilder _sessionBuilder;
        private ITickSystem _runner;
        private bool _restartRequested;

        public SessionCoordinator(ISessionBuilder sessionBuilder)
        {
            _sessionBuilder = sessionBuilder;
        }

        public void StartNewSession()
        {
            EndCurrentSession();
            _runner = _sessionBuilder.NewSessionRunner();
        }

        /// <summary>
        /// Запрашивает перезапуск сессии на следующем тике.
        /// Безопасно вызывать внутри текущего тика.
        /// </summary>
        public void RequestRestart()
        {
            _restartRequested = true;
        }

        public void Tick(float deltaTime)
        {
            if (_restartRequested)
            {
                _restartRequested = false;
                StartNewSession();
                return;
            }

            _runner?.Tick(deltaTime);
        }

        public void EndCurrentSession()
        {
            (_runner as IDisposable)?.Dispose();
            _runner = null;
        }

        public void Dispose()
        {
            EndCurrentSession();
        }
    }
}