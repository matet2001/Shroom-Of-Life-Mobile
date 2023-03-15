using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static event Action<ResourceManagerData> OnResourceManagerInit;
    public static event Action<Dictionary<ResourceType, float>> OnResourceAmountChange;
    public static event Action<Dictionary<ResourceType, float>> OnResourceAmountMaxChange;

    public static ResourceManagerData resourceData { private set; get; }
    [SerializeField] StarterResourceDataType resourceDataType;

    [SerializeField] float refreshTimer;
    private float refreshTimeMax;

    private List<TreeController> treeList;

    private void Awake()
    {
        resourceData = new ResourceManagerData(resourceDataType);
        refreshTimeMax = refreshTimer;
    }
    private void Start()
    {
        ConnectionManager.OnTreeListChange += ConnectionManager_OnTreeListChange;
        TreeController.OnTreeUgrade += SetUpTreeResourceData;
        ConnectionManager.OnMushroomListChange += SetUpResourceUse;

        OnResourceManagerInit?.Invoke(resourceData);
    }
    private void ConnectionManager_OnTreeListChange(List<TreeController> _treeList)
    {
        treeList = _treeList;
    }
    private void SetUpTreeResourceData()
    {
        resourceData.resourceProduce = new Dictionary<ResourceType, float>();
        resourceData.resourceMax = new Dictionary<ResourceType, float>();

        foreach (ResourceType resourceType in resourceData.resourceTypes)
        {
            resourceData.resourceProduce.Add(resourceType, 0f);
            resourceData.resourceMax.Add(resourceType, 0f);

            foreach (TreeController treeController in treeList)
            {
                TreeResourceData treeResourceData = treeController.resourceData;

                float produceAmount = treeResourceData.resourceProduce[resourceType];
                resourceData.resourceProduce[resourceType] += produceAmount;

                float maxAmount = treeResourceData.resourceMax[resourceType];
                resourceData.resourceMax[resourceType] += maxAmount;
            }
        }

        OnResourceAmountMaxChange?.Invoke(resourceData.resourceMax);
    }
    private void SetUpResourceUse(List<MushroomController> mushroomList)
    {
        resourceData.resourceUse = new Dictionary<ResourceType, float>();

        foreach (ResourceType resourceType in resourceData.resourceTypes)
        {
            resourceData.resourceUse.Add(resourceType, 0f);

            foreach (MushroomController mushroomController in mushroomList)
            {
                MushroomResourceData mushroomResourceData = mushroomController.resourceData;

                float amount = mushroomResourceData.resourceUse[resourceType];
                resourceData.resourceUse[resourceType] += amount;
            }
        }
    }
    private void Update()
    {
        RefreshCountdown();
    }
    #region Refresh Resource Data
    private void RefreshCountdown()
    {
        if (refreshTimer <= 0)
        {
            refreshTimer = refreshTimeMax;
            RefreshResourceData();
        }
        else refreshTimer -= Time.deltaTime;
    }
    private void RefreshResourceData()
    {
        foreach (ResourceType resourceType in resourceData.resourceTypes)
        {
            UseResource(resourceType);
            ProduceResource(resourceType);
        }

        OnResourceAmountChange?.Invoke(resourceData.resourceAmount);
    }
    private void UseResource(ResourceType resourceType)
    {
        float amount = resourceData.resourceUse[resourceType];
        if(!resourceData.TryToSpendResource(resourceType, amount))
        {
            //Die
        }
    }
    private void ProduceResource(ResourceType resourceType)
    {
        float amount = resourceData.resourceProduce[resourceType];
        resourceData.TryToAddResource(new ResourceAmount(resourceType, amount));
    }
    #endregion
}
