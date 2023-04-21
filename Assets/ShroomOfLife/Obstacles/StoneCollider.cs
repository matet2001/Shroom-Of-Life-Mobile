using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StoneCollider : CollidableObstacle
{
    public override void Collision()
    {
        base.Collision();
        SoundManager.Instance.PlaySound("Yarn/Fail", transform.position);
    }
}
