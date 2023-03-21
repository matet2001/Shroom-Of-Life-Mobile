using Sentinel.NotePlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeVisualController : MonoBehaviour
{
    [SerializeField] SpriteRenderer treeSpriteRenderer, rootSpriteRenderer;
    private TreeSprite[] treeSprites;

    public void SetTreeSprites(TreeType treeType)
    {
        treeSprites = treeType.treeSprites;
    }
    public void SetSpriteIndex(int spriteIndex)
    {
        treeSpriteRenderer.sprite = treeSprites[spriteIndex].treeSprite;
        rootSpriteRenderer.sprite = treeSprites[spriteIndex].rootSprite;
    }
}
