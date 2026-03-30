using Game.Core.Units;

namespace Game.Core.Services
{
    /// <summary>
    /// Обробник влучання снаряду або атаки у юніта.
    /// </summary>
    public interface IHitHandler
    {
        /// <summary>
        /// Викликається при влучанні у ціль.
        /// </summary>
        /// <param name="source">Юніт, що завдав удар.</param>
        /// <param name="target">Юніт, що отримав удар.</param>
        void Handle(Unit source, Unit target);
    }
}