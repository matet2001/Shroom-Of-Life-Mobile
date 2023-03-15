using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotateToGlobeCenter : MonoBehaviour
{
    private void Update()
    {
        Vector3 rotationVector = transform.position - Vector3.zero;
        float zAngle = Vector3.SignedAngle(Vector3.up, rotationVector, transform.forward);
        transform.eulerAngles = new Vector3(0f, 0f, zAngle);
    }
}
