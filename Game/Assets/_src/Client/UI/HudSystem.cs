using System;
using Game.Core;
using Game.Core.Events;
using UnityEngine;

namespace Game.Client.UI
{
    /// <summary>
    /// Подписывается на GameEvents и обновляет overlay UI.
    /// Реализует IDisposable — отписывается от всех событий при уничтожении сессии.
    /// </summary>
    public sealed class HudSystem : IDisposable
    {
        private readonly GameObject  _winPanel;
        private readonly GameObject  _losePanel;

        private readonly IDisposable _stateSubscription;
        private readonly IDisposable _killSubscription;
        private readonly IDisposable _hpSubscription;

        public HudSystem(
            GameObject     winPanel,
            GameObject     losePanel,
            GameEvents     events,
            Action<float>  setHpBar    = null,
            Action<string> setKillText = null)
        {
            _winPanel = winPanel;
            _losePanel = losePanel;

            _stateSubscription = events.GameStateChanged.Subscribe(OnStateChanged);
            _killSubscription  = events.EnemyKilled.Subscribe(kills => setKillText?.Invoke($"Kills: {kills}"));
            _hpSubscription    = events.CarHealthRatioChanged.Subscribe(ratio => setHpBar?.Invoke(ratio));

            HideAll();
        }

        public void Dispose()
        {
            _stateSubscription?.Dispose();
            _killSubscription?.Dispose();
            _hpSubscription?.Dispose();
        }

        private void OnStateChanged(GameState state)
        {
            HideAll();
            switch (state)
            {
                case GameState.Win:  _winPanel?.SetActive(true);  break;
                case GameState.Lose: _losePanel?.SetActive(true); break;
            }
        }

        private void HideAll()
        {
            _winPanel?.SetActive(false);
            _losePanel?.SetActive(false);
        }
    }
}
