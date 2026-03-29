using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Client.Input
{
    public sealed class HorizontalDragInput : IHorizontalDragInput, IDisposable
    {
        private readonly InputAction _action;

        public HorizontalDragInput(InputActionReference actionReference)
        {
            if (actionReference == null)
                throw new ArgumentNullException(nameof(actionReference));

            _action = actionReference.action
                      ?? throw new InvalidOperationException("InputActionReference.action is null");

            _action.Enable();
        }

        public float ReadDeltaX()
        {
            Vector2 delta = _action.ReadValue<Vector2>();
            return delta.x;
        }

        public void Dispose()
        {
            _action.Disable();
        }
    }
}