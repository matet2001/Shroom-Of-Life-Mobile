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

    private bool isPause;
    
    private void Awake()
    {
        treePositionList = new List<Vector2>();

        ConnectionManager.OnConnectionListInit += ConnectionInit;
        ConnectionManager.OnTreeListChange += AddTreeToList;

        ManagerState.OnManagerStateInit += ManagerState_OnManagerStateInit;
        ManagerState.OnManagerStateEnter += ManagerStateEnter;
        ManagerState.OnManagerStateExit += ManagerStateExit;

        MushroomController.OnMushroomCreate += SetCameraPosition;

        TutorialManager.OnStageReveale += Pause;
        TutorialManager.OnStageHide += Continue;

        LevelSceneManager.OnRestart += ResetCamera; 
    }
    private void ManagerState_OnManagerStateInit(Transform containerTransform)
    {
        cameraContainerTransform = containerTransform;
        isManagerStateActive = true;
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
    
        SimpleHorizontalMovement();
    }
    private void ManagerStateEnter()
    {
        isManagerStateActive = true;
    }
    private void ManagerStateExit(Transform containerTransform)
    {
        isManagerStateActive = false;
    }
    private void ConnectionInit(List<TreeController> treeList, List<MushroomController> mushroomList)
    {
        treePositionList.Clear();
        if (treeList.Count != 0) treeList.ForEach(tree => AddTreeToList(tree));
    }
    private void AddTreeToList(TreeController treeController)
    {
        treePositionList.Add(treeController.transform.position);
    }
    #region Simple Camera Movemement
    [SerializeField] float cameraRotationSpeed;
   
    private void SimpleHorizontalMovement()
    {
        Vector2 input = MoveInputUI.Instance.GetInputVector();
        Vector2 horizontalInput = new Vector2(input.x, 0f);
        Vector3 eulerAdd = new Vector3(0f, 0f, horizontalInput.x) * cameraRotationSpeed/10f;

        cameraContainerTransform.transform.localEulerAngles += -eulerAdd * Time.deltaTime;
    }
    private void ResetCamera()
    {
        cameraContainerTransform.localEulerAngles = Vector3.zero; 
    }
    public void SetCameraPosition(Vector2 position)
    {
        Vector2 containerToPositionVector = position - (Vector2)cameraContainerTransform.position;
        float containerToPositionAngle = Vector2.SignedAngle(Vector2.up, containerToPositionVector.normalized);
        Vector3 newEuler = new Vector3(0f, 0f, containerToPositionAngle);
        cameraContainerTransform.localEulerAngles = newEuler;
    }
    #endregion
    private List<Vector2> treePositionList;
    [SerializeField] float minimumAngle;
}
