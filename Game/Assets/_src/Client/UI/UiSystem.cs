using System;
using FTg.Common.Observables;
using Game.Core;
using Game.Core.Session;
using UnityEngine;
using VContainer.Unity;

namespace Game.Client.UI
{
    /// <summary>
    /// Система UI. Керує видимістю панелей залежно від стану гри.
    /// Підписується на зміни стану сесії та перемикає відповідні панелі.
    /// </summary>
    public sealed class UiSystem : IDisposable, IInitializable
    {
        private readonly GameObject _winPanel;
        private readonly GameObject _losePanel;
        private readonly GameObject _beginGamePanel;
        private readonly GameObject _hudGame;

        private readonly CompositeDisposable _disposables = new();
        private IDisposable _stateSubscription;

        /// <summary>
        /// Створює систему UI та підписується на події координатора сесій.
        /// </summary>
        public UiSystem(SessionCoordinator coordinator,
            GameObject hudGame,
            GameObject beginGamePanel,
            GameObject winPanel,
            GameObject losePanel)
        {
            _hudGame        = hudGame;
            _beginGamePanel = beginGamePanel;
            _winPanel       = winPanel;
            _losePanel      = losePanel;
            coordinator.NewSession.Subscribe(session =>
            {
                _stateSubscription = session.GameStateChanged.Subscribe(OnStateChanged);
                OnStateChanged(session.State);
            }).AddTo(_disposables);
            coordinator.EndSession.Subscribe(_ =>
            {
                _stateSubscription?.Dispose();
            }).AddTo(_disposables);
            HideAll();
        }

        /// <inheritdoc/>
        public void Initialize() { }

        /// <inheritdoc/>
        public void Dispose()
        {
            _disposables.Dispose();
            _stateSubscription?.Dispose();
        }

        private void OnStateChanged(GameState state)
        {
            HideAll();
            switch (state)
            {
                case GameState.Win:            _winPanel?.SetActive(true);         break;
                case GameState.Lose:           _losePanel?.SetActive(true);        break;
                case GameState.WaitingToStart: _beginGamePanel?.SetActive(true);   break;
                case GameState.Playing:        _hudGame?.SetActive(true);          break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void HideAll()
        {
            _hudGame?.SetActive(false);
            _winPanel?.SetActive(false);
            _losePanel?.SetActive(false);
            _beginGamePanel?.SetActive(false);
        }
    }
}
