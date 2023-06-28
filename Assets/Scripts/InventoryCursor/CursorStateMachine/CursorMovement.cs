using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace InventoryCursor.CursorStateMachine
{
    public class CursorMovement : MonoBehaviour
    {
        [Header("Properties")] [SerializeField]
        protected internal Vector2 pos = Vector2.zero;

        [SerializeField] protected internal InventoryGridManager inventoryGridManager;
        [SerializeField] protected internal CursorStateMachine cursorStateMachine;
        [SerializeField] protected internal Cursor cursor;
        [Header("Visual")] [SerializeField] protected Color defaultColor;
        [SerializeField] protected Color holdItemColor;

        private Item3dModel _holdItem;

        private bool _upPressed;
        private bool _downPressed;
        private bool _leftPressed;
        private bool _rightPressed;
        private bool _actionPressed;
        private bool _rotatePressed;

        private void Start()
        {
            inventoryGridManager = GetComponentInParent<InventoryGridManager>();
        }

        public void DrawCursor()
        {
            if (!cursorStateMachine.InHoldingItemState())
            {
                var item3dModel = inventoryGridManager.GetItemFromPos(pos.x, pos.y);

                if (item3dModel)
                {
                    cursor.UpdateCursorVisual(new Vector2(item3dModel.Pos.x - 1, -item3dModel.Pos.y + 1),
                        item3dModel.Size);
                }
                else
                {
                    cursor.UpdateCursorVisual(new Vector2(pos.x, -pos.y), new Vector2(1, 1));
                }
            }
            else
            {
                cursor.UpdateCursorVisual(new Vector2(_holdItem.Pos.x - 1, -_holdItem.Pos.y + 1));
                _holdItem.UpdateItemMiddlePoint();
                _holdItem.SetItemPosByGridPos();
            }
        }

        public void OnHoldItem(Item3dModel item3dModel)
        {
            _holdItem = item3dModel;

            pos = new Vector2(item3dModel.Pos.x - 1, item3dModel.Pos.y - 1);

            cursor.UpdateCursorVisual(new Vector2(_holdItem.Pos.x - 1, -_holdItem.Pos.y + 1),
                item3dModel.Size, holdItemColor);

            item3dModel.OnItemHold();

            //Clear actual item from grid
            foreach (var itemSlotListCurrentPo in item3dModel.GetItemSlotListCurrentPos())
            {
                inventoryGridManager.Grid.GridArray[(int) itemSlotListCurrentPo.x, -(int) itemSlotListCurrentPo.y] =
                    null;
            }
            
            _holdItem.UpdateItemMiddlePoint();
        }

        public bool OnTryReleaseItem()
        {
            if (!_holdItem.IsFullyInsideInventory(inventoryGridManager))
                return false;

            var currentOverlapItems = _holdItem.GetOverlappingItems(inventoryGridManager);
            if (currentOverlapItems.Length > 1)
                return false;

            if (currentOverlapItems.Length == 1)
            {
                SwitchSelectedItem(currentOverlapItems[0]);
                return false;
            }

            var itemMostMiddleSlot = _holdItem.GetItemMostMiddleSlot();

            pos = new Vector2(itemMostMiddleSlot.x, -itemMostMiddleSlot.y);

            cursor.UpdateCursorVisual(itemMostMiddleSlot, defaultColor);

            _holdItem.OnItemRelease();

            //set actual item on grid
            foreach (var itemSlotListCurrentPo in _holdItem.GetItemSlotListCurrentPos())
            {
                inventoryGridManager.Grid.GridArray[(int) itemSlotListCurrentPo.x, -(int) itemSlotListCurrentPo.y] =
                    _holdItem;
            }

            DrawCursor();
            _holdItem = null;

            return true;
        }

        public void RotateItem(float value)
        {
            //rotate item 3d
            _holdItem.OnRotateItem(value);

            cursor.RotateCursor(_holdItem.ItemMiddlePoint);

            pos = cursor.GetGridPosByCursorPos();
            _holdItem.Pos = new Vector2(pos.x + 1, pos.y + 1);
        }

        private bool InOffsetPos()
        {
            return cursor.transform.localPosition.x.ToString(CultureInfo.InvariantCulture).Contains('.');
        }

        private void SwitchSelectedItem(Item3dModel currentOverlapItem)
        {
            _holdItem.UpdateItemMiddlePoint();
            _holdItem.SetItemPosByGridPos();

            //Keeps reference to the old selected item
            var oldItem = _holdItem;

            // put selected item on defaut pos
            _holdItem.OnItemRelease();

            //Hold the new item
            OnHoldItem(currentOverlapItem);

            //set old item on grid (this is made after so the act of getting the actual item do not erase the item from the switched item)
            foreach (var itemSlotListCurrentPo in oldItem.GetItemSlotListCurrentPos())
            {
                inventoryGridManager.Grid.GridArray[(int) itemSlotListCurrentPo.x, -(int) itemSlotListCurrentPo.y] =
                    oldItem;
            }
        }

        private void CheckIfTheItemIsOutsideOfTheGridAndAdjustToBeOnTheBorder()
        {
            //is up the grid
            if (_holdItem.Pos.y < 1)
            {
                pos = new Vector2(pos.x, 0);
                _holdItem.Pos = new Vector2(_holdItem.Pos.x, 0);
            }

            if (_holdItem.Pos.x < 1)
            {
                pos = new Vector2(0, pos.y);
                _holdItem.Pos = new Vector2(0, _holdItem.Pos.y);
            }

            if (_holdItem.Pos.y + _holdItem.Size.y > inventoryGridManager.Height)
            {
                pos = new Vector2(pos.x, inventoryGridManager.Height+ 2 - _holdItem.Size.y);
                _holdItem.Pos = new Vector2(_holdItem.Pos.x, inventoryGridManager.Height + 2 - _holdItem.Size.y);
            }

            if (_holdItem.Pos.x + _holdItem.Size.x > inventoryGridManager.Width)
            {
                pos = new Vector2(inventoryGridManager.Width + 2 - _holdItem.Size.x, pos.y);
                _holdItem.Pos = new Vector2(inventoryGridManager.Width + 2 - _holdItem.Size.x, _holdItem.Pos.y);
            }
        }

        public void MoveCursor(Vector2 movement)
        {
            // Up
            if (movement.y > 0 && !_upPressed)
            {
                if (cursorStateMachine.InHoldingItemState())
                {
                    MoveItemUp();
                    _upPressed = true;
                }
                else
                    MoveUp();
            }
            else if (movement.y == 0)
            {
                _upPressed = false;
            }

            // Down
            if (movement.y < 0 && !_downPressed)
            {
                if (cursorStateMachine.InHoldingItemState())
                {
                    MoveItemDown();
                    _downPressed = true;
                }
                else
                    MoveDown();
            }
            else if (movement.y == 0)
            {
                _downPressed = false;
            }

            // Right
            if (movement.x > 0 && !_rightPressed)
            {
                if (cursorStateMachine.InHoldingItemState())
                {
                    MoveItemRight();
                    _rightPressed = true;
                }
                else
                    MoveRight();
            }
            else if (movement.x == 0)
            {
                _rightPressed = false;
            }

            // Left
            if (movement.x < 0 && !_leftPressed)
            {
                if (cursorStateMachine.InHoldingItemState())
                {
                    MoveItemLeft();
                    _leftPressed = true;
                }
                else
                    MoveLeft();
            }
            else if (movement.x == 0)
            {
                _leftPressed = false;
            }
        }

        private void MoveUp()
        {
            Item3dModel currentSlotItem = inventoryGridManager.GetItemFromPos(pos.x, pos.y);

            pos.y = (pos.y - 1 + inventoryGridManager.Height) % inventoryGridManager.Height;

            Item3dModel nextSlotItem = inventoryGridManager.GetItemFromPos(pos.x, pos.y);

            if (currentSlotItem && nextSlotItem && currentSlotItem.Equals(nextSlotItem))
            {
                MoveUp();
                return;
            }

            _upPressed = true;
            DrawCursor();
        }

        private void MoveDown()
        {
            Item3dModel currentSlotItem = inventoryGridManager.GetItemFromPos(pos.x, pos.y);

            pos.y = (pos.y + 1) % inventoryGridManager.Height;

            Item3dModel nextSlotItem = inventoryGridManager.GetItemFromPos(pos.x, pos.y);

            if (currentSlotItem && nextSlotItem && currentSlotItem.Equals(nextSlotItem))
            {
                MoveDown();
                return;
            }

            _downPressed = true;
            DrawCursor();
        }

        private void MoveRight()
        {
            Item3dModel currentSlotItem = inventoryGridManager.GetItemFromPos(pos.x, pos.y);

            pos.x = (pos.x + 1) % inventoryGridManager.Width;

            Item3dModel nextSlotItem = inventoryGridManager.GetItemFromPos(pos.x, pos.y);

            if (currentSlotItem && nextSlotItem && currentSlotItem.Equals(nextSlotItem))
            {
                MoveRight();
                return;
            }

            _rightPressed = true;
            DrawCursor();
        }

        private void MoveLeft()
        {
            Item3dModel currentSlotItem = inventoryGridManager.GetItemFromPos(pos.x, pos.y);

            pos.x = (pos.x - 1 + inventoryGridManager.Width) %
                    inventoryGridManager.Width;

            Item3dModel nextSlotItem = inventoryGridManager.GetItemFromPos(pos.x, pos.y);

            if (currentSlotItem && nextSlotItem && currentSlotItem.Equals(nextSlotItem))
            {
                MoveLeft();
                return;
            }

            _leftPressed = true;
            DrawCursor();
        }

        private void MoveItemUp()
        {
            if (InOffsetPos())
            {
                CheckIfTheItemIsOutsideOfTheGridAndAdjustToBeOnTheBorder();
                DrawCursor();
                return;
            }

            var marginTopList = _holdItem.GetItemMarginUpPos();

            bool inPosToWrapAround = true;
            foreach (var pos in marginTopList)
            {
                if (pos.y <= 0)
                {
                    inPosToWrapAround = false;
                }
            }

            if (inPosToWrapAround)
            {
                var wrapPos = inventoryGridManager.Height + 2 - _holdItem.Size.y;
                pos = new Vector2(pos.x, pos.y + wrapPos);
                _holdItem.Pos = new Vector2(_holdItem.Pos.x, _holdItem.Pos.y + wrapPos);
                DrawCursor();
                return;
            }

            pos = new Vector2(pos.x, pos.y - 1);
            _holdItem.Pos = new Vector2(_holdItem.Pos.x, _holdItem.Pos.y - 1);
            DrawCursor();
        }

        private void MoveItemDown()
        {
            if (InOffsetPos())
            {
                CheckIfTheItemIsOutsideOfTheGridAndAdjustToBeOnTheBorder();
                DrawCursor();
                return;
            }

            var marginDownList = _holdItem.GetItemMarginDownPos();

            bool inPosToWrapAround = true;
            foreach (var pos in marginDownList)
            {
                if (pos.y > -inventoryGridManager.Height)
                {
                    inPosToWrapAround = false;
                }
            }

            if (inPosToWrapAround)
            {
                pos = new Vector2(pos.x, 0);
                _holdItem.Pos = new Vector2(_holdItem.Pos.x, 0);
                DrawCursor();
                return;
            }

            pos = new Vector2(pos.x, pos.y + 1);
            _holdItem.Pos = new Vector2(_holdItem.Pos.x, _holdItem.Pos.y + 1);
            DrawCursor();
        }

        private void MoveItemLeft()
        {
            if (InOffsetPos())
            {
                CheckIfTheItemIsOutsideOfTheGridAndAdjustToBeOnTheBorder();
                DrawCursor();
                return;
            }

            var marginLeftList = _holdItem.GetItemMarginLeftPos();

            bool inPosToWrapAround = true;
            foreach (var pos in marginLeftList)
            {
                if (pos.x >= 0)
                {
                    inPosToWrapAround = false;
                }
            }

            if (inPosToWrapAround)
            {
                var wrapTarget = inventoryGridManager.Width + 2 - _holdItem.Size.x;
                pos = new Vector2(pos.x + wrapTarget, pos.y);
                _holdItem.Pos = new Vector2(_holdItem.Pos.x + wrapTarget, _holdItem.Pos.y);
                DrawCursor();
                return;
            }

            pos = new Vector2(pos.x - 1, pos.y);
            _holdItem.Pos = new Vector2(_holdItem.Pos.x - 1, _holdItem.Pos.y);
            DrawCursor();
        }

        private void MoveItemRight()
        {
            if (InOffsetPos())
            {
                CheckIfTheItemIsOutsideOfTheGridAndAdjustToBeOnTheBorder();
                DrawCursor();
                return;
            }

            var marginRightList = _holdItem.GetItemMarginRightPos();

            bool inPosToWrapAround = true;
            foreach (var pos in marginRightList)
            {
                if (pos.x < inventoryGridManager.Width)
                {
                    inPosToWrapAround = false;
                }
            }

            if (inPosToWrapAround)
            {
                pos = new Vector2(0, pos.y);
                _holdItem.Pos = new Vector2(0, _holdItem.Pos.y);
                DrawCursor();
                return;
            }

            pos = new Vector2(pos.x + 1, pos.y);
            _holdItem.Pos = new Vector2(_holdItem.Pos.x + 1, _holdItem.Pos.y);
            DrawCursor();
        }

        public Item3dModel GetItemActualPos()
        {
            return inventoryGridManager.GetItemFromPos(pos.x, pos.y);
        }
    }
}