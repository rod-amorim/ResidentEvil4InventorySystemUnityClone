using InventoryCursor.CursorStateMachine.States;
using InventoryCursor.CursorStateMachine.States.HoldItem;
using InventoryCursor.CursorStateMachine.States.Idle;
using ScriptableObjects;
using UnityEngine;

namespace InventoryCursor.CursorStateMachine
{
    public class CursorStateMachine : MonoBehaviour
    {
        //pivot will be the upper left cell
        [SerializeField] protected internal PlayerInputScriptableObject playerInput;
        [SerializeField] protected internal CursorMovement cursorMovement;

        //States
        public CursorBaseState LastState;
        public CursorBaseState CurrentState;
        public CursorMovingState MovingState = new();
        public CursorMovingHoldItemState MovingHoldItemState = new();
        public CursorStoppedState StoppedState = new();
        public CursorStoppedHoldItemState StoppedHoldItemState = new();
        public CursorPausedState PausedState = new();

        [TextArea] [SerializeField] private string lastState;
        [TextArea] [SerializeField] private string actualState;

        private void Start()
        {
            CurrentState = StoppedState;
            CurrentState.OnEnter(this);
        }

        private void Update()
        {
            var actualStateSplit = CurrentState.ToString().Split(".");
            actualState = actualStateSplit[^1];

            if (LastState != null)
            {
                var lastStateSplit = LastState.ToString().Split(".");
                lastState = lastStateSplit[^1];
            }

            CurrentState.OnUpdate(this);
        }

        public void SwitchState(CursorBaseState state)
        {
            if (CurrentState == null || CurrentState == state)
                return;

            LastState = CurrentState;

            if (state == PausedState)
                CurrentState.OnpauseStateMachine(this);
            else
                CurrentState.OnExit(this);

            if (LastState == PausedState)
                state.OnUnpauseStateMachine(this);
            else
                state.OnEnter(this);

            CurrentState = state;
        }

        public void OnActionPress()
        {
            if (InHoldingItemState())
            {
                var itemReleased = cursorMovement.OnTryReleaseItem();
                if (itemReleased)
                {
                    SwitchState(StoppedState);
                }

                return;
            }

            if (!InHoldingItemState() && cursorMovement.GetItemActualPos())
            {
                cursorMovement.OnHoldItem(cursorMovement.GetItemActualPos());
                SwitchState(StoppedHoldItemState);
            }
        }

        public void OnRotatePress(float value)
        {
            if (InHoldingItemState())
            {
                cursorMovement.RotateItem(value);
            }
        }

        public bool InHoldingItemState()
        {
            return CurrentState == MovingHoldItemState || CurrentState == StoppedHoldItemState;
        }
    }
}