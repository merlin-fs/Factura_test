using System;
using UnityEngine.InputSystem;

namespace Game.Client.Input
{
    public sealed class FireInput : IFireInput, IDisposable
    {
        private readonly InputAction _action;

        public FireInput(InputActionReference actionReference)
        {
            if (actionReference == null)
                throw new ArgumentNullException(nameof(actionReference));

            _action = actionReference.action
                      ?? throw new InvalidOperationException("InputActionReference.action is null");

            _action.Enable();
        }

        public bool IsPressed()
        {
            return _action.IsPressed();
        }

        public bool WasPressedThisFrame()
        {
            return _action.WasPressedThisFrame();
        }

        public bool WasReleasedThisFrame()
        {
            return _action.WasReleasedThisFrame();
        }

        public void Dispose()
        {
            _action.Disable();
        }
    }
}