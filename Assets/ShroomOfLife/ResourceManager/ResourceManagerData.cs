using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManagerData : ResourceData
{
    #region Resource Data Store
    public Dictionary<ResourceType, float> resourceAmount;
    public Dictionary<ResourceType, float> resourceProduce;
    public Dictionary<ResourceType, float> resourceUse;
    public Dictionary<ResourceType, float> resourceMax;

    public ResourceManagerData(StarterResourceDataType resourceDataType)
    {
        Initialize(resourceDataType);
    }
    private void Initialize(StarterResourceDataType resourceDataType)
    {
        SetUpResources();

        foreach (ResourceType resourceType in resourceTypes)
        {
            resourceAmount[resourceType] = resourceDataType.resourceAmount.Find(x => x.type == resourceType).amount;
            resourceProduce.Add(resourceType, 0f);
            resourceUse.Add(resourceType, 0f);
            resourceMax[resourceType] = resourceDataType.resourceMax.Find(x => x.type == resourceType).amount;
        }
    }
    protected override void SetUpResources()
    {
        base.SetUpResources();

        resourceAmount = new Dictionary<ResourceType, float>();
        resourceProduce = new Dictionary<ResourceType, float>();
        resourceUse = new Dictionary<ResourceType, float>();
        resourceMax = new Dictionary<ResourceType, float>();
    }
    #endregion
    #region Spend Resource
    public bool TryToSpendResource(ResourceUnit resourceUnit)
    {
        float unitAmount = resourceUnit.amount;
        float currentAmount = resourceAmount[resourceUnit.type];

        if (currentAmount - unitAmount < 0f) return false;

        resourceAmount[resourceUnit.type] -= unitAmount;
        resourceAmount[resourceUnit.type] = Mathf.Max(resourceAmount[resourceUnit.type], 0f);
        
        return true;
    }
    public bool CanSpendResource(ResourceUnit resourceUnit)
    {
        float unitAmount = resourceUnit.amount;
        float currentAmount = resourceAmount[resourceUnit.type];

        if (currentAmount - unitAmount <= 0f) return false;
        return true;
    }
    #endregion
    #region Add Resource
    public bool TryToAddResource(ResourceUnit resourceUnit)
    {
        float maxAmount = resourceMax[resourceUnit.type];
        float unitAmount = resourceUnit.amount;
        float currentAmount = resourceAmount[resourceUnit.type];

        resourceAmount[resourceUnit.type] += unitAmount;
        resourceAmount[resourceUnit.type] = Mathf.Min(resourceAmount[resourceUnit.type], maxAmount);

        if (currentAmount + unitAmount >= maxAmount) return false;
        return true;
    }
    #endregion
}
