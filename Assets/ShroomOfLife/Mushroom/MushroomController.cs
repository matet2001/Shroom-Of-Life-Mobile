using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomController : MonoBehaviour
{
    [SerializeField] Transform yarnStartPoint;

    public MushroomResourceData resourceData { get; private set; }
    [SerializeField] MushroomResourceDataType resourceDataType;

    public static MushroomController CreateMushroom(Vector2 createPosition)
    {
        GameObject mushroomPrefab = Resources.Load<GameObject>("MushroomBase");
        GameObject mushroom = Instantiate(mushroomPrefab, createPosition, Quaternion.identity);
        return mushroom.GetComponent<MushroomController>();
    }
    public Vector2 GetYarnStartPosition() => yarnStartPoint.position;

    private void Awake()
    {
        resourceData = new MushroomResourceData(resourceDataType);
    }
}
