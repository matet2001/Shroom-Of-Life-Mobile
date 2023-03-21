using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotateToGlobeCenter : MonoBehaviour
{
    //[SerializeField] CircleCollider2D globeCollider;

    private void Update()
    {
        RotateToCenter();
    }
    //private void KeepOnGlobeArc()
    //{
    //    float globeSize = globeCollider.bounds.size.x;
    //}
    private void RotateToCenter()
    {
        Vector3 rotationVector = transform.position - Vector3.zero;
        float zAngle = Vector3.SignedAngle(Vector3.up, rotationVector, transform.forward);
        transform.eulerAngles = new Vector3(0f, 0f, zAngle);
    }
    //private void OnDrawGizmos()
    //{
    //    if(!globeCollider) return;

    //    float globeSize = globeCollider.bounds.extents.x;
    //    Gizmos.DrawWireSphere(Vector3.zero, globeSize);
    //}
}