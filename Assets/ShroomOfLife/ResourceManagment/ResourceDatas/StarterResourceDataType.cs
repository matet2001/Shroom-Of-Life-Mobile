using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StarterResourceData", menuName = "ScriptableObjects/Resources/StarterResourceData")]
public class StarterResourceDataType : ResourceDataType
{
    public List<ResourceAmount> resourceAmount;

    public override void ReBuild()
    {
        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");

        resourceAmount = new List<ResourceAmount>();
        resourceMax = new List<ResourceAmount>();

        foreach (ResourceType resourceType in resourceTypeContainer.resourceTypes)
        {
            ResourceAmount currentResourceAmount = new ResourceAmount(resourceType);

            resourceAmount.Add(currentResourceAmount);
            resourceMax.Add(currentResourceAmount);
        }
    }
}
