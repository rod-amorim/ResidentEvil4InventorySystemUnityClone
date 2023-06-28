using System;
using UnityEngine;

namespace InventoryCursor
{
    public class Cursor : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer cursorSprite;
        [SerializeField] private InventoryGridManager inventoryGridManager;

        private Vector2 test;

        public void UpdateCursorVisual(Vector2 pos)
        {
            var thisCursor = cursorSprite.gameObject;
            thisCursor.transform.localPosition = pos;
        }

        public void UpdateCursorVisual(Vector2 pos, Vector2 size)
        {
            var thisCursor = cursorSprite.gameObject;
            thisCursor.transform.localPosition = pos;
            cursorSprite.size = size;
        }

        public void UpdateCursorVisual(Vector2 pos, Color color)
        {
            var thisCursor = cursorSprite.gameObject;
            thisCursor.transform.localPosition = pos;
            cursorSprite.color = color;
        }

        public void UpdateCursorVisual(Vector2 pos, Vector2 size, Color color)
        {
            var thisCursor = cursorSprite.gameObject;
            thisCursor.transform.localPosition = pos;
            cursorSprite.size = size;
            cursorSprite.color = color;
        }

        public void RotateCursor(Vector2 middlePoint)
        {
            var scale = inventoryGridManager.transform.localScale;

            var cursorTransform = transform;

            cursorSprite.size = new Vector2(cursorSprite.size.y, cursorSprite.size.x);

            var cursorPos = new Vector2(cursorTransform.position.x, cursorTransform.position.y);
            var rotatedMiddlePoint =
                new Vector2(cursorSprite.size.x / 2 * scale.x, -cursorSprite.size.y / 2 * scale.y) + cursorPos;

            var offsetRotMiddlePos = cursorPos - rotatedMiddlePoint;

            var finalPos = middlePoint + offsetRotMiddlePos;

            cursorTransform.position = new Vector2(finalPos.x, finalPos.y);
            cursorTransform.localPosition =
                new Vector3(cursorTransform.localPosition.x, cursorTransform.localPosition.y, 0);
        }

        public Vector2 GetGridPosByCursorPos()
        {
            var pos = transform.localPosition;
            return new Vector2((int) Math.Floor(pos.x), -(int) Math.Round(pos.y));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(test, .2f);
        }
    }
}