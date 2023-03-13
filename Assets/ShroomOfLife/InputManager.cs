using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Texture2D cursorSprite;

    private void Start()
    {
        cursorSprite = Resources.Load("Cursor") as Texture2D;
        Cursor.SetCursor(cursorSprite, Vector2.zero, CursorMode.Auto);
    }
    public static Vector2 GetMouseWorldPosition()
    {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(vec.x, vec.y);
    }
    public static Vector2 GetMouseScreenPosition()
    {
        return Input.mousePosition;
    }
    public static bool IsMouseRightClick() => Input.GetMouseButton(1);
    public static bool IsMouseLeftClick() => Input.GetMouseButton(0);
    public static float GetHorizontalAxis() => Input.GetAxis("Horizontal");
    public static bool IsShiftHold() => Input.GetKey(KeyCode.LeftShift);
    public static bool IsRightMovementKeyPressed() => Input.GetKeyDown(KeyCode.D);
    public static bool IsLeftMovementKeyPressed() => Input.GetKeyDown(KeyCode.A);
}
