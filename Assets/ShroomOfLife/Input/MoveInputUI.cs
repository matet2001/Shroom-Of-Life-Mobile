using StateManagment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInputUI : MonoBehaviour
{
    public static MoveInputUI Instance { get; private set; }

    [SerializeField] RectTransform cursorTransform, innerOutlineTransform, outerOutlineTransform;
    [SerializeField] float range;

    private bool isHold;

    private void OnValidate()
    {
        SetOutlineSizes();
    }
    private void SetOutlineSizes()
    {
        if (innerOutlineTransform)
        {
            innerOutlineTransform.localScale = Vector3.one * range / 100f;
        }
        if (outerOutlineTransform)
        {
            outerOutlineTransform.localScale = Vector3.one * range / 100f * 2f;
        }
    }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        LoseState.OnLoseGame += HideUI;
        WinState.OnWinGame += HideUI;
        LevelSceneManager.OnRestart += ShowUI;
    }
    
    private void Update()
    {
        SetCursorBasedOnInput();
    }
    private void SetCursorBasedOnInput()
    {
        Vector2 mouseVector = GetMouseVector();
        SetIsHold(mouseVector);

        if (!isHold)
        {
            ResetCursorPos();
            return;
        }

        Vector2 cursorPlacementVector = GetCursorPlacementVector(mouseVector);

        cursorTransform.localPosition = cursorPlacementVector;
    } 
    private void ResetCursorPos()
    {
        cursorTransform.localPosition = Vector2.zero;
    }
    private Vector2 GetMouseVector()
    {
        Vector3 mousePos = InputManager.GetMouseScreenPosition();
        Vector2 mouseVector = mousePos - transform.position;
        return mouseVector;
    }
    private void SetIsHold(Vector2 mouseVector)
    {
        if (!isHold && InputManager.IsMouseLeftClick() && mouseVector.magnitude < range * 2f)
        {
            bool isMouseOnUI = RaycastUtilities.PointerIsOverUI("Arrow");
            if (isMouseOnUI) return;
            isHold = true;
        }
        if (isHold && !InputManager.IsMouseLeftClick()) isHold = false;
    }
    private Vector2 GetCursorPlacementVector(Vector2 mouseVector)
    {
        Vector2 mouseVectorNormalized = mouseVector.normalized;
        float vectorMagnitude = mouseVector.magnitude / (range / (range * 0.8f));
        float vectorMagnitudeClamped = Mathf.Clamp(vectorMagnitude, 0, range);
        Vector2 cursorPlacementVector = mouseVectorNormalized * vectorMagnitudeClamped;
        return cursorPlacementVector;
    }
    public Vector2 GetInputVector()
    {
        return cursorTransform.localPosition;
    }
    public float GetRange() => range;
    private void ShowUI()
    {
        cursorTransform.gameObject.SetActive(true);
        innerOutlineTransform.gameObject.SetActive(true);
        outerOutlineTransform.gameObject.SetActive(true);
    }
    private void HideUI()
    {
        cursorTransform.gameObject.SetActive(false);
        innerOutlineTransform.gameObject.SetActive(false);
        outerOutlineTransform.gameObject.SetActive(false);
    }
}
