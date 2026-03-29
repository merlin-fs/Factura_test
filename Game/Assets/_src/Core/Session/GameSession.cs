using Game.Core.Events;
using Game.Core.Units;
using UnityEngine;

namespace Game.Core.Session
{
    /// <summary>
    /// Holds per-session state: events bus, game state, player unit reference, pause flag.
    /// Lifecycle methods are the only authoritative way to change state from outside.
    /// </summary>
    public sealed class GameSession
    {
        public GameEvents Events          { get; }
        public GameState  State           { get; private set; } = GameState.WaitingToStart;
        public bool       IsPaused        { get; private set; } = true;
        public Unit       Player          { get; private set; }
        public float      StartPositionZ  { get; private set; }

        public GameSession(GameEvents events)
        {
            Events = events;
        }

        /// <summary>Called once by GameSessionBuilder after the player unit is ready.</summary>
        public void Initialize(Unit player)
        {
            Player         = player;
            StartPositionZ = player.Position.z;
        }

        // ------------------------------------------------------------------ lifecycle

        /// <summary>Начать сессию: снять паузу и перейти в Playing.</summary>
        public void Begin()
        {
            IsPaused = false;
            SetState(GameState.Playing);
        }

        /// <summary>Поставить на паузу без изменения GameState.</summary>
        public void Pause() => IsPaused = true;

        /// <summary>Снять паузу без изменения GameState.</summary>
        public void Resume() => IsPaused = false;

        /// <summary>Завершить победой: пауза + Win.</summary>
        public void FinishWin()
        {
            IsPaused = true;
            SetState(GameState.Win);
        }

        /// <summary>Завершить поражением: пауза + Lose.</summary>
        public void FinishLose()
        {
            IsPaused = true;
            SetState(GameState.Lose);
        }

        /// <summary>Changes state and fires GameStateChanged event.</summary>
        public void SetState(GameState newState)
        {
            State = newState;
            Events.GameStateChanged.Invoke(newState);
        }
    }
}