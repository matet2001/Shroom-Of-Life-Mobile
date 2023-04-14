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

        LevelSceneManager.OnRestart += ResetResourceManager;
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            OnLoseGame?.Invoke();
        }
#endif
    }
    #endregion
    #region Set Resource Data
    private List<TreeController> treeList = new List<TreeController>();
    private List<MushroomController> mushroomList = new List<MushroomController>();
  
    private void ManageTreeGrow(TreeController treeController)
    {
        TreeResourceData treeResourceData = treeController.resourceData;

        OnResourceAmountMaxChange?.Invoke(resourceData.resourceMax);
    }
    private void ConnectionInit(List<TreeController> _treeList, List<MushroomController> _mushroomList)
    {
        treeList = new List<TreeController>();
        mushroomList = new List<MushroomController>();
        
        if (_treeList.Count != 0) _treeList.ForEach(tree => AddTreeToList(tree));
        if (_mushroomList.Count != 0) _mushroomList.ForEach(mushroom => AddMushroomToList(mushroom));
    }
    private void AddTreeToList(TreeController treeController)
    {
        if (treeList.Contains(treeController)) return;
        treeList.Add(treeController);
        CalculateResourceMax();
    }
    private void AddMushroomToList(MushroomController mushroomController)
    {
        if (mushroomList.Contains(mushroomController)) return;
        mushroomList.Add(mushroomController);
        CalculateResourceMax();
    }
    private void CalculateResourceMax()
    {
        Dictionary<ResourceType, float> resourceMax = new Dictionary<ResourceType, float>();

        foreach (ResourceType resourceType in resourceData.resourceTypes)
        {
            float max = 0;
            foreach (TreeController tree in treeList)
            {
                max += tree.GetCurrentResourceMax(resourceType).amount;
            }
            foreach (MushroomController mushroom in mushroomList)
            {
                max += mushroom.resourceData.resourceMax[resourceType];
            }
            max += resourceDataType.resourceMax.Find(x => x.type == resourceType).amount;
            resourceMax.Add(resourceType, max);
            
        }
        resourceData.resourceMax = resourceMax;

        OnResourceAmountMaxChange?.Invoke(resourceData.resourceMax);
    }
    #endregion
    #region Refresh Resource Data
    private void RefreshResourceData()
    {       
        foreach (ResourceType resourceType in resourceData.resourceTypes)
        {
            foreach (TreeController tree in treeList)
            {               
                ResourceUnit currentResourceProduce = tree.GetCurrentResourceProduce(resourceType);
                TryToProduceResource(currentResourceProduce);

                currentResourceProduce = tree.GetCurrentResourceUse(resourceType);
                TryToUseResource(currentResourceProduce);
            }
            foreach(MushroomController mushroom in mushroomList)
            {
                float amount = mushroom.resourceData.resourceUse[resourceType];
                ResourceUnit currentResourceProduce = new ResourceUnit(resourceType, amount);
                TryToUseResource(currentResourceProduce);
            }
        }

        OnResourceAmountChange?.Invoke(resourceData.resourceAmount);
    }
    #endregion
    #region Spend Resource
    private void TryToUseResource(ResourceUnit resourceUnit)
    {
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
    private void TryToProduceResource(ResourceUnit resourceUnit)
    {
        if(resourceData.TryToAddResource(resourceUnit)) return;

        Debug.Log("Max reached for " + resourceUnit.type);
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
    private void ResetResourceManager()
    {
        resourceData = new ResourceManagerData(resourceDataType);
        OnResourceManagerInit?.Invoke(resourceData);

        OnResourceAmountChange?.Invoke(resourceData.resourceAmount);
        OnResourceAmountMaxChange?.Invoke(resourceData.resourceMax);
    }
}
