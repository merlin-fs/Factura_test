using UnityEngine;

namespace Game.Client.Views
{
    /// <summary>
    /// Базовий MonoBehaviour-клас для всіх видів (View).
    /// Реалізує <see cref="IView"/>, повертаючи <c>gameObject</c> як кореневий об'єкт.
    /// </summary>
    public abstract class BaseView : MonoBehaviour, IView
    {
        /// <inheritdoc/>
        public GameObject RootGameObject => gameObject;
    }
}