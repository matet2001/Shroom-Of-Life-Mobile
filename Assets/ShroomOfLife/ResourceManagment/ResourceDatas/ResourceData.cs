using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ResourceData
{
    public Dictionary<ResourceType, float> resourceMax;
    public Dictionary<ResourceType, float> resourceUse;
    public List<ResourceType> resourceTypes;

    protected virtual void SetUpResources()
    {
        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");
        resourceTypes = resourceTypeContainer.resourceTypes.ToList();

        resourceMax = new Dictionary<ResourceType, float>();
        resourceUse = new Dictionary<ResourceType, float>();
    }
}