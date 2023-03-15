using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TreeController : MonoBehaviour
{
    public static event Action<TreeController> OnTreeCollision;

    [SerializeField] TreeUIManager treeUIManager;
    [SerializeField] TreeCollider treeCollisionManager;
    [SerializeField] TreeSpriteController treeSpriteController;

    [SerializeField] TreeType treeType;
    private TreeResourceData resourceData;

    private int growLevel = 0, growLevelMax = 3;

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
        //Check enough resources

        NextGrowLevel();
        if (growLevel >= growLevelMax) treeUIManager.SetGrowButtonShouldVisible(false);
    }
    public void NextGrowLevel()
    {
        //Substract resources
        //Refresh resource values

        growLevel++;
        int spriteIndex = growLevel - 1;

        treeSpriteController.SetSpriteIndex(spriteIndex);
        treeCollisionManager.ResetCollider();
    }
    public TreeUIManager GetTreeUIManager() => treeUIManager;
}
