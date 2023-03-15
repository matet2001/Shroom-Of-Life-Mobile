using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TreeController : MonoBehaviour
{
    public static event Action<TreeController> OnTreeCollision;
    public static event Action OnTreeUgrade;

    [SerializeField] TreeUIManager treeUIManager;
    [SerializeField] TreeCollider treeCollisionManager;
    [SerializeField] TreeSpriteController treeSpriteController;

    [SerializeField] TreeType treeType;
    public TreeResourceData resourceData { get; private set; }

    private int growLevel = 0;
    private readonly int growLevelMax = 3;

    private void Awake()
    {
        resourceData = new TreeResourceData(treeType);
    }
    private void Start()
    {
        treeSpriteController.SetTreeSprites(treeType);
        NextGrowLevel();

        treeUIManager.OnGrowButtonPressed += TryToGrow;

        treeCollisionManager.OnYarnCollided += delegate () { OnTreeCollision?.Invoke(this); };
    }   
    private void TryToGrow()
    {
        if (growLevel >= growLevelMax) return;
        if(!TryToSpendUgradeCost()) return;

        NextGrowLevel();

        if (growLevel >= growLevelMax) treeUIManager.SetGrowButtonShouldVisible(false);
    }
    private bool TryToSpendUgradeCost()
    {
        ResourceType resourceType = treeType.ugradeResourceType;

        if (growLevel > treeType.upgradeResourceAmount.Length) return false;
        float amount = treeType.upgradeResourceAmount[growLevel];

        return ResourceManager.resourceData.TryToSpendResource(resourceType, amount);      
    }
    public void NextGrowLevel()
    {
        growLevel++;
        IncreaseResourceValues();
        RefreshSprites();
        OnTreeUgrade?.Invoke();
    }
    private void IncreaseResourceValues()
    {
        resourceData.DuplicateResourceValues();      
    }
    private void RefreshSprites()
    {
        int spriteIndex = growLevel - 1;

        treeSpriteController.SetSpriteIndex(spriteIndex);
        treeCollisionManager.ResetCollider();
    }
    public TreeUIManager GetTreeUIManager() => treeUIManager;
}
