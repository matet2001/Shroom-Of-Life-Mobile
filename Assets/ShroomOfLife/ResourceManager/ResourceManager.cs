using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static event Action OnLoseGame;

    public static event Action<Dictionary<ResourceType, float>> OnResourceAmountChange;
    public static event Action<Dictionary<ResourceType, float>> OnResourceAmountMaxChange;

    #region Built In Functions
    public static ResourceManager Instance;
    public static event Action<ResourceManagerData> OnResourceManagerInit;
    public static ResourceManagerData resourceData { private set; get; }
    [SerializeField] StarterResourceDataType resourceDataType;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        resourceData = new ResourceManagerData(resourceDataType);
        OnResourceManagerInit?.Invoke(resourceData);

        SubscribeToEvents(); 
    }
    private void SubscribeToEvents()
    {
        TreeController.OnTreeGrow += ManageTreeGrow;

        ConnectionManager.OnConnectionListInit += ConnectionInit;
        ConnectionManager.OnTreeListChange += AddTreeToList;
        ConnectionManager.OnMushroomListChange += AddMushroomToList;

        ResourceRefreshTimer.OnResourceRefresh += RefreshResourceData;
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.L))
        {
            OnLoseGame?.Invoke();
        }
    }
    #endregion
    #region Set Resource Data
    private List<TreeController> treeList = new List<TreeController>();
    private List<MushroomController> mushroomList = new List<MushroomController>();

   
    private void ManageTreeGrow(TreeController treeController)
    {
        TreeResourceData treeResourceData = treeController.resourceData;
        
        foreach (ResourceType resourceType in resourceData.resourceTypes)
        {
            int treePreviousLevel = treeController.growLevel - 1;
            
            float oldUseAmount = treeController.treeType.resourceUse.Find(x => x.type == resourceType).amount * treePreviousLevel;
            float useAmount = treeResourceData.resourceUse[resourceType] - oldUseAmount;
            AddToUseDict(new ResourceUnit(resourceType, useAmount));

            float oldProduceAmount = treeController.treeType.resourceProduce.Find(x => x.type == resourceType).amount * treePreviousLevel;
            float produceAmount = treeResourceData.resourceProduce[resourceType] - oldProduceAmount;
            AddToProduceDict(new ResourceUnit(resourceType, produceAmount));

            float oldMaxAmount = treeController.treeType.resourceMax.Find(x => x.type == resourceType).amount * treePreviousLevel;
            float maxAmount = treeResourceData.resourceMax[resourceType] - oldMaxAmount;
            AddToMaxDict(new ResourceUnit(resourceType, maxAmount));
        }

        OnResourceAmountMaxChange?.Invoke(resourceData.resourceMax);
    }
    private void ConnectionInit(List<TreeController> treeList, List<MushroomController> mushroomList)
    {
        treeList.ForEach(tree => AddTreeToList(tree));
        mushroomList.ForEach(mushroom => AddMushroomToList(mushroom));
    }
    private void AddTreeToList(TreeController treeController)
    {
        if (treeList.Contains(treeController)) return;
        treeList.Add(treeController);

        TreeResourceData treeResourceData = treeController.resourceData;

        foreach (ResourceType resourceType in resourceData.resourceTypes)
        {
            float useAmount = treeResourceData.resourceUse[resourceType];
            AddToUseDict(new ResourceUnit(resourceType, useAmount));

            float produceAmount = treeResourceData.resourceProduce[resourceType];
            AddToProduceDict(new ResourceUnit(resourceType, produceAmount));

            float maxAmount = treeResourceData.resourceMax[resourceType];
            AddToMaxDict(new ResourceUnit(resourceType, maxAmount));
        }

        OnResourceAmountMaxChange?.Invoke(resourceData.resourceMax);
    }
    private void AddMushroomToList(MushroomController mushroomController)
    {
        if (mushroomList.Contains(mushroomController)) return;
        mushroomList.Add(mushroomController);

        MushroomResourceData treeResourceData = mushroomController.resourceData;
        
        foreach (ResourceType resourceType in resourceData.resourceTypes)
        {
            float useAmount = treeResourceData.resourceUse[resourceType];
            AddToUseDict(new ResourceUnit(resourceType, useAmount));

            float maxAmount = treeResourceData.resourceMax[resourceType];
            AddToMaxDict(new ResourceUnit(resourceType, maxAmount));
        }

        OnResourceAmountMaxChange?.Invoke(resourceData.resourceMax);
    }
    private void AddToMaxDict(ResourceUnit resourceUnit)
    {
        resourceData.resourceMax[resourceUnit.type] += resourceUnit.amount;
    }
    private void AddToUseDict(ResourceUnit resourceUnit)
    {
        resourceData.resourceUse[resourceUnit.type] += resourceUnit.amount;
    }
    private void AddToProduceDict(ResourceUnit resourceUnit)
    {
        resourceData.resourceProduce[resourceUnit.type] += resourceUnit.amount;
    }
    #endregion
    #region Refresh Resource Data
    private void RefreshResourceData()
    {
        foreach (ResourceType resourceType in resourceData.resourceTypes)
        {
            TryToUseResource(resourceType);
            TryToProduceResource(resourceType);
        }

        OnResourceAmountChange?.Invoke(resourceData.resourceAmount);
    }
    #endregion
    #region Spend Resource
    private void TryToUseResource(ResourceType resourceType)
    {
        ResourceUnit resourceUnit = new ResourceUnit(resourceType, resourceData.resourceUse[resourceType]);

        if (resourceData.TryToSpendResource(resourceUnit)) return;

        OnLoseGame?.Invoke();
    }
    public bool TryToSpendResource(ResourceUnit resourceUnit)
    {
        bool isSpent = resourceData.TryToSpendResource(resourceUnit);
        OnResourceAmountChange?.Invoke(resourceData.resourceAmount);

        if (isSpent) return true;

        Debug.Log("Can't spent resource : " + resourceUnit.type);
        return false;
    }
    #endregion
    #region Add Resource
    private void TryToProduceResource(ResourceType resourceType)
    {
        ResourceUnit resourceUnit = new ResourceUnit(resourceType, resourceData.resourceProduce[resourceType]);
        
        if(resourceData.TryToAddResource(resourceUnit)) return;

        Debug.Log("Max reached for " + resourceType);
    }
    public bool TryToAddResource(ResourceUnit resourceUnit)
    {
        bool isAdded = resourceData.TryToAddResource(resourceUnit);
        OnResourceAmountChange?.Invoke(resourceData.resourceAmount);
        
        if (isAdded) return true;

        Debug.Log("Can't add, max reached for " + resourceUnit.type);
        return false;
    }
    #endregion
}
