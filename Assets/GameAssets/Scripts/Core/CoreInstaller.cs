using UnityEngine;

[DefaultExecutionOrder(-100)]
public class CoreInstaller : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private ShooterManager shooterManager;
    [SerializeField] private FXManager fxManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GameInputController gameInputController;
    [SerializeField] private UIManager uiManager;

    [Header("State Machines")]
    [SerializeField] private MainStateMachine mainStateMachine;
    [SerializeField] private UIStateMachine uiStateMachine;

    [Header("Save & Data")]
    [SerializeField] private DataManager dataManager;

    [Header("Settings (ScriptableObjects)")]
    [SerializeField] private GameSettings gameSettings;

    [Header("Prefabs & Pools")]
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private GameObject levelPrefab;

    private void Awake()
    {
        Install();
    }

    private void Install()
    {
        dataManager.Inject(new JsonSaveSystem());
        levelManager.Inject(dataManager, gameSettings, levelPrefab, objectPool);
        shooterManager.Inject(levelManager, gameSettings, fxManager);
        gameInputController.Inject(inputManager, shooterManager);
        uiStateMachine.Inject(uiManager, gameSettings);
        uiManager.Inject(levelManager, gameSettings);
        gameManager.Inject(mainStateMachine, uiStateMachine, levelManager, fxManager, uiManager, shooterManager);
    }

    private void Start()
    {
        dataManager.Initialize();
        levelManager.Initialize();
        mainStateMachine.Initialize();
        uiStateMachine.Initialize();
        gameManager.Initialize();
    }
}
