using UnityEngine;

namespace InventoryCursor.CursorStateMachine.States.HoldItem
{
    public class CursorStoppedHoldItemState : CursorBaseState
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
                machine.SwitchState(machine.MovingHoldItemState);
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