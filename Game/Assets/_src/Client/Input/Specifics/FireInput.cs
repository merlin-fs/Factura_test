using System;
using UnityEngine.InputSystem;

namespace Game.Client.Input
{
    /// <summary>
    /// Реалізація <see cref="IFireInput"/> на основі Unity Input System.
    /// </summary>
    public sealed class FireInput : IFireInput, IDisposable
    {
        private readonly InputAction _action;

        /// <summary>
        /// Створює екземпляр та активує дію вводу.
        /// </summary>
        /// <param name="actionReference">Посилання на InputAction зі сцени.</param>
        public FireInput(InputActionReference actionReference)
        {
            if (actionReference == null)
                throw new ArgumentNullException(nameof(actionReference));

            _action = actionReference.action
                      ?? throw new InvalidOperationException("InputActionReference.action is null");

            _action.Enable();
        }

        /// <inheritdoc/>
        public bool IsPressed() => _action.IsPressed();

        /// <inheritdoc/>
        public bool WasPressedThisFrame() => _action.WasPressedThisFrame();

        /// <inheritdoc/>
        public bool WasReleasedThisFrame() => _action.WasReleasedThisFrame();

        /// <summary>Деактивує дію вводу.</summary>
        public void Dispose() => _action.Disable();
    }
}