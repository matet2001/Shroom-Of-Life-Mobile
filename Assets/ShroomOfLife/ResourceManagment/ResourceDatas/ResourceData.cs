using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceData : MonoBehaviour
{
    public Dictionary<ResourceType, float> resourceAmount;
    public Dictionary<ResourceType, float> resourceUsage;
    public Dictionary<ResourceType, float> resourceUse;
    public Dictionary<ResourceType, float> resourceProduce;
    public Dictionary<ResourceType, float> resourceMax;

    //Fill resource amount from resource data type
    public ResourceData(ResourceDataType resourceDataType)
    {
        Initialize(resourceDataType);
    }
    public void Initialize(ResourceDataType resourceDataType)
    {
        SetUpResources();
        
        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");

        foreach (ResourceType resourceType in resourceTypeContainer.resourceTypes)
        {
            resourceAmount[resourceType] = resourceDataType.resourceAmount.Find(x => x.resourceType == resourceType).amount;
            resourceUse[resourceType] = resourceDataType.resourceUse.Find(x => x.resourceType == resourceType).amount;
            resourceProduce[resourceType] = resourceDataType.resourceProcude.Find(x => x.resourceType == resourceType).amount;
            resourceMax[resourceType] = resourceDataType.resourceMax.Find(x => x.resourceType == resourceType).amount;
        }
    }
    private void SetUpResources()
    {
        resourceAmount = new Dictionary<ResourceType, float>();
        resourceUsage = new Dictionary<ResourceType, float>();
        resourceUse = new Dictionary<ResourceType, float>();
        resourceProduce = new Dictionary<ResourceType, float>();
        resourceMax = new Dictionary<ResourceType, float>();
    }
}
