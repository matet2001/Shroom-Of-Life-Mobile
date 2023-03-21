using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnCrosshairController : MonoBehaviour
{
    [SerializeField] YarnMovementController yarnMovementController;
    
    private void Update()
    {
        SetPosition();
        //ManageYarnInArc();
    }
    private void SetPosition()
    {
        transform.position = InputManager.GetMouseWorldPosition();
    }
    #region Arc Move
    public static event Action<Vector2, float> OnYarnCrosshairEnter;
    public static event Action<Vector2> OnYarnCrosshairExit;

    [SerializeField] float arcDistace;
    private bool isYarnInArc;
    private Vector2 cursorPositionOld;
    [SerializeField] float cursorMovingTimer;
    [SerializeField] float cursorMovingTimerMax;
    [SerializeField] bool isCursorMoving = true;
    
    private void ManageYarnInArc()
    {
        SetIsCursorMoving();
        bool isCloser = IsPositionCloserThanArc(yarnMovementController.transform.position);

        if (isCloser && !isYarnInArc && !isCursorMoving)
        {
            isYarnInArc = true;
            //Switch to stop camera
            OnYarnCrosshairEnter?.Invoke(InputManager.GetMouseWorldPosition(), arcDistace);
        }
        
        if (isYarnInArc && (!isCloser || isCursorMoving))
        {
            isYarnInArc = false;
            //Switch to follow camera
            OnYarnCrosshairExit?.Invoke(yarnMovementController.transform.position);
        }
    }
    private bool SetIsCursorMoving()
    {
        Vector2 currentCursorPosition = InputManager.GetMouseScreenPosition();
        bool isCursorMovingNew = currentCursorPosition != cursorPositionOld;

        if (isCursorMovingNew != isCursorMoving)
        {
            if (cursorMovingTimer == 0) cursorMovingTimer = cursorMovingTimerMax;

            if (cursorMovingTimer > 0) cursorMovingTimer -= Time.deltaTime;
            else
            {
                cursorMovingTimer = 0f;
                isCursorMoving = isCursorMovingNew;
            }
        }
        
        cursorPositionOld = currentCursorPosition;
        return isCursorMoving;
    }
    public bool IsPositionCloserThanArc(Vector2 position)
    {
        return Vector2.Distance(transform.position, position) < arcDistace;
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(transform.position, arcDistace);
    //}
    #endregion
}
