using Game.Core.Common;

namespace Game.Core.Session
{
    /// <summary>
    /// Раннер однієї ігрової сесії. Отримує виклики Tick кожен кадр.
    /// </summary>
    public interface ISessionRunner : ITickSystem
    {
        /// <summary>Поточна ігрова сесія.</summary>
        GameSession Session { get; }
    }
}