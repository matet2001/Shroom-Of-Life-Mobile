using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManagerCameraMovementController : MonoBehaviour
{
    public Transform cameraContainerTransform { private get; set; }
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

    private List<Vector2> treePositionList;
    public Transform cameraTransform { private get; set; }
    [SerializeField] float minimumAngle;

    private void Awake()
    {
        treePositionList = new List<Vector2>();
        ConnectionManager.OnTreeListChange += ConnectionManager_OnTreeListChange;
    }
    private void ConnectionManager_OnTreeListChange(List<TreeController> treeList)
    {
        treePositionList = new List<Vector2>();
        treeList.ForEach(x => treePositionList.Add(x.transform.position));
    }
    public void MoveCamera()
    {
        if (InputManager.IsShiftHold()) JumpHorizontalMovement();
        else SimpleHorizontalMovement();
    }
    #region SimpleCameraMovemement
    private void SimpleHorizontalMovement()
    {
        float horizontalInput = InputManager.GetHorizontalAxis();
        float containerCurrentZRotation = cameraContainerTransform.transform.localEulerAngles.z;
        containerCurrentZRotation += horizontalInput * Time.deltaTime * cameraRotationSpeed * -1f;

        cameraContainerTransform.transform.localEulerAngles = new Vector3(0f, 0f, containerCurrentZRotation);

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
    public void ResetCameraSpeed()
    {
        if (cameraSpeedResetTimeMax <= cameraSpeedResetTimer)
        {
            cameraRotationSpeed = cameraRotationSpeedMin;
            speedIncreaseTimer = 0f;
        }
        else cameraSpeedResetTimer += Time.deltaTime;
    }
    public void ResetCamera()
    {
        cameraContainerTransform.transform.localEulerAngles = Vector3.zero;
    }
    #endregion
    #region Jump Horizontal Movement
    private void JumpHorizontalMovement()
    {
        if (treePositionList.Count == 0) return;

        float inputDirection = 0;

        if (InputManager.IsLeftMovementKeyPressed()) inputDirection = 1;
        if (InputManager.IsRightMovementKeyPressed()) inputDirection = -1;

        if (inputDirection == 0) return;
      
        Vector2 containerForwardVector = cameraTransform.position - cameraContainerTransform.position;
        float closestAngle = 360f;

        foreach (Vector2 treePosition in treePositionList)
        {
            Vector2 containerToTreeVector = treePosition - (Vector2)cameraContainerTransform.position;
            float containerToTreeAngle = Vector2.SignedAngle(containerForwardVector.normalized, containerToTreeVector.normalized);

            if (Mathf.Sign(containerToTreeAngle) != Mathf.Sign(inputDirection)) continue;
            if (Mathf.Abs(containerToTreeAngle) > Mathf.Abs(closestAngle)) continue;
            if (Mathf.Abs(containerToTreeAngle) < minimumAngle) continue;

            closestAngle = containerToTreeAngle;
        }

        cameraContainerTransform.transform.localEulerAngles += new Vector3(0f, 0f, closestAngle);
    }
    #endregion
}
