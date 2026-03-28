using UnityEngine;

public class DataManager : MonoBehaviour
{
    private GameData _gameData;
    private ISaveSystem _saveSystem;

    public GameData GameData => _gameData;
    
    public void Inject(ISaveSystem saveSystem)
    {
        _saveSystem = saveSystem;
    }

    private void OnEnable()
    {
        GameEvents.OnLevelCompleted += OnLevelCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnLevelCompleted -= OnLevelCompleted;
    }

    private void OnDestroy()
    {
        SaveData();
    }

    public void Initialize()
    {
        LoadData();
    }

    private void SaveData()
    {
        if (_saveSystem.Save(Constants.GameDataKey, _gameData))
        {
            Debug.Log("Data is saved successfully.");
        }
        else
        {
            Debug.LogWarning("Data cannot be saved.");
        }
    }

    private void LoadData()
    {
        if (!_saveSystem.HasKey(Constants.GameDataKey))
        {
            _gameData = new GameData();
            SaveData();
        }
        else if (_saveSystem.TryGet(Constants.GameDataKey, out _gameData))
        {
            Debug.Log("Data is loaded successfully.");
        }
        else
        {
            Debug.LogError("Data cannot be loaded.");
            Application.Quit();
        }
    }

    private void OnLevelCompleted(int levelIndex)
    {
        _gameData.currentLevelNumber = levelIndex + 1;
        SaveData();
    }
}
