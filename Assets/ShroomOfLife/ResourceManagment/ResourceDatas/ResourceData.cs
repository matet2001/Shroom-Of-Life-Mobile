using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ResourceData
{
    public Dictionary<ResourceType, float> resourceMax;
    public List<ResourceType> resourceTypes;

    public ResourceData(ResourceDataType resourceDataType)
    {

    }
    protected abstract void SetUpResources();
}
public class ResourceManagerData : ResourceData
{
    #region Resource Data Store
    public Dictionary<ResourceType, float> resourceAmount;
    public Dictionary<ResourceType, float> resourceUse;
    public Dictionary<ResourceType, float> resourceProduce;

    //Fill resource amount from resource data type
    public ResourceManagerData(StarterResourceDataType resourceDataType) : base(resourceDataType)
    {
        Initialize(resourceDataType);
    }
    public void Initialize(StarterResourceDataType resourceDataType)
    {
        SetUpResources();

        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");
        resourceTypes = resourceTypeContainer.resourceTypes.ToList();

        foreach (ResourceType resourceType in resourceTypes)
        {
            resourceAmount[resourceType] = resourceDataType.resourceAmount.Find(x => x.resourceType == resourceType).amount;
            resourceProduce.Add(resourceType, 0f);
            resourceMax[resourceType] = resourceDataType.resourceMax.Find(x => x.resourceType == resourceType).amount;
        }
    }
    protected override void SetUpResources()
    {
        resourceAmount = new Dictionary<ResourceType, float>();
        resourceUse = new Dictionary<ResourceType, float>();
        resourceProduce = new Dictionary<ResourceType, float>();
        resourceMax = new Dictionary<ResourceType, float>();
    }
    #endregion
    #region Spend Resource
    public bool TryToSpendResource(ResourceType resourceType, float amount)
    {
        if (CanSpendResource(resourceType, amount))
        {
            SpendResource(resourceType, amount);
            return true;
        }
        else return false;
    }
    private bool CanSpendResource(ResourceType resourceType, float amount)
    {
        if (resourceAmount[resourceType] >= amount) return true;
        else return false;
    }
    private void SpendResource(ResourceType resourceType, float amount)
    {
        if (CanSpendResource(resourceType, amount))
        {
            resourceAmount[resourceType] -= amount;
        }
    }
    #endregion
    #region Produce Resource
    public bool TryToAddResource(ResourceAmount _resourceAmount)
    {
        ResourceType resourceType = _resourceAmount.resourceType;
        float amount = _resourceAmount.amount;
        
        if (CanAddResource(resourceType, amount))
        {
            AddResource(resourceType, amount);
            return true;
        }
        else
        {
            float amountToAdd = resourceMax[resourceType] - resourceAmount[resourceType];
            AddResource(resourceType, amountToAdd);
            return false;
        }
    }
    private bool CanAddResource(ResourceType resourceType, float amount)
    {
        if (resourceAmount[resourceType] + amount <= resourceMax[resourceType]) return true;
        else return false;
    }
    private void AddResource(ResourceType resourceType, float amount)
    {
        if (CanAddResource(resourceType, amount))
        {
            resourceAmount[resourceType] += amount;
        }
    }
    #endregion
}