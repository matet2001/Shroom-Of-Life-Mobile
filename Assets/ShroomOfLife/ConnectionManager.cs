using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public List<TreeController> treeControllerList;

    public static event Action<List<TreeController>> OnTreeListChange;

    private void Start()
    {
        OnTreeListChange?.Invoke(treeControllerList);

        TreeController.OnTreeCollision += (treeController) => AddTreeToList(treeController);
    }
    public void AddTreeToList(TreeController treeController)
    {
        if (treeControllerList.Contains(treeController)) return;

        treeControllerList.Add(treeController);
        OnTreeListChange?.Invoke(treeControllerList);
    }
}
