using Sentinel.NotePlus;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeResourceData : ResourceData
{
    //                GrowLevel ResourceTypeList(3)
    public Dictionary<int, ResourceUnitList> resourceProduce;
    public Dictionary<int, ResourceUnitList> resourceUse;
    public Dictionary<int, ResourceUnitList> resourceMax;

    public TreeResourceData(TreeType treeType)
    {
        Initialize(treeType);
    }
    private void Initialize(TreeType treeType)
    {
        SetUpResources();

        for(int i = 0; i < treeType.resourceProduce.Count;i++)
        {
            resourceProduce.Add(i, treeType.resourceProduce[i]);
            resourceUse.Add(i, treeType.resourceUse[i]);
            resourceMax.Add(i, treeType.resourceMax[i]);
        }
    }
    protected override void SetUpResources()
    {
        base.SetUpResources();

        resourceProduce = new Dictionary<int, ResourceUnitList>();
        resourceUse = new Dictionary<int, ResourceUnitList>();
        resourceMax = new Dictionary<int, ResourceUnitList>();
    }
}
