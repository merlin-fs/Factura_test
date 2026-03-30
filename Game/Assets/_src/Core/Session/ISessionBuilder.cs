using Game.Core.Common;

namespace Game.Core.Session
{
    /// <summary>
    /// Фабрика ігрових сесій. Реалізується у шарі Client (<c>GameSessionBuilder</c>).
    /// </summary>
    public interface ISessionBuilder
    {
        /// <summary>
        /// Створює новий <see cref="ISessionRunner"/> для однієї ігрової сесії.
        /// </summary>
        /// <returns>Готовий до тікання раннер сесії.</returns>
        ISessionRunner NewSessionRunner();
    }
}
