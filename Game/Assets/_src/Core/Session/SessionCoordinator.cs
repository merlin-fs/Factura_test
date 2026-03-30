using System;
using FTg.Common.Observables;
using Game.Core.Common;

namespace Game.Core.Session
{
    /// <summary>
    /// Координує життєвий цикл ігрових сесій: запуск, тікання та перезапуск.
    /// Публікує події <see cref="NewSession"/> та <see cref="EndSession"/> для підписників UI та ін.
    /// </summary>
    public sealed class SessionCoordinator : IDisposable
    {
        private readonly ISessionBuilder _sessionBuilder;
        private ISessionRunner _runner;
        private bool _restartRequested;

        private readonly ObservableEvent<GameSession> _newSession = new();
        private readonly ObservableEvent<GameSession> _endSession = new();

        /// <summary>Подія, що спрацьовує при старті нової сесії.</summary>
        public IObservable<GameSession> NewSession => _newSession;
        /// <summary>Подія, що спрацьовує при завершенні сесії.</summary>
        public IObservable<GameSession> EndSession => _endSession;

        /// <summary>
        /// Створює координатор.
        /// </summary>
        /// <param name="sessionBuilder">Фабрика для створення нових сесій.</param>
        public SessionCoordinator(ISessionBuilder sessionBuilder)
        {
            _sessionBuilder = sessionBuilder;
        }

        /// <summary>
        /// Завершує поточну сесію (якщо є) та запускає нову.
        /// </summary>
        public void StartNewSession()
        {
            EndCurrentSession();
            _runner = _sessionBuilder.NewSessionRunner();
            _newSession.Raise(_runner.Session);
        }

        /// <summary>
        /// Запитує перезапуск сесії. Перезапуск виконується на початку наступного Tick.
        /// </summary>
        public void RequestRestart()
        {
            _restartRequested = true;
        }

        /// <summary>
        /// Тікає поточний раннер сесії. При запиті перезапуску виконує його першим.
        /// </summary>
        /// <param name="deltaTime">Дельта-час у секундах.</param>
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

        /// <summary>
        /// Завершує поточну сесію та звільняє її ресурси.
        /// </summary>
        public void EndCurrentSession()
        {
            if (_runner == null) return;

            _endSession.Raise(_runner.Session);
            (_runner as IDisposable)?.Dispose();
            _runner = null;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            EndCurrentSession();
        }
    }
}