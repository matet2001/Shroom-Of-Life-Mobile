using StateManagment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerCameraDistanceChecker : MonoBehaviour
{
    private List<TreeUIManager> treeUIList = new List<TreeUIManager>();
    private List<MushroomController> mushroomList = new List<MushroomController>();

    private Transform cameraContainerTransform;
    private bool isManagerStateActive;

    private void Awake()
    {      
        SubscribeToEvents();
    }
    private void SubscribeToEvents()
    {
        ConnectionManager.OnConnectionListInit += ConnectionManager_OnConnectionListInit;
        ConnectionManager.OnTreeListChange += AddToTreeList;
        ConnectionManager.OnMushroomListChange += AddToMushroomList;

        ManagerState.OnManagerStateInit += delegate (Transform containerTransform) { cameraContainerTransform = containerTransform; };
        ManagerState.OnManagerStateEnter += delegate () { isManagerStateActive = true; };
        ManagerState.OnManagerStateExit += delegate (Transform containerTransform) { isManagerStateActive = false; };
    }
    private void Update()
    {
        CheckDistanceFromTrees();
        CheckDistanceFromMushrooms();
    }
    private void ConnectionManager_OnConnectionListInit(List<TreeController> treeList, List<MushroomController> mushroomList)
    {
        treeList.ForEach(tree => AddToTreeList(tree));
        mushroomList.ForEach(mushroom => AddToMushroomList(mushroom));
    }
    private void AddToTreeList(TreeController treeController)
    {
         treeUIList.Add(treeController.GetTreeUIManager());
    }
    private void AddToMushroomList(MushroomController mushroomController)
    {
        mushroomList.Add(mushroomController);
    }

    [SerializeField] float treeUIShowAngleDifference;
    [SerializeField] float mushroomUIShowAngleDifference;

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
    public void CheckDistanceFromMushrooms()
    {
        if (!isManagerStateActive) return;
        if (mushroomList.Count == 0) return;

        float containerToCameraAngle = Vector2.SignedAngle(cameraContainerTransform.up, cameraContainerTransform.up);

        foreach (MushroomController mushroomController in mushroomList)
        {
            Vector2 containerToTreeVector = mushroomController.transform.position - cameraContainerTransform.position;
            float containerToTreeAngle = Vector2.SignedAngle(containerToTreeVector, cameraContainerTransform.up);
            bool shouldShowUI = Mathf.Abs(containerToCameraAngle - containerToTreeAngle) < treeUIShowAngleDifference;

            mushroomController.SetStartButtonActive(shouldShowUI);
        }
    }
}
