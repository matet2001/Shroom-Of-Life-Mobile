using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MushroomResourceData : ResourceData
{

    public MushroomResourceData(MushroomResourceDataType resourceDataType)
    {
        Initialize(resourceDataType);
    }
    private void Initialize(MushroomResourceDataType resourceDataType)
    {
        SetUpResources();

        foreach (ResourceType resourceType in resourceTypes)
        {
            resourceUse[resourceType] = resourceDataType.resourceUse.Find(x => x.type == resourceType).amount;
            resourceMax[resourceType] = resourceDataType.resourceMax.Find(x => x.type == resourceType).amount;
        }
    }
}
