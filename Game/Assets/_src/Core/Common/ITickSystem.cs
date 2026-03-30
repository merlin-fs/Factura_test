namespace Game.Core.Common
{
    /// <summary>
    /// Система, що виконує оновлення кожного кадру або з рівним інтервалом часу.
    /// Реєструється у <see cref="TickSystemRegistry"/> і отримує виклики <see cref="Tick"/> автоматично.
    /// </summary>
    public interface ITickSystem
    {
        /// <summary>
        /// Викликається кожен кадр із часом, що минув із попереднього кадру.
        /// </summary>
        /// <param name="dt">Дельта-час у секундах.</param>
        void Tick(float dt);
    }
}