using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MushroomResourceData : ResourceData
{
    public Dictionary<ResourceType, float> resourceUse;

    //Fill resource amount from resource data type
    public MushroomResourceData(MushroomResourceDataType resourceDataType) : base(resourceDataType)
    {
        Initialize(resourceDataType);
    }
    public void Initialize(MushroomResourceDataType resourceDataType)
    {
        SetUpResources();

        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");
        resourceTypes = resourceTypeContainer.resourceTypes.ToList();

        foreach (ResourceType resourceType in resourceTypes)
        {
            resourceUse[resourceType] = resourceDataType.resourceUse.Find(x => x.resourceType == resourceType).amount;
            resourceMax[resourceType] = resourceDataType.resourceMax.Find(x => x.resourceType == resourceType).amount;
        }
    }
    protected override void SetUpResources()
    {
        resourceUse = new Dictionary<ResourceType, float>();
        resourceMax = new Dictionary<ResourceType, float>();
    }
}
