using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    [field: SerializeField]
    public TreeType treeType { get; private set; }
    public TreeResourceData resourceData { get; private set; }

    public static event Action OnTreeInit;
    public static event Action OnTreeMax;

    [SerializeField] TreeResourcePaneUI resourcePanelUI;
    [SerializeField] TreeUIManager treeUIManager;

    private bool isReachedMaxLevel;

    public Biome biome { get; private set; }
    
    private void Awake()
    {
        resourceData = new TreeResourceData(treeType);
    }
    private void Start()
    {
        SetUpSprite();
        SubscriptToEvents();
        treeUIManager.SetGrowCostText(treeType.upgradeResourceCost[growLevel].amount);    

        resourcePanelUI.RefreshSliderValues(this);

        biome = BiomeManager.Instance.GetBiomOnPosition(transform.position);

        OnTreeInit?.Invoke();
    } 
    private void SubscriptToEvents()
    {
        treeUIManager.growButtonController.OnGrowButtonPressed += TryToGrow;
        treeUIManager.growButtonController.OnPointerEnterButton += TreeUIManager_OnPointerEnterButton;
        treeUIManager.growButtonController.OnPointerExitButton += TreeUIManager_OnPointerExitButton;

        treeCollisionManager.OnYarnCollided += delegate () { OnTreeCollision?.Invoke(this); };

        LevelSceneManager.OnRestart += ResetTree;

    }
    private void TreeUIManager_OnPointerEnterButton()
    {
        if (isReachedMaxLevel)
        {
            resourcePanelUI.RefreshSliderValues(this);
            return;
        } 
        resourcePanelUI.RefreshSliderValues(this, growLevel);
    }
    private void TreeUIManager_OnPointerExitButton()
    {
        resourcePanelUI.RefreshSliderValues(this);
    }
    #region Manage Collider and Visual
    public static event Action<TreeController> OnTreeCollision;

    [SerializeField] TreeVisualController treeSpriteController;
    [SerializeField] TreeCollider treeCollisionManager;

    private void SetUpSprite()
    {
        treeSpriteController.SetTreeSprites(treeType);
        RefreshSprites();
    }
    private void RefreshSprites()
    {
        treeSpriteController.SetSpriteIndex(growLevel);
        treeCollisionManager.ResetCollider();
    }
    #endregion
    #region Grow
    public static event Action<TreeController> OnTreeGrow;

    public int growLevel { private set; get;}
    private readonly int growLevelMax = 3;

    private void TryToGrow()
    {
        if (isReachedMaxLevel) return;

        if (!TryToSpendUgradeCost())
        {
            JuiceTextCreator.CreateJuiceText("You don't have enough material to grow tree", Color.red);
            return;
        }
        NextGrowLevel();      
        InvokeIsTreeMax();
        SetTreeUI();
    }
    private void PlayGrowSound()
    {
        SoundManager.Instance.PlaySound("Tree/Grow", transform.position);
    }
    private bool TryToSpendUgradeCost()
    {
        return ResourceManager.Instance.TryToSpendResource(treeType.upgradeResourceCost[growLevel]);
    }
    private void NextGrowLevel()
    {
        growLevel++;
        RefreshSprites();
        OnTreeGrow?.Invoke(this);
        PlayGrowSound();
    }
    private void SetTreeUI()
    {
        treeUIManager.SetGrowButtonShouldVisible(!isReachedMaxLevel);

        if (isReachedMaxLevel) return;

        treeUIManager.SetGrowCostText(treeType.upgradeResourceCost[growLevel].amount);
        resourcePanelUI.RefreshSliderValues(this);
    }
    private void InvokeIsTreeMax()
    {
        if (growLevel < growLevelMax - 1) return;
        if (isReachedMaxLevel) return;
        
        isReachedMaxLevel = true;
        resourcePanelUI.RefreshSliderValues(this);
        OnTreeMax?.Invoke();
    }
    #endregion
    #region Resource Managment
    private ResourceUnit GetResourceFromList(ResourceType resourceType, ResourceUnitList resourceUnitList)
    {
        ResourceUnit returnResourceUnit = new ResourceUnit(resourceType);

        foreach (ResourceUnit resourceUnit in resourceUnitList.resourceUnits)
        {
            if (resourceUnit.type == resourceType)
            {
                returnResourceUnit = resourceUnit;
            }
        }
        return returnResourceUnit;
    }
    public ResourceUnit GetCurrentResourceProduce(ResourceType resourceType)
    {
        return GetResourceFromList(resourceType, resourceData.resourceProduce[growLevel]);
    }
    public ResourceUnit GetCurrentResourceUse(ResourceType resourceType)
    {
        return GetResourceFromList(resourceType, resourceData.resourceUse[growLevel]);
    } 
    public ResourceUnit GetCurrentResourceMax(ResourceType resourceType)
    {
        return GetResourceFromList(resourceType, resourceData.resourceMax[growLevel]);
    }
    public float GetResourceProduceAtLevel(ResourceType resourceType, int growthLevel)
    {
        return GetResourceFromList(resourceType, resourceData.resourceProduce[growthLevel]).amount;
    }
    public float GetResourceUseAtLevel(ResourceType resourceType, int growthLevel)
    {
        return GetResourceFromList(resourceType, resourceData.resourceUse[growthLevel]).amount;
    }
    public float GetResourceMaxAtLevel(ResourceType resourceType, int growthLevel)
    {
        return GetResourceFromList(resourceType, resourceData.resourceMax[growthLevel]).amount;
    }
    #endregion
    #region Manage UI
    public TreeUIManager GetTreeUIManager() => treeUIManager;
    #endregion
    private void ResetTree()
    {
        growLevel = 0;
        RefreshSprites();
        treeUIManager.SetGrowButtonShouldVisible(true);
        treeUIManager.SetGrowCostText(treeType.upgradeResourceCost[growLevel].amount);
        resourcePanelUI.RefreshSliderValues(this);
    }
}
