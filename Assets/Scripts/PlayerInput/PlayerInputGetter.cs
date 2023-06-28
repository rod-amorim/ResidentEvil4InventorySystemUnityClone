using InventoryCursor.CursorStateMachine;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInput
{
    public class PlayerInputGetter : MonoBehaviour
    {
        [SerializeField] private CursorStateMachine cursorStateMachine;
        [SerializeField] private PlayerInputScriptableObject playerInput;

        public void OnMoveCursorVertical(InputAction.CallbackContext context)
        {
            playerInput.Movement = new Vector2(playerInput.Movement.x, context.ReadValue<float>());
        }

        public void OnMoveCursorHorizontal(InputAction.CallbackContext context)
        {
            playerInput.Movement = new Vector2(context.ReadValue<float>(), playerInput.Movement.y);
        }

        public void OnActionPress(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<float>();

            if (value > 0)
            {
                cursorStateMachine.OnActionPress();
            }

            playerInput.Action = value != 0;
        }

        public void OnRotatePress(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<float>();

            if (value != 0)
            {
                cursorStateMachine.OnRotatePress(value);
            }

            playerInput.Action = value != 0;
        }
    }
}