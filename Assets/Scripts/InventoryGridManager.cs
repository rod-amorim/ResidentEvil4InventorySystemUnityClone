using System;
using System.Collections.Generic;
using InventoryCursor.CursorStateMachine;
using ScriptableObjects;
using UnityEngine;

public class InventoryGridManager : MonoBehaviour
{
    [SerializeField] private CursorMovement cursorMovement;
    [SerializeField] private List<ItemSpec> itemListSpec;
    [SerializeField] private Transform item3dParent;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private SpriteRenderer cellRenderer;
    [SerializeField] private bool showDebug;

    private Grid<Item3dModel> _grid;

    public Grid<Item3dModel> Grid => _grid;

    public int Width => width;

    public int Height => height;

    private Vector3 initialDebugPoint;

    void Start()
    {
        UpdateInventory();
        initialDebugPoint = cellRenderer.transform.position;
    }

    private void OnDrawGizmos()
    {
        if (_grid == null)
            return;

        if (showDebug)
        {
            for (int x = 0; x < _grid.GridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GridArray.GetLength(1); y++)
                {
                    var hasItem = _grid.GridArray[x, y] != null;

                    var xScaledValue = x * transform.localScale.x;
                    var yScaledValue = y * -transform.localScale.y;

                    Gizmos.color = hasItem ? Color.red : Color.white;

                    Gizmos.DrawSphere(
                        new Vector2(xScaledValue + initialDebugPoint.x,
                            yScaledValue + initialDebugPoint.y), .2f);
                }
            }
        }
    }

    [ContextMenu("UpdateInventory")]
    public void UpdateInventory()
    {
        _grid = new Grid<Item3dModel>(width, height);

        cellRenderer.size = new Vector2(_grid.GridArray.GetLength(0), _grid.GridArray.GetLength(1));

        DeSpawnItems();
        SpawnItems();
        cursorMovement.DrawCursor();
    }

    private void SpawnItems()
    {
        foreach (var item3d in itemListSpec)
        {
            var item = Instantiate(item3d.ItemScriptableObject.Prefab, item3dParent);
            var item3dModel = item.AddComponent<Item3dModel>();
            item3dModel.Pos = item3d.Pos;
            item3dModel.Size = item3d.ItemScriptableObject.Size;
            item3dModel.ItemScriptableObject = item3d.ItemScriptableObject;
            item3dModel.SetItemPosByGridPos();
            item3dModel.InventoryGridManager = this;

            for (var x = item3d.Pos.x; x <= item3d.Pos.x + item3d.ItemScriptableObject.Size.x - 1; x++)
            {
                for (var y = item3d.Pos.y; y <= item3d.Pos.y + item3d.ItemScriptableObject.Size.y - 1; y++)
                {
                    _grid.GridArray[(int) x - 1, (int) y - 1] = item3dModel;
                }
            }
        }
    }

    private void DeSpawnItems()
    {
        foreach (Transform itemObj in item3dParent)
        {
            Destroy(itemObj.gameObject);
        }
    }

    public Item3dModel GetItemFromPos(float x, float y)
    {
        if (Grid == null)
            return null;

        return Grid.GridArray[(int) x, (int) y];
    }

    [Serializable]
    class ItemSpec
    {
        [SerializeField] private Vector2 pos;
        [SerializeField] private InventoryItemScriptableObject itemScriptableObject;

        public Vector2 Pos => pos;

        public InventoryItemScriptableObject ItemScriptableObject => itemScriptableObject;
    }
}