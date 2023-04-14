using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    #region Manage Connection Lists
    public static event Action OnWinGame;
    
    public static event Action<TreeController> OnTreeListChange;
    public static event Action<MushroomController> OnMushroomListChange;

    public static event Action<List<TreeController>, List<MushroomController>> OnConnectionListInit;

    public List<TreeController> treeControllerList = new List<TreeController>();
    public List<MushroomController> mushroomControllerList = new List<MushroomController>();
    
    private List<TreeController> originalTreeControllerList = new List<TreeController>();
    private List<MushroomController> originMushroomControllerList = new List<MushroomController>();

    private int allTree;
    private int maxTree;

    private Biome yarnStartBiome;

    private void Awake()
    {
        TreeController.OnTreeInit += IncreaseAllTree;
        TreeController.OnTreeMax += IncreaseMaxTree;
    }
    private void Start()
    {
        OnConnectionListInit?.Invoke(treeControllerList, mushroomControllerList);

        originalTreeControllerList.AddRange(treeControllerList);
        originMushroomControllerList.AddRange(mushroomControllerList);

        TreeController.OnTreeCollision += TreeController_OnTreeCollision;
        ManagerCameraDistanceChecker.OnCanCreateMushroom += CreateNewMushroom;

        LevelSceneManager.OnRestart += ResetConnectionManager;

        YarnMovementController.OnYarnStart += SetYarnStartBiome;
    }
    private void TreeController_OnTreeCollision(TreeController treeController)
    {
        Biome treeBiome = treeController.biome;
        if (treeBiome != yarnStartBiome)
        {
            JuiceTextCreator.CreateJuiceText("You can only connect trees, with a mushroom from the same biome", Color.red);
            return;
        }

        bool isAdd = AddTreeToList(treeController);
        if (isAdd)
        {
            SoundManager.Instance.PlaySound("Yarn/Succes", transform.position);
        }
    }
    private void IncreaseAllTree()
    {
        allTree++;
    }
    private void IncreaseMaxTree()
    {
        maxTree++;
        
        if (maxTree >= allTree)
        {
            OnWinGame?.Invoke();
        }
    }
    private void SetYarnStartBiome(Vector2 position)
    {
        yarnStartBiome = BiomeManager.Instance.GetBiomOnPosition(position);
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnWinGame?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            JuiceTextCreator.CreateJuiceText("Test");
        }
#endif
    }
    private void CreateNewMushroom(Vector3 exitPosition)
    {
        if (!MushroomController.CanPlaceMushroom(exitPosition)) return;
        MushroomController mushroomController = MushroomController.CreateMushroom(exitPosition);
        AddMushroomToList(mushroomController);
    }
    public bool AddTreeToList(TreeController treeController)
    {
        if (treeControllerList.Contains(treeController)) return false;

        treeControllerList.Add(treeController);
        OnTreeListChange?.Invoke(treeController);

        return true;
    }
    public void AddMushroomToList(MushroomController mushroomController)
    {
        if (mushroomControllerList.Contains(mushroomController)) return;

        mushroomControllerList.Add(mushroomController);
        OnMushroomListChange?.Invoke(mushroomController);
    }
    #endregion
    private void ResetConnectionManager()
    {
        treeControllerList = new List<TreeController>();
        treeControllerList.AddRange(originalTreeControllerList);
        
        for (int i = 0; i < mushroomControllerList.Count; i++)
        {
            if (!originMushroomControllerList.Contains(mushroomControllerList[i]))
            {
                Destroy(mushroomControllerList[i].gameObject);
            }
        }
        mushroomControllerList = new List<MushroomController>();
        mushroomControllerList.AddRange(originMushroomControllerList);

        OnConnectionListInit?.Invoke(treeControllerList, mushroomControllerList);
    }
}
