using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StarterResourceData", menuName = "ScriptableObjects/Resources/StarterResourceData")]
public class StarterResourceDataType : ResourceDataType
{
    public List<ResourceUnit> resourceAmount;
    public List<ResourceUnit> resourceMax;

    [ContextMenu("Rebuild")]
    public void ReBuild()
    {
        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");

        resourceAmount = new List<ResourceUnit>();
        resourceMax = new List<ResourceUnit>();

        foreach (ResourceType resourceType in resourceTypeContainer.resourceTypes)
        {
            ResourceUnit currentResourceAmount = new ResourceUnit(resourceType);

            resourceAmount.Add(currentResourceAmount);
            resourceMax.Add(currentResourceAmount);
        }
    }
}
