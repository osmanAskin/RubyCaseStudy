using UnityEngine;

public class LevelGenerator
{
    private LevelManager _levelManager;
    private GameObject _levelPrefab;
    private GameSettings _gameSettings;
    private ObjectPool _objectPool;

    public LevelGenerator(LevelManager levelManager, GameObject levelPrefab, GameSettings gameSettings,
        ObjectPool objectPool)
    {
        _levelManager = levelManager;
        _levelPrefab = levelPrefab;
        _gameSettings = gameSettings;
        _objectPool = objectPool;
    }

    public Level GenerateLevel(LevelData data)
    {
        ResetLevel();

        if (data == null)
        {
            Debug.LogError("[LevelGenerator] Level data null! GameSettings.levels array may be empty.");
            return null;
        }

        Level levelInstance = Object.Instantiate(_levelPrefab).GetComponent<Level>();

        levelInstance.Init(data, _gameSettings, _objectPool);
        return levelInstance;
    }

    private void ResetLevel()
    {
        Time.timeScale = 1;
        if (_levelManager.CurrentLevel == null) return;

        _levelManager.CurrentLevel.Reset();

        Object.Destroy(_levelManager.CurrentLevel.gameObject);
    }
}
