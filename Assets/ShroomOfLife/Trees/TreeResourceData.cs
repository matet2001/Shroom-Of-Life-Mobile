using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeResourceData : ResourceData
{
    public Dictionary<ResourceType, float> resourceTrade;

    public TreeResourceData(TreeType treeType) : base(treeType)
    {       
        Initialize(treeType);

        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");
        resourceTrade = new Dictionary<ResourceType, float>();

        foreach (ResourceType resourceType in resourceTypeContainer.resourceTypes)
        {
            resourceTrade[resourceType] = treeType.resourceTrade.Find(x => x.resourceType == resourceType).amount;
        }
    }
}
