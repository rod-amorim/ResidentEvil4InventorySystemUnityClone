using UnityEngine;

namespace InventoryCursor.CursorStateMachine.States.HoldItem
{
    public class CursorMovingHoldItemState : CursorBaseState
    {
        private float _holdCurrentTime;
        private float _holdTotalTime = .1f;

        private bool _autoScroll;

        private float _scrollCurrentTime;
        private float _scrollMaxTime = .2f;

        public override void OnUnpauseStateMachine(CursorStateMachine machine)
        {
        }

        public override void OnEnter(CursorStateMachine machine)
        {
            _holdCurrentTime = 0;
            _scrollCurrentTime = 0;
        }

        public override void OnUpdate(CursorStateMachine machine)
        {
            _holdCurrentTime += Time.deltaTime;

            machine.cursorMovement.MoveCursor(machine.playerInput.Movement);

            if (_holdCurrentTime >= _holdTotalTime)
            {
                _autoScroll = true;
            }

            if (_autoScroll)
            {
                _scrollCurrentTime += Time.deltaTime;
            }

            if (_scrollCurrentTime >= _scrollMaxTime)
            {
                machine.cursorMovement.MoveCursor(Vector2.zero);
                _scrollCurrentTime = 0;
            }

            if (machine.playerInput.Movement == Vector2.zero)
            {
                machine.SwitchState(machine.StoppedHoldItemState);
            }
        }

        public override void OnExit(CursorStateMachine machine)
        {
            _autoScroll = false;
        }

        public override void OnpauseStateMachine(CursorStateMachine machine)
        {
        }
    }
}