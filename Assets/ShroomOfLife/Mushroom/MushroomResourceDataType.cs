using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MushroomResourceData", menuName = "ScriptableObjects/Resources/MushroomResourceData")]
public class MushroomResourceDataType : ResourceDataType
{
    public List<ResourceAmount> resourceUse;

    public override void ReBuild()
    {
        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");

        resourceUse = new List<ResourceAmount>();
        resourceMax = new List<ResourceAmount>();

        foreach (ResourceType resourceType in resourceTypeContainer.resourceTypes)
        {
            ResourceAmount currentResourceAmount = new ResourceAmount(resourceType);

            resourceUse.Add(currentResourceAmount);
            resourceMax.Add(currentResourceAmount);
        }
    }
}
