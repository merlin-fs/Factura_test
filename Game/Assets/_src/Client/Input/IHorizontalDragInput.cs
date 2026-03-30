namespace Game.Client.Input
{
    /// <summary>
    /// Інтерфейс введення горизонтального свайпу (перетягування пальцем).
    /// Використовується для повороту турелі.
    /// </summary>
    public interface IHorizontalDragInput
    {
        /// <summary>
        /// Повертає горизонтальну дельту переміщення за поточний кадр.
        /// </summary>
        float ReadDeltaX();
    }
}
