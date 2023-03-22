using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGlobeRotator : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 50f;

    private void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
