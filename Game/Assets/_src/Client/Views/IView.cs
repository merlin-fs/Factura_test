using UnityEngine;

namespace Game.Client.Views
{
    /// <summary>
    /// Базовий інтерфейс усіх видів (View) у клієнтському шарі.
    /// Дозволяє отримати кореневий <see cref="GameObject"/> незалежно від реалізації.
    /// </summary>
    public interface IView
    {
        /// <summary>Кореневий <see cref="GameObject"/> цього вигляду.</summary>
        GameObject RootGameObject { get; }
    }
}
