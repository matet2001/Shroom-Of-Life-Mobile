using StateManagment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerCameraDistanceChecker : MonoBehaviour
{
    private List<TreeUIManager> treeUIList = new List<TreeUIManager>();
    private Transform cameraContainerTransform;
    private bool isManagerStateActive;

    private void Awake()
    {      
        SubscribeToEvents();
    }
    private void SubscribeToEvents()
    {
        ConnectionManager.OnTreeListChange += ConnectionManager_OnTreeListChange;

        ManagerState.OnManagerStateInit += delegate (Transform containerTransform) { cameraContainerTransform = containerTransform; };
        ManagerState.OnManagerStateEnter += delegate () { isManagerStateActive = true; };
        ManagerState.OnManagerStateExit += delegate (Transform containerTransform) { isManagerStateActive = false; };
    }
    private void Update()
    {
        CheckDistanceFromTrees();
    }
    private void ConnectionManager_OnTreeListChange(TreeController treeController)
    {
         treeUIList.Add(treeController.GetTreeUIManager());
    }
    [SerializeField] float treeUIShowAngleDifference;
    
    public void CheckDistanceFromTrees()
    {
        if (!isManagerStateActive) return;
        if (treeUIList.Count == 0) return;

        float containerToCameraAngle = Vector2.SignedAngle(cameraContainerTransform.up, cameraContainerTransform.up);
        
        foreach (TreeUIManager treeUI in treeUIList)
        {
            Vector2 containerToTreeVector = treeUI.transform.position - cameraContainerTransform.position;
            float containerToTreeAngle = Vector2.SignedAngle(containerToTreeVector, cameraContainerTransform.up);
            bool shouldShowUI = Mathf.Abs(containerToCameraAngle - containerToTreeAngle) < treeUIShowAngleDifference;

            if (treeUI.isUIVisible == shouldShowUI) continue;
            treeUI.SetUIActive(shouldShowUI);
        }
    }
}
