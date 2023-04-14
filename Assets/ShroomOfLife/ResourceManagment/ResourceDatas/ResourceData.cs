using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ResourceData
{
    public List<ResourceType> resourceTypes;

    protected virtual void SetUpResources()
    {
        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");
        resourceTypes = resourceTypeContainer.resourceTypes.ToList();
    }
}