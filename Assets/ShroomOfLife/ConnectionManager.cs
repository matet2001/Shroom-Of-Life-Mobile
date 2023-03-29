using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    #region Manage Connection Lists
    public static event Action OnWinGame;
    
    public static event Action<TreeController> OnTreeListChange;
    public static event Action<MushroomController> OnMushroomListChange;

    public static event Action<List<TreeController>, List<MushroomController>> OnConnectionListInit;

    public List<TreeController> treeControllerList = new List<TreeController>();
    public List<MushroomController> mushroomControllerList = new List<MushroomController>();
    
    private void Start()
    {
        OnConnectionListInit?.Invoke(treeControllerList, mushroomControllerList);

        TreeController.OnTreeCollision += (treeController) => AddTreeToList(treeController);
        ManagerCameraDistanceChecker.OnCanCreateMushroom += CreateNewMushroom;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnWinGame?.Invoke();
        }
    }
    private void CreateNewMushroom(Vector3 exitPosition)
    {
        MushroomController mushroomController = MushroomController.CreateMushroom(exitPosition);
        AddMushroomToList(mushroomController);
    }
    public void AddTreeToList(TreeController treeController)
    {
        if (treeControllerList.Contains(treeController)) return;

        treeControllerList.Add(treeController);
        OnTreeListChange?.Invoke(treeController);
    }
    public void AddMushroomToList(MushroomController mushroomController)
    {
        if (mushroomControllerList.Contains(mushroomController)) return;

        mushroomControllerList.Add(mushroomController);
        OnMushroomListChange?.Invoke(mushroomController);
    }
    #endregion
}
