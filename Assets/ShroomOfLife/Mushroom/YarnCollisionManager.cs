using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnCollisionManager : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ICollidable colidable))
        {
            colidable.Collision();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out GlobeCollisionController globeCollisionController))
        {
            globeCollisionController.CollisionExit();
        }
    }
}
