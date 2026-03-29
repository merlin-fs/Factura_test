using Game.Core.Common;

namespace Game.Core.Session
{
    /// <summary>
    /// Фабрика сессий. Реализуется GameSessionBuilder в Client.
    /// </summary>
    public interface ISessionBuilder
    {
        ITickSystem NewSessionRunner();
    }
}

