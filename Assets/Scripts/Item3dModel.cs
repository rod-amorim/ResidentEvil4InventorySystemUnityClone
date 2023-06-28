using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public class Item3dModel : MonoBehaviour
{
    [SerializeField] private Vector2 pos;
    [SerializeField] private Vector2 size;
    [SerializeField] private Vector2 itemMiddlePoint;
    [SerializeField] private InventoryItemScriptableObject itemScriptableObject;
    [SerializeField] private bool inHold;
    [SerializeField] private InventoryGridManager inventoryGridManager;

    public Vector2 Pos
    {
        get => pos;
        set => pos = value;
    }

    public Vector2 Size
    {
        get => size;
        set => size = value;
    }

    private bool inOffsetPos;

    public bool InOffsetPos => inOffsetPos;

    public Vector2 ItemMiddlePoint => itemMiddlePoint;

    public InventoryItemScriptableObject ItemScriptableObject
    {
        get => itemScriptableObject;
        set => itemScriptableObject = value;
    }

    public InventoryGridManager InventoryGridManager
    {
        get => inventoryGridManager;
        set => inventoryGridManager = value;
    }

    public void SetItemPosByGridPos()
    {
        transform.localPosition = new Vector2(Pos.x + Size.x * .5f - 1,
            -(Pos.y + Size.y * .5f - 1));
        if (inHold)
        {
            var thisObj = gameObject;
            thisObj.transform.localPosition += thisObj.transform.right * .5f;
        }
    }

    public void OnItemHold()
    {
        inHold = true;
        var thisObj = gameObject;
        thisObj.transform.localPosition += thisObj.transform.right * .3f;
    }

    public void OnItemRelease()
    {
        inHold = false;
        var thisObj = gameObject;
        thisObj.transform.localPosition -= thisObj.transform.right * .3f;
    }

    public void OnRotateItem(float value)
    {
        size = new Vector2(size.y, size.x);
        transform.RotateAround(itemMiddlePoint, -transform.right, value > 0 ? -90 : 90);
    }

    public Vector2 GetItemMostMiddleSlot()
    {
        var x = (int) Math.Ceiling(Size.x / 2) + Pos.x - 2;
        var y = -(int) Math.Ceiling(Size.y / 2) - Pos.y + 2;

        return new Vector2(x, y);
    }

    public void UpdateItemMiddlePoint()
    {
        var scale = inventoryGridManager.transform.localScale;

        var itemPos = new Vector3((pos.x - 1) * scale.x, (-pos.y + 1) * scale.x) +
                      inventoryGridManager.transform.position;


        var middlePoint = new Vector3((size.x / 2) * scale.x, (-size.y / 2) * scale.y);
        itemMiddlePoint = middlePoint + itemPos;
    }

    public List<Vector2> GetItemMarginUpPos()
    {
        var firstItemSlotWorldPos = new Vector2(Pos.x - 1, -Pos.y + 1);

        List<Vector2> returnList = new List<Vector2>();

        for (int column = 0; column < Size.x; column++)
        {
            returnList.Add(new Vector2(firstItemSlotWorldPos.x + column, firstItemSlotWorldPos.y));
        }

        return returnList;
    }

    public List<Vector2> GetItemMarginDownPos()
    {
        var firstItemSlotWorldPos = new Vector2(Pos.x - 1, -Pos.y + 1);

        List<Vector2> returnList = new List<Vector2>();

        for (int column = 0; column < Size.x; column++)
        {
            returnList.Add(new Vector2(firstItemSlotWorldPos.x + column, firstItemSlotWorldPos.y - Size.y + 1));
        }

        return returnList;
    }

    public List<Vector2> GetItemMarginLeftPos()
    {
        var firstItemSlotWorldPos = new Vector2(Pos.x - 1, -Pos.y + 1);

        List<Vector2> returnList = new List<Vector2>();

        for (int row = 0; row < Size.y; row++)
        {
            var xValue = firstItemSlotWorldPos.x;
            returnList.Add(new Vector2(xValue, firstItemSlotWorldPos.y - row));
        }

        return returnList;
    }

    public List<Vector2> GetItemMarginRightPos()
    {
        var firstItemSlotWorldPos = new Vector2(Pos.x - 1, -Pos.y + 1);

        List<Vector2> returnList = new List<Vector2>();

        for (int row = 0; row < Size.y; row++)
        {
            var xValue = firstItemSlotWorldPos.x + Size.x - 1;
            returnList.Add(new Vector2(xValue, firstItemSlotWorldPos.y - row));
        }

        return returnList;
    }

    public List<Vector2> GetItemSlotListCurrentPos()
    {
        var firstItemSlotWorldPos = new Vector2(Pos.x - 1, -Pos.y + 1);

        List<Vector2> returnList = new List<Vector2>();

        for (int row = 0; row < Size.y; row++)
        {
            for (int column = 0; column < Size.x; column++)
            {
                returnList.Add(new Vector2(firstItemSlotWorldPos.x + column, firstItemSlotWorldPos.y - row));
            }
        }

        return returnList;
    }

    public bool IsFullyInsideInventory(InventoryGridManager inventoryGridManager)
    {
        var itemSlotListCurrentPos = GetItemSlotListCurrentPos();
        var inValidPos = true;
        foreach (var itemSlotPos in itemSlotListCurrentPos)
        {
            if (itemSlotPos.x < 0 || itemSlotPos.x > inventoryGridManager.Width - 1 ||
                itemSlotPos.y > 0 || itemSlotPos.y < -inventoryGridManager.Height + 1)
            {
                inValidPos = false;
            }
        }

        return inValidPos;
    }

    public Item3dModel[] GetOverlappingItems(InventoryGridManager inventoryGridManager)
    {
        var itemSlotListCurrentPos = GetItemSlotListCurrentPos();
        List<Item3dModel> overItemList = new List<Item3dModel>();

        foreach (var itemSlotPos in itemSlotListCurrentPos)
        {
            var itemFromPos = inventoryGridManager.GetItemFromPos(itemSlotPos.x, -itemSlotPos.y);

            if (itemFromPos && !overItemList.Contains(itemFromPos))
            {
                overItemList.Add(itemFromPos);
            }
        }

        return overItemList.ToArray();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(itemMiddlePoint, .2f);
    }
}