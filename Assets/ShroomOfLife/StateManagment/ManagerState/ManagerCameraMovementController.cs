using StateManagment;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManagerCameraMovementController : MonoBehaviour
{
    private Transform cameraContainerTransform;
    private bool isManagerStateActive;
    
    private void Awake()
    {
        treePositionList = new List<Vector2>();
        
        ConnectionManager.OnTreeListChange += ResetTreeData;

        ManagerState.OnManagerStateInit += ManagerState_OnManagerStateInit;
        ManagerState.OnManagerStateEnter += ManagerStateEnter;
        ManagerState.OnManagerStateExit += ManagerStateExit;
    }
    private void ManagerState_OnManagerStateInit(Transform containerTransform)
    {
        cameraContainerTransform = containerTransform;
    }
    private void Update()
    {
        MoveCamera();
    }
    private void MoveCamera()
    {
        if (!isManagerStateActive) return;
        
        if (InputManager.IsShiftHold()) JumpHorizontalMovement();
        else SimpleHorizontalMovement();
    }
    private void ManagerStateEnter()
    {
        isManagerStateActive = true;
        ResetCameraSpeed();
    }
    private void ManagerStateExit(Transform containerTransform)
    {
        isManagerStateActive = false;
        ResetCamera();
    }
    #region Simple Camera Movemement
    [Header("Camera Rotation")]
    [SerializeField] float cameraRotationSpeedMin;
    [SerializeField] float cameraRotationSpeedMax;
    [Header("Speed Incrase")]
    [SerializeField] float speedIncreaseTimeMax;
    private float speedIncreaseTimer;
    [SerializeField] float cameraRotationSpeed;
    [Header("Speed Reset")]
    [SerializeField] float cameraSpeedResetTimeMax;
    private float cameraSpeedResetTimer;
   
    private void SimpleHorizontalMovement()
    {
        float horizontalInput = InputManager.GetHorizontalAxis();
        float containerCurrentZRotation = cameraContainerTransform.localEulerAngles.z;
        containerCurrentZRotation += horizontalInput * Time.deltaTime * cameraRotationSpeed * -1f;

        cameraContainerTransform.localEulerAngles = new Vector3(0f, 0f, containerCurrentZRotation);

        if (horizontalInput != 0) IncreaseSpeedOverTime();
        else ResetCameraSpeed();
    }
    private void IncreaseSpeedOverTime()
    {
        cameraSpeedResetTimer = 0;

        if (speedIncreaseTimer < speedIncreaseTimeMax)
        {
            float t = speedIncreaseTimer / speedIncreaseTimeMax;
            t = t * t * (3f - 2f * t);

            speedIncreaseTimer += Time.deltaTime;
            cameraRotationSpeed = Mathf.Lerp(cameraRotationSpeedMin, cameraRotationSpeedMax, t);
        }
    }
    private void ResetCameraSpeed()
    {
        if (cameraSpeedResetTimeMax <= cameraSpeedResetTimer)
        {
            cameraRotationSpeed = cameraRotationSpeedMin;
            speedIncreaseTimer = 0f;
        }
        else cameraSpeedResetTimer += Time.deltaTime;
    }
    private void ResetCamera()
    {
        cameraContainerTransform.localEulerAngles = Vector3.zero; 
    }
    #endregion
    #region Jump Horizontal Movement
    private List<Vector2> treePositionList;
    [SerializeField] float minimumAngle;
    
    private void JumpHorizontalMovement()
    {
        if (treePositionList.Count == 0) return;

        float inputDirection = 0;

        if (InputManager.IsLeftMovementKeyPressed()) inputDirection = 1;
        if (InputManager.IsRightMovementKeyPressed()) inputDirection = -1;

        if (inputDirection == 0) return;
      
        Vector2 containerUpVector = cameraContainerTransform.up;
       
        float closestAngle = 360f;

        foreach (Vector2 treePosition in treePositionList)
        {
            Vector2 containerToTreeVector = treePosition - (Vector2)cameraContainerTransform.position;
            float containerToTreeAngle = Vector2.SignedAngle(containerUpVector, containerToTreeVector.normalized);

            if (Mathf.Sign(containerToTreeAngle) != Mathf.Sign(inputDirection)) continue;
            if (Mathf.Abs(containerToTreeAngle) > Mathf.Abs(closestAngle)) continue;
            if (Mathf.Abs(containerToTreeAngle) < minimumAngle) continue;

            closestAngle = containerToTreeAngle;
        }

        cameraContainerTransform.transform.localEulerAngles += new Vector3(0f, 0f, closestAngle);
    }
    private void ResetTreeData(TreeController treeController)
    {
        treePositionList.Add(treeController.transform.position);
    }
    #endregion
}
