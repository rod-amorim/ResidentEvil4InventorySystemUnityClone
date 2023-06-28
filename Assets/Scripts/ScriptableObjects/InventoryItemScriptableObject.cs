using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "InventoryItem", menuName = "InventoryItem", order = 0)]
    public class InventoryItemScriptableObject : ScriptableObject
    {
        [SerializeField] private Vector2 size;

        [SerializeField] private GameObject prefab;

        public Vector2 Size => size;

        public GameObject Prefab => prefab;
    }
}