using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Collidable collidable))
        {
            collidable.Collision();
        }
        if (collision.TryGetComponent(out StageTriggerer triggererObstacle))
        {
            triggererObstacle.Trigger();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out GlobeCollider globeCollisionController))
        {
            Vector2 exitPoint = transform.position;
            globeCollisionController.CollisionExit(exitPoint);
        }
    }
}
