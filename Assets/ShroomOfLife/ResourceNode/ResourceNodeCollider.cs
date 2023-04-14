using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Collider2D))]
public class ResourceNodeCollider : CollidableObstacle
{
    [SerializeField] ResourceUnit resourceUnit;
    [SerializeField] float sizeMultiplier = 1f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
    }
    public override void Collision()
    {
        base.Collision();
        ResourceManager.Instance.TryToAddResource(resourceUnit);
        SoundManager.Instance.PlaySound("Yarn/Succes", transform.position);
        StartDisappeare();
    }
    private void Update()
    {
        SetScaleBasedOnResourceAmount();
        Disappeare();
    }   
    private void SetScaleBasedOnResourceAmount()
    {
        float amount = resourceUnit.amount / sizeMultiplier;
        transform.localScale = new Vector3(amount, amount, 1);
    }
    #region Disappeare
    private SpriteRenderer spriteRenderer;
    private new Collider2D collider;
    private float alpha, disappeareTime = 2f, timeElapsed;
    private bool shouldDisappeare;
    
    private void Disappeare()
    {
        if (!shouldDisappeare) return;

        if (timeElapsed < disappeareTime)
        {
            alpha = Mathf.Lerp(1, 0, timeElapsed / disappeareTime);
            timeElapsed += Time.deltaTime;

            Color color = spriteRenderer.color;
            color = new Color(color.r, color.g, color.b, alpha);
            spriteRenderer.color = color;
        }
        else Destroy(gameObject);
    }
    private void StartDisappeare()
    {
        collider.enabled = false;
        shouldDisappeare = true;
        alpha = spriteRenderer.color.a;
    }
    #endregion
}