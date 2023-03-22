using Sentinel.NotePlus;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeResourceData : ResourceData
{
    public Dictionary<ResourceType, float> resourceProduce;

    public TreeResourceData(TreeType treeType)
    {
        Initialize(treeType);
    }
    private void Initialize(TreeType treeType)
    {
        SetUpResources();

        foreach (ResourceType resourceType in resourceTypes)
        {
            resourceUse[resourceType] = treeType.resourceUse.Find(x => x.type == resourceType).amount;
            resourceProduce[resourceType] = treeType.resourceProduce.Find(x => x.type == resourceType).amount;
            resourceMax[resourceType] = treeType.resourceMax.Find(x => x.type == resourceType).amount;
        }
    }
    protected override void SetUpResources()
    {
        base.SetUpResources();
        
        resourceProduce = new Dictionary<ResourceType, float>();     
    }
    public void DuplicateResourceValues()
    {
        foreach (ResourceType resourceType in resourceTypes)
        {
            resourceUse[resourceType] *= 2;
            resourceProduce[resourceType] *= 2;
            resourceMax[resourceType] *= 2;
        }
    }
}
