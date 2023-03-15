using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Collider2D))]
public class ResourceNodeCollider : Collidable
{
    [SerializeField] ResourceAmount resourceAmount;

    private SpriteRenderer spriteRenderer;
    private new Collider2D collider;
    public float alpha, disappeareTime = 2f, timeElapsed;
    public bool shouldDisappeare;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
    }
    private void Update()
    {
        SetScaleBasedOnResourceAmount();
        Disappeare();
    }
    private void SetScaleBasedOnResourceAmount()
    {
        float amount = resourceAmount.amount / 10f;
        transform.localScale = new Vector3(amount, amount, 1);
    }
    public override void Collision()
    {
        base.Collision();
        ResourceManager.resourceData.TryToAddResource(resourceAmount);
        StartDisappeare();
    }
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
}