using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture;
    [SerializeField] Vector2 cursorHotSpot;
    private void Awake()
    {
        SetCursor();
    }
    [Button("Reset Cursor")]
    private void SetCursor()
    {
        Cursor.SetCursor(cursorTexture, cursorHotSpot, CursorMode.Auto);
    }
    private void OnDrawGizmos()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(mousePos, .1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(mousePos + cursorHotSpot, .1f);
    }
}
