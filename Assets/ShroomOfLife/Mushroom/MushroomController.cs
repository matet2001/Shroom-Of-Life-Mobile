using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomController : MonoBehaviour
{
    public MushroomResourceData resourceData { get; private set; }
    [SerializeField] MushroomResourceDataType resourceDataType;

    [SerializeField] Transform yarnStartPoint;

    public static MushroomController CreateMushroom(Vector2 createPosition)
    {
        GameObject mushroomPrefab = Resources.Load<GameObject>("MushroomBase");
        GameObject mushroom = Instantiate(mushroomPrefab, createPosition, Quaternion.identity);
        return mushroom.GetComponent<MushroomController>();
    }
    private void Awake()
    {
        resourceData = new MushroomResourceData(resourceDataType);
    }
    public Vector2 GetYarnStartPosition() => yarnStartPoint.position;
}
