using FTg.Common.Observables;

namespace Game.Core.Events
{
    /// <summary>
    /// Typed event bus. Services raise events; HUD/Audio/etc. subscribe.
    /// Passed by reference — no static singletons.
    /// </summary>
    public class GameEvents
    {
        /// <summary>Состояние гри змінився (WaitingToStart / Playing / Win / Lose).</summary>
        public readonly ObservableEvent<GameState> GameStateChanged = new ObservableEvent<GameState>();

        /// <summary>HP автомобіля змінилось (нове значення 0..1).</summary>
        public readonly ObservableEvent<float> CarHealthRatioChanged = new ObservableEvent<float>();

        /// <summary>Ворог знешкоджений. Аргумент — кількість кілів.</summary>
        public readonly ObservableEvent<int> EnemyKilled = new ObservableEvent<int>();
    }
}
