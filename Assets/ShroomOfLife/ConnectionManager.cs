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

    public List<TreeController> treeControllerList;
    public List<MushroomController> mushroomControllerList;
    
    private void Start()
    {
        //OnTreeListChange?.Invoke(treeControllerList);
        //OnMushroomListChange?.Invoke(mushroomControllerList);

        TreeController.OnTreeCollision += (treeController) => AddTreeToList(treeController);
        GlobeCollider.OnYarnExitGlobe += GlobeCollider_OnYarnExitGlobe;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnWinGame?.Invoke();
        }
    }
    private void GlobeCollider_OnYarnExitGlobe(Vector2 exitPosition)
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
