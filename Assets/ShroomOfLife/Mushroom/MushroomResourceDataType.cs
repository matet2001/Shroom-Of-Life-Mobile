using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MushroomResourceData", menuName = "ScriptableObjects/Resources/MushroomResourceData")]
public class MushroomResourceDataType : ScriptableObject
{
    public List<ResourceUnit> resourceUse;
    public List<ResourceUnit> resourceMax;

    [ContextMenu("Rebuild")]
    public void ReBuild()
    {
        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");

        resourceUse = new List<ResourceUnit>();
        resourceMax = new List<ResourceUnit>();

        foreach (ResourceType resourceType in resourceTypeContainer.resourceTypes)
        {
            ResourceUnit currentResourceAmount = new ResourceUnit(resourceType);

            resourceUse.Add(currentResourceAmount);
            resourceMax.Add(currentResourceAmount);
        }
    }
}
