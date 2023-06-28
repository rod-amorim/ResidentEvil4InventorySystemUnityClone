using UnityEngine;

namespace InventoryCursor.CursorStateMachine.States.Idle
{
    public class CursorStoppedState : CursorBaseState
    {

        public override void OnUnpauseStateMachine(CursorStateMachine machine)
        {
        }

        public override void OnEnter(CursorStateMachine machine)
        {
        }

        public override void OnUpdate(CursorStateMachine machine)
        {
            if (machine.playerInput.Movement != Vector2.zero)
            {
                machine.SwitchState(machine.MovingState);
            }
        }

        public override void OnExit(CursorStateMachine machine)
        {
        }

        public override void OnpauseStateMachine(CursorStateMachine machine)
        {
        }
    }
}