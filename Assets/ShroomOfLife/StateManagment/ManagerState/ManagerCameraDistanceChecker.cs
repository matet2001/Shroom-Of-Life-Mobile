using StateManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerCameraDistanceChecker : MonoBehaviour
{
    private List<TreeUIManager> treeUIList = new List<TreeUIManager>();
    private List<MushroomController> mushroomUIList = new List<MushroomController>();

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

        WinState.OnWinGame += ManageGameEnd;
        LoseState.OnLoseGame += ManageGameEnd;
    }
    private void ManageGameEnd()
    {
        foreach(TreeUIManager treeUI in treeUIList)
        {
            treeUI.SetUIActive(false);
        }
        foreach(MushroomController mushroomController in mushroomUIList)
        {
            mushroomController.SetStartButtonActive(false);
        }
    }
    private void Update()
    {
        ManageTreeUI();
        ManageMushroomUI();
    }
    private void ConnectionInit(List<TreeController> treeList, List<MushroomController> mushroomList)
    {
        treeUIList = new List<TreeUIManager>();
        mushroomUIList = new List<MushroomController>();

        if (treeList.Count != 0) treeList.ForEach(tree => AddToTreeList(tree));
        if (mushroomList.Count != 0) mushroomList.ForEach(mushroom => AddToMushroomList(mushroom));
    }
    private void AddToTreeList(TreeController treeController)
    {
        treeUIList.Add(treeController.GetTreeUIManager());
    }
    private void AddToMushroomList(MushroomController mushroomController)
    {
        mushroomUIList.Add(mushroomController);
    }

    [SerializeField] float treeUIShowAngleDifference;
    [SerializeField] float mushroomUIShowAngleDifference;
    [SerializeField] float mushroomPlacementAngleDifference;

    private void ManageTreeUI()
    {
        if (!isManagerStateActive) return;
        if (treeUIList.Count == 0) return;

        foreach (TreeUIManager treeUI in treeUIList)
        {
            SetTreeUI(treeUI);
        }
    }
    private void SetTreeUI(TreeUIManager treeUI)
    {
        bool shouldShowUI = IsObjectCloserToCamera(treeUI.transform.position, mushroomUIShowAngleDifference);
        treeUI.SetUIActive(shouldShowUI);
    }
    private void ManageMushroomUI()
    {
        if (!isManagerStateActive) return;
        if (mushroomUIList.Count == 0) return;

        foreach (MushroomController mushroomController in mushroomUIList)
        {
            SetMushroomUI(mushroomController);
        }
    }
    private void SetMushroomUI(MushroomController mushroomController)
    {
        bool shouldShowUI = IsObjectCloserToCamera(mushroomController.transform.position, mushroomUIShowAngleDifference);
        mushroomController.SetStartButtonActive(shouldShowUI);
    }
    private void CheckCanCreateMushroom(Vector3 mushroomPosition)
    {
        float distanceFromNewMushroom = GetContainerToObjectAngle(mushroomPosition);
        
        foreach (MushroomController mushroomController in mushroomUIList)
        {
            float distanceFromCurrentMushroom = GetContainerToObjectAngle(mushroomController.transform.position);
            float distanceDifference = Mathf.Abs(distanceFromNewMushroom - distanceFromCurrentMushroom);
            if (distanceDifference < mushroomPlacementAngleDifference)
            {
                JuiceTextCreator.CreateJuiceText("Can't place mushroom here, there is another mushroom too close", Color.red);
                return;
            }
        }

        foreach (TreeUIManager treeUI in treeUIList)
        {
            float distanceFromCurrentTree = GetContainerToObjectAngle(treeUI.transform.position);
            float distanceDifference = Mathf.Abs(distanceFromNewMushroom - distanceFromCurrentTree);
            if (distanceDifference < mushroomPlacementAngleDifference)
            {
                JuiceTextCreator.CreateJuiceText("Can't place mushroom here, there is tree too close", Color.red);
                return;
            }
        }

        OnCanCreateMushroom?.Invoke(mushroomPosition);
    }
    private bool IsObjectCloserToCamera(Vector3 objectPosition, float angleDifference)
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
    public Transform GetClosestObjectToDirection(float direction)
    {
        if (!isManagerStateActive) return null;

        Transform closestMushroom = GetClosestMushroom(direction);
        float closestMushroomAngle = GetClosestObjectAngle(closestMushroom);
        
        Transform closestTree = GetClosestTree(direction);
        float closestTreeAngle = GetClosestObjectAngle(closestTree);

        Transform returnTransform = closestMushroomAngle < closestTreeAngle ? closestMushroom : closestTree;
        return returnTransform;
    }
    private Transform GetClosestMushroom(float direction)
    {
        foreach (MushroomController mushroomController in mushroomUIList)
        {
            float currentMushroomAngle = GetContainerToObjectAngle(mushroomController.transform.position);
            if (currentMushroomAngle * direction > mushroomUIShowAngleDifference) return mushroomController.transform;
        }
        return null;
    }
    private Transform GetClosestTree(float direction)
    {
        foreach (TreeUIManager treeUI in treeUIList)
        {
            float currentTreeAngle = GetContainerToObjectAngle(treeUI.transform.position);
            if (currentTreeAngle * direction > treeUIShowAngleDifference) return treeUI.transform;
        }

        return null;
    }
    private float GetClosestObjectAngle(Transform transform)
    {
        if (!transform) return Mathf.Infinity;
        return Mathf.Abs(GetContainerToObjectAngle(transform.position));
    }
}
