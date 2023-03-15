using Sentinel.NotePlus;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeResourceData : ResourceData
{
    public Dictionary<ResourceType, float> resourceProduce;

    //Fill resource amount from resource data type
    public TreeResourceData(TreeType treeType) : base(treeType)
    {
        Initialize(treeType);
    }
    public void Initialize(TreeType treeType)
    {
        SetUpResources();

        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");
        resourceTypes = resourceTypeContainer.resourceTypes.ToList();

        foreach (ResourceType resourceType in resourceTypes)
        {
            resourceProduce[resourceType] = treeType.resourceProduce.Find(x => x.resourceType == resourceType).amount;
            resourceMax[resourceType] = treeType.resourceMax.Find(x => x.resourceType == resourceType).amount;
        }
    }
    protected override void SetUpResources()
    {
        resourceProduce = new Dictionary<ResourceType, float>();
        resourceMax = new Dictionary<ResourceType, float>();
    }
    public void DuplicateResourceValues()
    {
        foreach (ResourceType resourceType in resourceTypes)
        {
            resourceProduce[resourceType] *= 2;
            resourceMax[resourceType] *= 2;
        }
    }
}
