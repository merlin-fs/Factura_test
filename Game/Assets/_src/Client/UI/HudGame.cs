using System;
using FTg.Common.Observables;
using Game.Core.Session;
using Game.Core.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Client.UI
{
    /// <summary>
    /// HUD активної ігрової сесії. Відображає рахунок, здоров'я та прогрес рівня.
    /// </summary>
    public class HudGame : BaseGameWindow
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Image healthBarFill;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Image levelBarFill;

        private CompositeDisposable _disposables = new();
        private GameSession _session;

        /// <inheritdoc/>
        protected override void Bind(GameSession session)
        {
            base.Bind(session);
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
            _session = session;
            _session.Player.Stats.Get<HpStat>().OnChange.Subscribe(OnHealthChanged).AddTo(_disposables);
            OnHealthChanged(_session.Player.Stats.Get<HpStat>());
            _session.GameStatistics.OnEnemyKilled.Subscribe(OnScoreChanged).AddTo(_disposables);
            OnScoreChanged(_session.GameStatistics.KillCount);
        }

        private void Update()
        {
            if (_session == null) return;
            var length = _session.Player.Position.z / (_session.StartPositionZ + _session.GameStatistics.LevelLength);
            levelBarFill.fillAmount = Mathf.Clamp01(length);
        }

        private void OnScoreChanged(int score)
        {
            scoreText.text = $"Score: {score}";
        }

        private void OnHealthChanged(UnitStat stat)
        {
            healthBarFill.fillAmount = stat.Ratio;
            healthText.text = $"{(int)stat.Value} / {(int)stat.Max}";
        }

        /// <inheritdoc/>
        protected override void Unbind(GameSession session)
        {
            base.Unbind(session);
            _disposables?.Dispose();
            _disposables = null;
            _session = null;
        }
    }
}