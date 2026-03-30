namespace Game.Core
{
    /// <summary>
    /// Загальний стан ігрової сесії.
    /// </summary>
    public enum GameState
    {
        /// <summary>Очікування початку — гра на паузі до першого торкання екрана.</summary>
        WaitingToStart,
        /// <summary>Гра активна — гравець рухається і бореться.</summary>
        Playing,
        /// <summary>Гравець переміг.</summary>
        Win,
        /// <summary>Гравець програв.</summary>
        Lose
    }
}
