using Game.Core.Events;
using Game.Core.Units;
using UnityEngine;

namespace Game.Core.Session
{
    /// <summary>
    /// Holds per-session state: events bus, game state, player unit reference, pause flag.
    /// </summary>
    public sealed class GameSession
    {
        public GameEvents Events          { get; }
        public GameState  State           { get; private set; } = GameState.WaitingToStart;
        public bool       IsPaused        { get; set; }         = true;
        public Unit       Player          { get; private set; }
        public float      StartPositionZ  { get; private set; }

        public GameSession(GameEvents events)
        {
            Events = events;
        }

        /// <summary>Called once by GameSessionBuilder after the player unit is ready.</summary>
        public void Initialize(Unit player)
        {
            Player          = player;
            StartPositionZ  = player.Position.z;
        }

        /// <summary>Changes state and fires GameStateChanged event.</summary>
        public void SetState(GameState newState)
        {
            State = newState;
            Events.GameStateChanged.Invoke(newState);
        }
    }
}