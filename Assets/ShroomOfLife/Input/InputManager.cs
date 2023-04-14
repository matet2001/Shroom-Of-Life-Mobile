using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    private Texture2D cursorSprite;

    private void Start()
    {
        cursorSprite = Resources.Load("Cursor") as Texture2D;
        Cursor.SetCursor(cursorSprite, Vector2.zero, CursorMode.Auto);
    }
    #region Mouse Input
    public static Vector2 GetMouseWorldPosition()
    {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(vec.x, vec.y);
    }
    public static Vector2 GetMouseScreenPosition()
    {
        return Input.mousePosition;
    }
    public static bool IsMouseLeftClick() => Input.GetMouseButton(0);
    public static bool IsMouseLeftClickPressed() => Input.GetMouseButtonDown(0);
    public static bool IsMouseRightClick() => Input.GetMouseButton(1);
    public static bool IsMouseRightClickPressed() => Input.GetMouseButtonDown(1);   
    #endregion
    #region Keyboard Input
    public static float GetHorizontalAxis() => Input.GetAxis("Horizontal");
    public static bool IsShiftHold() => Input.GetKey(KeyCode.LeftShift);
    public static bool IsRightMovementKeyPressed() => Input.GetKeyDown(KeyCode.D);
    public static bool IsLeftMovementKeyPressed() => Input.GetKeyDown(KeyCode.A);
    #endregion
}
public static class RaycastUtilities
{
    public static bool PointerIsOverUI(string layerName)
    {
        Vector2 screenPos = InputManager.GetMouseScreenPosition();
        var hitObject = UIRaycast(ScreenPosToPointerData(screenPos));
        return hitObject != null && hitObject.layer == LayerMask.NameToLayer(layerName);
    }

    static GameObject UIRaycast(PointerEventData pointerData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count < 1 ? null : results[0].gameObject;
    }

    static PointerEventData ScreenPosToPointerData(Vector2 screenPos)
       => new(EventSystem.current) { position = screenPos };
}
