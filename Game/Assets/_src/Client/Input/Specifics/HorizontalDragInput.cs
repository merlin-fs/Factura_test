using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Client.Input
{
    /// <summary>
    /// Реалізація <see cref="IHorizontalDragInput"/> на основі Unity Input System.
    /// Зчитує горизонтальну складову дельти переміщення (свайп/миша).
    /// </summary>
    public sealed class HorizontalDragInput : IHorizontalDragInput, IDisposable
    {
        private readonly InputAction _action;

        /// <summary>
        /// Створює екземпляр та активує дію вводу.
        /// </summary>
        /// <param name="actionReference">Посилання на InputAction зі сцени.</param>
        public HorizontalDragInput(InputActionReference actionReference)
        {
            if (actionReference == null)
                throw new ArgumentNullException(nameof(actionReference));

            _action = actionReference.action
                      ?? throw new InvalidOperationException("InputActionReference.action is null");

            _action.Enable();
        }

        /// <inheritdoc/>
        public float ReadDeltaX()
        {
            Vector2 delta = _action.ReadValue<Vector2>();
            return delta.x;
        }

        /// <summary>Деактивує дію вводу.</summary>
        public void Dispose() => _action.Disable();
    }
}