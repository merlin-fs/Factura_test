using System;
using FTg.Common.Observables;

namespace Game.Core.Services
{
    /// <summary>
    /// Статистика ігрової сесії: кількість знищених ворогів і поточна кількість живих ворогів.
    /// </summary>
    public sealed class GameStatistics
    {
        /// <summary>Подія, що спрацьовує при знищенні ворога. Передає поточний лічильник вбивств.</summary>
        public IObservable<int> OnEnemyKilled => _onEnemyKilled;
        /// <summary>Загальна кількість знищених ворогів у цій сесії.</summary>
        public int KillCount => _killCount;

        private int _killCount;
        private readonly ObservableEvent<int> _onEnemyKilled = new();

        /// <summary>Кількість ворогів, що зараз живі на рівні.</summary>
        public int LiveCount { get; private set; }
        /// <summary>Довжина рівня вздовж осі Z.</summary>
        public float LevelLength { get; private set; }

        /// <summary>
        /// Скидає всю статистику на початок нової сесії.
        /// </summary>
        /// <param name="levelLength">Довжина рівня.</param>
        public void Reset(float levelLength)
        {
            LiveCount = 0;
            _killCount = 0;
            _onEnemyKilled.Raise(_killCount);
            LevelLength = levelLength;
        }

        /// <summary>Збільшує лічильник живих ворогів при спауні.</summary>
        public void TrackSpawn() => LiveCount++;

        /// <summary>
        /// Зменшує лічильник живих ворогів і збільшує лічильник вбивств при загибелі ворога.
        /// </summary>
        public void ReportKill()
        {
            LiveCount--;
            _onEnemyKilled.Raise(++_killCount);
        }

        /// <summary>Зменшує лічильник живих ворогів при видаленні ворога без вбивства.</summary>
        public void TrackRelease() => LiveCount--;
    }
}
