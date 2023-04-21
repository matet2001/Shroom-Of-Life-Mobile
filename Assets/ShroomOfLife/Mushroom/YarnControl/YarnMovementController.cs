using StateManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class YarnMovementController : MonoBehaviour
{
    #region Built In Functions
    private void Awake()
    {
        yarnTrailPrefab = Resources.Load<Transform>("pfYarnTrail");

        startPosition = transform.position;
    }
    private void Start()
    {
        SubscribeToEvents();

        darkerYarnMaterial = Resources.Load<Material>("DarkerYarn");
    }
    private void Update()
    {
        ManageMovement();
    }
    private void SubscribeToEvents()
    {
        WinState.OnWinGame += ManageGameEnd;
        LoseState.OnLoseGame += ManageGameEnd;
        
        ConquerState.OnConquerStateExit += StopMoving;

        MushroomController.OnTryToStartYarn += TryToStartYarn;

        //YarnCrosshairController.OnYarnCrosshairEnter += StartArcMovement;
        //YarnCrosshairController.OnYarnCrosshairExit += delegate (Vector2 position) { shouldMoveOnArc = false; };

        TutorialManager.OnStageReveale += PauseYarn;
        TutorialManager.OnStageHide += ContinueYarn;

        LevelSceneManager.OnRestart += Restart;
    }
    private void OnDestroy()
    {
        WinState.OnWinGame -= ManageGameEnd;
        LoseState.OnLoseGame -= ManageGameEnd;
        ConquerState.OnConquerStateExit -= StopMoving;
    }
    #endregion
    #region Manage Movement
    private Transform yarnTrailPrefab;
    private TrailRenderer currentYarnTrail;
    private bool shouldMoveYarn;
    private bool isPaused = false;
    private bool isGameEnded = false;
    [SerializeField] Transform yarnTrailContainer;

    [SerializeField] Transform cameraContainerTransform;
    [SerializeField] Transform followCameraTransform;

    private Vector3 startPosition;

    public static event Action<Vector2> OnYarnStart;
    private GameObject moveSound;

    private Material darkerYarnMaterial;
    public Biome startBiome { get; private set; }

    public UnityEvent OnYarnFirstStart;
    public UnityEvent OnYarnSecondStart;
    private bool isFirstStart = true;

    public void TryToStartYarn(Vector3 startPosition)
    {
        if (isGameEnded) return;
        if (shouldMoveYarn) return;
        
        if (!ResourceManager.resourceData.CanSpendResource(idleMovementCost))
        {
            JuiceTextCreator.CreateJuiceText("You don't have enough energy to start yarn", Color.red);
            return;
        } 

        StartYarn(startPosition);

        OnYarnFirstStart?.Invoke();
        OnYarnFirstStart = new UnityEvent();
        if(isFirstStart)
        {
            isFirstStart = false;
            return;
        }
        OnYarnSecondStart?.Invoke();
        OnYarnSecondStart = new UnityEvent();
    }
    private void StartYarn(Vector3 startPosition)
    {
        shouldMoveYarn = true;
        transform.position = startPosition;
        startBiome = BiomeManager.Instance.GetBiomOnPosition(startPosition);
        currentYarnTrail = Instantiate(yarnTrailPrefab, startPosition, Quaternion.identity, transform).GetComponent<TrailRenderer>();
        OnYarnStart?.Invoke(startPosition);

        moveSound = SoundManager.Instance.PlaySoundLooped("Yarn/Movement", transform.position, transform);
    }
    private void StopMoving()
    {
        shouldMoveYarn = false;
        DetacheYarn();

        if(moveSound) SoundManager.Instance.StopSound(moveSound);
    }
    private void DetacheYarn()
    {
        if (!currentYarnTrail) return;

        currentYarnTrail.emitting = false;
        currentYarnTrail.transform.parent = yarnTrailContainer;
        currentYarnTrail.sortingOrder = -3;
        currentYarnTrail.material = darkerYarnMaterial;
        oldVector = Vector2.zero;
    }
    private void ContinueYarn()
    {
        isPaused = false;
        
        if (!currentYarnTrail) return;
        currentYarnTrail.emitting = true;
    }
    private void PauseYarn()
    {
        isPaused = true;
        
        if (!currentYarnTrail) return;
        currentYarnTrail.emitting = false;
    }
    private void ManageMovement()
    {
        if (!shouldMoveYarn) return;
        if (isPaused) return;

        SimpleMovement();  
    }
    private void ManageGameEnd()
    {
        StopMoving();
        isGameEnded = true;
        transform.position = startPosition;
    }
    #endregion
    #region Simple Movement
    [SerializeField] YarnCrosshairController yarnCrosshairController;
    [SerializeField] float idleSpeed, sprintMultiplier;
    private Vector2 oldVector = Vector2.zero;

    private void SimpleMovement()
    {
        Vector2 inputVector = GetInputVector();
        bool isSprint = GetIsSprint(inputVector);
        Vector2 moveVector = GetMoveVector(inputVector, isSprint);
        MoveTransformToDirection(moveVector);

        bool isResourceSpent = SpendMovementCost(isSprint);
        RunOutResource(isResourceSpent);
    }
    private Vector2 GetInputVector()
    {
        Vector2 inputVector = MoveInputUI.Instance.GetInputVector();

        Vector2 cameraUpVector = followCameraTransform.up;
        Quaternion fixRotation = Quaternion.FromToRotation(Vector2.up, cameraUpVector);
        return fixRotation * inputVector;
    }
    private static bool GetIsSprint(Vector2 inputVector)
    {
        float inputRange = MoveInputUI.Instance.GetRange();
        return (inputVector.magnitude > inputRange / 2f);
    }
    private Vector2 GetMoveVector(Vector2 inputVector, bool isSprint)
    {
        Vector2 moveVector = GetIdleMoveVector();

        if (inputVector != Vector2.zero)
        {
            moveVector = GetControlledMoveVector(inputVector, isSprint);
        }
        else if (oldVector != Vector2.zero)
        {
            moveVector = GetPreviousMoveVector();
        }

        return moveVector;
    }
    private Vector2 GetControlledMoveVector(Vector2 inputVector, bool isSprint)
    {
        Vector2 moveVector;
        float speed = (!isSprint) ? idleSpeed : sprintMultiplier;
        moveVector = inputVector.normalized * (speed);
        oldVector = inputVector;
        return moveVector;
    }
    private Vector2 GetPreviousMoveVector()
    {
        return oldVector.normalized * (idleSpeed / 2f);
    }
    private Vector2 GetIdleMoveVector()
    {
        return (cameraContainerTransform.position - transform.position).normalized * (idleSpeed / 2f);
    }
    private void MoveTransformToDirection(Vector3 moveVector)
    {
        transform.position += moveVector * Time.deltaTime;
    }
    #endregion
    #region Resource Management
    public static event Action OnRunOutOfYarnResource;
    
    [SerializeField] ResourceUnit idleMovementCost, sprintMovementCost;
    
    private bool SpendMovementCost(bool isSprint)
    {
        float movementCost = DecideMovementCost(isSprint) * Time.deltaTime;
        ResourceUnit resourceUnit = new ResourceUnit(idleMovementCost.type, movementCost);
        bool isResourceSpent = ResourceManager.Instance.TryToSpendResource(resourceUnit);
        return isResourceSpent;
    }
    private float DecideMovementCost(bool isSprint)
    {
        float movementCost = (!isSprint) ? idleMovementCost.amount : idleMovementCost.amount * sprintMultiplier;
        return movementCost;
    }
    private void RunOutResource(bool isResourceSpent)
    {
        if (!isResourceSpent)
        {
            StopMoving();
            JuiceTextCreator.CreateJuiceText("You ran out of energy", Color.red);
            OnRunOutOfYarnResource?.Invoke();
        }
    }
    #endregion  
    private void Restart()
    {
        isGameEnded = false;

        foreach (Transform child in yarnTrailContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
