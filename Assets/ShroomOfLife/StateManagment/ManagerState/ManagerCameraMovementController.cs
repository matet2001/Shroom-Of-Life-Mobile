using StateManagment;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManagerCameraMovementController : MonoBehaviour
{
    private Transform cameraContainerTransform;
    private bool isManagerStateActive;

    [SerializeField] float doubleClickdelayTime;
    private bool isDoubleClick;
    private Coroutine DoubleClickCoroutine;

    private bool isPause;
    
    private void Awake()
    {
        treePositionList = new List<Vector2>();

        ConnectionManager.OnConnectionListInit += ConnectionInit;
        ConnectionManager.OnTreeListChange += AddTreeToList;

        ManagerState.OnManagerStateInit += ManagerState_OnManagerStateInit;
        ManagerState.OnManagerStateEnter += ManagerStateEnter;
        ManagerState.OnManagerStateExit += ManagerStateExit;

        TutorialManager.OnStageReveale += Pause;
        TutorialManager.OnStageHide += delegate (GameObject gm) { Continue(); };
    }
    private void ManagerState_OnManagerStateInit(Transform containerTransform)
    {
        cameraContainerTransform = containerTransform;
    }
    private void Pause() => isPause = true;
    private void Continue() => isPause = false;
    private void Update()
    {
        MoveCamera();
    }
    private void MoveCamera()
    {
        if (!isManagerStateActive) return;
        if (isPause) return;

        if (ShouldUseJumpMovement()) JumpHorizontalMovement();
        else SimpleHorizontalMovement();
    }
    private bool ShouldUseJumpMovement()
    {
        bool isClick = InputManager.IsMouseLeftClickPressed();

        if (isDoubleClick && isClick)
        {
            isDoubleClick = false;
            StopCoroutine(DoubleClickCoroutine);
            return true;
        } 
        if(!isDoubleClick && isClick)
        {
            isDoubleClick = true;
            DoubleClickCoroutine = StartCoroutine(DoubleClickDelay());
        }

        return InputManager.IsShiftHold();
    }
    private IEnumerator DoubleClickDelay()
    {
        yield return new WaitForSeconds(doubleClickdelayTime);
        isDoubleClick = false;
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
        float horizontalInput = GetHorizontalInput();

        float containerCurrentZRotation = cameraContainerTransform.localEulerAngles.z;
        containerCurrentZRotation += horizontalInput * Time.deltaTime * cameraRotationSpeed * -1f;

        cameraContainerTransform.localEulerAngles = new Vector3(0f, 0f, containerCurrentZRotation);

        if (horizontalInput != 0) IncreaseSpeedOverTime();
        else ResetCameraSpeed();
    }
    private static float GetHorizontalInput()
    {
        bool isClick = InputManager.IsMouseLeftClick();
        if (isClick)
        {
            Vector2 mousePos = InputManager.GetMouseScreenPosition();
            float halfWidth = Screen.width / 2f;

            if (mousePos.x < halfWidth - halfWidth / 3 * 1) return -1f;
            if (mousePos.x > halfWidth + halfWidth / 3 * 1) return 1f;
        }

        return InputManager.GetHorizontalAxis();
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

        float inputDirection = GetHorizontalPressInput();

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
    private static float GetHorizontalPressInput()
    {
        bool isClick = InputManager.IsMouseLeftClickPressed();

        if (isClick)
        {
            Vector2 mousePos = InputManager.GetMouseScreenPosition();
            float halfWidth = Screen.width / 2f;

            if (mousePos.x < halfWidth - halfWidth / 3 * 1) return 1f;
            if (mousePos.x > halfWidth + halfWidth / 3 * 1) return -1f;
        }

        float horizontalInput = 0f;

        if (InputManager.IsRightMovementKeyPressed()) horizontalInput = -1f;
        if (InputManager.IsLeftMovementKeyPressed()) horizontalInput = 1f;

        return horizontalInput;
    }
    private void ConnectionInit(List<TreeController> treeList, List<MushroomController> mushroomList)
    {
        if (treeList.Count != 0) treeList.ForEach(tree => AddTreeToList(tree));
    }
    private void AddTreeToList(TreeController treeController)
    {
        treePositionList.Add(treeController.transform.position);
    }

    #endregion
}
