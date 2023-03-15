using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public List<TreeController> treeControllerList;
    public List<MushroomController> mushroomControllerList;

    public static event Action<List<TreeController>> OnTreeListChange;
    public static event Action<List<MushroomController>> OnMushroomListChange;

    private void Start()
    {
        OnTreeListChange?.Invoke(treeControllerList);
        OnMushroomListChange?.Invoke(mushroomControllerList);

        TreeController.OnTreeCollision += (treeController) => AddTreeToList(treeController);
        GlobeCollider.OnYarnExitGlobe += GlobeCollider_OnYarnExitGlobe;
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
        OnTreeListChange?.Invoke(treeControllerList);
    }
    public void AddMushroomToList(MushroomController mushroomController)
    {
        if (mushroomControllerList.Contains(mushroomController)) return;

        mushroomControllerList.Add(mushroomController);
        OnMushroomListChange?.Invoke(mushroomControllerList);
    }
}
