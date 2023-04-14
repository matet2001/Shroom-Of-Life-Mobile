using StateManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MushroomController : MonoBehaviour
{
    public static event Action<Vector2> OnMushroomCreate;

    public MushroomResourceData resourceData { get; private set; }
    [SerializeField] MushroomResourceDataType resourceDataType;

    [SerializeReference] Transform yarnStartPoint;

    [SerializeField] Button startButton;
    public static event Action<Vector3> OnTryToStartYarn;

    [SerializeField] float startPointDepth;
    private static float staticStartPointDepth;

    private static List<Biome> mushroomBiomesList = new List<Biome>();
    public Biome biome;

    public static bool CanPlaceMushroom(Vector2 createPosition)
    {
        Vector2 center = GlobeCollider.globeTransform.position;
        Vector2 createPosToCenterVector = (center - createPosition).normalized;

        RaycastHit2D[] allHit = Physics2D.RaycastAll(createPosition, createPosToCenterVector, staticStartPointDepth * 2f);

        foreach(RaycastHit2D hit in allHit)
        {
            if (hit.transform.TryGetComponent(out CollidableObstacle collidable))
            {
                JuiceTextCreator.CreateJuiceText("Can't place mushroom here something block the way", Color.red);
                return false;
            } 
        }

        Biome biome = BiomeManager.Instance.GetBiomOnPosition(createPosition);
        if (!CanPlaceMushroomOnBiome(biome))
        {
            JuiceTextCreator.CreateJuiceText("Can't place mushroom here, there is another mushroom in this biome", Color.red);
            return false;
        }

        return true;
    }
    public static MushroomController CreateMushroom(Vector2 createPosition)
    {
        GameObject mushroomPrefab = Resources.Load<GameObject>("MushroomBase");
        GameObject mushroom = Instantiate(mushroomPrefab, createPosition, Quaternion.identity);

        Biome biome = BiomeManager.Instance.GetBiomOnPosition(createPosition);
        mushroom.GetComponent<MushroomController>().biome = biome;

        OnMushroomCreate?.Invoke(createPosition);
        return mushroom.GetComponent<MushroomController>();
    }
    private void OnValidate()
    {
        SetYarnStartPosition();
    }
    private void SetYarnStartPosition()
    {
        Vector3 downVector = -transform.up.normalized;
        Vector3 startPosition = transform.position + downVector * startPointDepth;
        yarnStartPoint.transform.position = startPosition;
    }
    private void Awake()
    {
        resourceData = new MushroomResourceData(resourceDataType);
        staticStartPointDepth = startPointDepth;
    }
    private void Start()
    {
        ConquerState.OnConquerStateEnter += HideStartButton;
        LevelSceneManager.OnRestart += LevelSceneManager_OnRestart;

        mushroomBiomesList.Add(biome);
    }
    private void LevelSceneManager_OnRestart()
    {
        mushroomBiomesList.Clear();
    }
    public void TryToStartYarn()
    {
        OnTryToStartYarn?.Invoke(yarnStartPoint.position);
    }
    private void HideStartButton() => SetStartButtonActive(false);
    public void SetStartButtonActive(bool active) => startButton.gameObject.SetActive(active);
    private static bool CanPlaceMushroomOnBiome(Biome biome)
    {
        return !mushroomBiomesList.Contains(biome);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 vector = -transform.up * startPointDepth;
        Gizmos.DrawLine(transform.position, transform.position + vector);
    }
}
