using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "PlayerInputScriptableObject", menuName = "ScriptableObjects/PlayerInput")]
    public class PlayerInputScriptableObject : ScriptableObject
    {
        [SerializeField] private Vector2 movement;
        [SerializeField] private bool action;
        [SerializeField] private int rotation;

        public Vector2 Movement
        {
            get => movement;
            set => movement = value;
        }

        public bool Action
        {
            get => action;
            set => action = value;
        }

        public int Rotation
        {
            get => rotation;
            set => rotation = value;
        }

        public void ResetAllValues()
        {
            movement = Vector2.zero;
            action = false;
            rotation = 0;
        }
    }
}