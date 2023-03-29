using ImmersiveVRTools.Runtime.Common.PropertyDrawer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    [field: SerializeField]
    public TreeType treeType { get; private set; }
    public TreeResourceData resourceData { get; private set; }

    [SerializeField] TreeResourcePaneUI panelUI;
    [SerializeField] TreeUIManager treeUIManager;

    private void Awake()
    {
        resourceData = new TreeResourceData(treeType);
    }
    private void Start()
    {
        SetUpSprite();
        SubscriptToEvents();
        treeUIManager.SetGrowCostText(treeType.upgradeResourceCost[growLevel].amount);    

        panelUI.RefreshSliderValues(resourceData);
    } 
    private void SubscriptToEvents()
    {
        treeUIManager.growButtonController.OnGrowButtonPressed += TryToGrow;
        treeUIManager.growButtonController.OnPointerEnterButton += TreeUIManager_OnPointerEnterButton;
        treeUIManager.growButtonController.OnPointerExitButton += TreeUIManager_OnPointerExitButton;

        treeCollisionManager.OnYarnCollided += delegate () { OnTreeCollision?.Invoke(this); };
    }
    private void TreeUIManager_OnPointerEnterButton()
    {
        panelUI.RefreshSliderValues(treeType, growLevel);
    }
    private void TreeUIManager_OnPointerExitButton()
    {
        panelUI.RefreshSliderValues(resourceData);
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
        if (growLevel >= growLevelMax - 1) return;
        if (!TryToSpendUgradeCost())
        {
            JuiceTextCreator.CreateJuiceText(transform.position, "You don't have enough \n material to grow", "BasicTextSO");
            return;
        }

        NextGrowLevel();

        bool shouldShowUI = growLevel < growLevelMax - 1;
        treeUIManager.SetGrowButtonShouldVisible(shouldShowUI);

        if(!shouldShowUI) return;
        treeUIManager.SetGrowCostText(treeType.upgradeResourceCost[growLevel].amount);
        panelUI.RefreshSliderValues(resourceData);
    }
    private bool TryToSpendUgradeCost()
    {
        return ResourceManager.Instance.TryToSpendResource(treeType.upgradeResourceCost[growLevel]);
    }
    private void NextGrowLevel()
    {
        growLevel++;
        IncreaseResourceValues();
        RefreshSprites();
        OnTreeGrow?.Invoke(this);
    }
    private void IncreaseResourceValues()
    {
        resourceData.DuplicateResourceValues();      
    }
    #endregion
    #region Manage UI
    

    public TreeUIManager GetTreeUIManager() => treeUIManager;
    #endregion
}
