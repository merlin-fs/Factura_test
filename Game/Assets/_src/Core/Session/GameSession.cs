using System;
using FTg.Common.Observables;
using Game.Core.Services;
using Game.Core.Units;

namespace Game.Core.Session
{
    /// <summary>
    /// Зберігає стан поточної ігрової сесії: шину подій, стан гри, посилання на гравця та ознаку паузи.
    /// Методи життєвого циклу є єдиним авторитетним способом зміни стану ззовні.
    /// </summary>
    public sealed class GameSession
    {
        /// <summary>Поточний стан гри.</summary>
        public GameState  State           { get; private set; } = GameState.WaitingToStart;
        /// <summary>Подія, що спрацьовує при зміні стану гри.</summary>
        public IObservable<GameState> GameStateChanged => _gameStateChanged;

        /// <summary>Статистика поточної сесії.</summary>
        public GameStatistics GameStatistics { get; private set; }
        /// <summary>Чи знаходиться гра на паузі.</summary>
        public bool       IsPaused        { get; private set; } = true;
        /// <summary>Юніт гравця.</summary>
        public Unit       Player          { get; private set; }
        /// <summary>Координата Z стартової позиції гравця.</summary>
        public float      StartPositionZ  { get; private set; }

        private readonly ObservableEvent<GameState> _gameStateChanged = new();

        /// <summary>
        /// Створює нову сесію зі вказаною статистикою.
        /// </summary>
        /// <param name="gameStatistics">Об'єкт статистики сесії.</param>
        public GameSession(GameStatistics gameStatistics)
        {
            GameStatistics = gameStatistics;
        }

        /// <summary>
        /// Ініціалізує сесію після створення юніта гравця.
        /// Викликається один раз із <c>GameSessionBuilder</c>.
        /// </summary>
        /// <param name="player">Готовий юніт гравця.</param>
        public void Initialize(Unit player)
        {
            Player         = player;
            StartPositionZ = player.Position.z;
        }

        /// <summary>
        /// Починає сесію: знімає паузу та переходить у стан <see cref="GameState.Playing"/>.
        /// </summary>
        public void Begin()
        {
            IsPaused = false;
            SetState(GameState.Playing);
        }

        /// <summary>
        /// Ставить гру на паузу без зміни <see cref="State"/>.
        /// </summary>
        public void Pause() => IsPaused = true;

        /// <summary>
        /// Знімає паузу без зміни <see cref="State"/>.
        /// </summary>
        public void Resume() => IsPaused = false;

        /// <summary>
        /// Завершує сесію перемогою: пауза та стан <see cref="GameState.Win"/>.
        /// </summary>
        public void FinishWin()
        {
            IsPaused = true;
            SetState(GameState.Win);
        }

        /// <summary>
        /// Завершує сесію поразкою: пауза та стан <see cref="GameState.Lose"/>.
        /// </summary>
        public void FinishLose()
        {
            IsPaused = true;
            SetState(GameState.Lose);
        }

        /// <summary>
        /// Встановлює новий стан та викликає подію <see cref="GameStateChanged"/>.
        /// </summary>
        /// <param name="newState">Новий стан гри.</param>
        public void SetState(GameState newState)
        {
            State = newState;
            _gameStateChanged.Raise(newState);
        }
    }
}