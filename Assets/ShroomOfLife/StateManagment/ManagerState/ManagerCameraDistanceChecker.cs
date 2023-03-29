using StateManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerCameraDistanceChecker : MonoBehaviour
{
    private List<TreeUIManager> treeUIList = new List<TreeUIManager>();
    private List<MushroomController> mushroomList = new List<MushroomController>();

    private Transform cameraContainerTransform;
    private bool isManagerStateActive;

    public static event Action<Vector3> OnCanCreateMushroom;

    private void Awake()
    {      
        SubscribeToEvents();
    }
    private void SubscribeToEvents()
    {
        ConnectionManager.OnConnectionListInit += ConnectionInit;
        ConnectionManager.OnTreeListChange += AddToTreeList;
        ConnectionManager.OnMushroomListChange += AddToMushroomList;

        ManagerState.OnManagerStateInit += delegate (Transform containerTransform) { cameraContainerTransform = containerTransform; };
        ManagerState.OnManagerStateEnter += delegate () { isManagerStateActive = true; };
        ManagerState.OnManagerStateExit += delegate (Transform containerTransform) { isManagerStateActive = false; };

        GlobeCollider.OnYarnExitGlobe += CheckCanCreateMushroom;
    }
    private void Update()
    {
        CheckDistanceFromTrees();
        CheckDistanceFromMushrooms();
    }
    private void ConnectionInit(List<TreeController> treeList, List<MushroomController> mushroomList)
    {
        if (treeList.Count != 0) treeList.ForEach(tree => AddToTreeList(tree));
        if (mushroomList.Count != 0) mushroomList.ForEach(mushroom => AddToMushroomList(mushroom));
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
    [SerializeField] float mushroomPlacementAngleDifference;

    public void CheckDistanceFromTrees()
    {
        if (!isManagerStateActive) return;
        if (treeUIList.Count == 0) return;

        foreach (TreeUIManager treeUI in treeUIList)
        {
            bool shouldShowUI = IsObjectCloserToCamera(treeUI.transform.position, mushroomUIShowAngleDifference);

            if (treeUI.isUIVisible == shouldShowUI) continue;
            treeUI.SetUIActive(shouldShowUI);
        }
    }
    public void CheckDistanceFromMushrooms()
    {
        if (!isManagerStateActive) return;
        if (mushroomList.Count == 0) return;

        foreach (MushroomController mushroomController in mushroomList)
        {
            bool shouldShowUI = IsObjectCloserToCamera(mushroomController.transform.position, mushroomUIShowAngleDifference);
            mushroomController.SetStartButtonActive(shouldShowUI);
        }
    }
    private void CheckCanCreateMushroom(Vector3 mushroomPosition)
    {
        float distanceFromNewMushroom = GetContainerToObjectAngle(mushroomPosition);
        foreach (MushroomController mushroomController in mushroomList)
        {
            float distanceFromCurrentMushroom = GetContainerToObjectAngle(mushroomController.transform.position);
            float distanceDifference = Mathf.Abs(distanceFromNewMushroom - distanceFromCurrentMushroom);
            if (distanceDifference < mushroomPlacementAngleDifference)
            {
                return;
            }
            Debug.Log(distanceDifference.ToString() + " - " + mushroomPlacementAngleDifference.ToString());
        }

        OnCanCreateMushroom?.Invoke(mushroomPosition);
    }
    public bool IsObjectCloserToCamera(Vector3 objectPosition, float angleDifference)
    {
        float containerToObjectAngle = GetContainerToObjectAngle(objectPosition);
        float containerToCameraAngle = Vector2.SignedAngle(cameraContainerTransform.up, cameraContainerTransform.up);

        return Mathf.Abs(containerToCameraAngle - containerToObjectAngle) < angleDifference;
    }
    private float GetContainerToObjectAngle(Vector3 objectPosition)
    {
        Vector2 containerToObjectVector = objectPosition - cameraContainerTransform.position;
        float containerToObjectAngle = Vector2.SignedAngle(containerToObjectVector, cameraContainerTransform.up);
        return containerToObjectAngle;
    }
}
