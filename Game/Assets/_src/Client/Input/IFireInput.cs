namespace Game.Client.Input
{
    /// <summary>
    /// Інтерфейс введення стрільби (кнопка «Вогонь»).
    /// </summary>
    public interface IFireInput
    {
        /// <summary>Повертає <c>true</c>, якщо кнопка стрільби утримується.</summary>
        bool IsPressed();
        /// <summary>Повертає <c>true</c>, якщо кнопка стрільби була натиснута цього кадру.</summary>
        bool WasPressedThisFrame();
        /// <summary>Повертає <c>true</c>, якщо кнопка стрільби була відпущена цього кадру.</summary>
        bool WasReleasedThisFrame();
    }
}