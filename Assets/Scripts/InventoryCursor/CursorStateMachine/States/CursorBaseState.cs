namespace InventoryCursor.CursorStateMachine.States
{
    public abstract class CursorBaseState
    {
        public abstract void OnUnpauseStateMachine(CursorStateMachine machine);

        public abstract void OnEnter(CursorStateMachine machine);

        public abstract void OnUpdate(CursorStateMachine machine);

        public abstract void OnExit(CursorStateMachine machine);
        public abstract void OnpauseStateMachine(CursorStateMachine machine);
    }
}