using UnityEngine;
using RubyCase.Data;
using RubyCase.Event;
using RubyCase.Pools;
using RubyCase.SaveSystem;

namespace RubyCase.Level
{
    public class LevelManager : MonoBehaviour
    {
        public int CurrentLevelNo { get; private set; }
        public Level CurrentLevel { get; private set; }

        private LevelGenerator _generator;
        private DataManager _dataManager;
        private GameSettings _gameSettings;

        public void Inject(DataManager dataManager, GameSettings settings, GameObject levelPrefab, ObjectPool pool)
        {
            _dataManager = dataManager;
            _gameSettings = settings;
            _generator = new LevelGenerator(this, levelPrefab, settings, pool);
        }

        public void SetupLevel()
        {
            var levelData = _gameSettings.GetLevel(CurrentLevelNo);
            CurrentLevel = _generator.GenerateLevel(levelData);
        }

        public void NextLevel()
        {
            FireLevelCompleted(CurrentLevelNo);
            CurrentLevelNo = (CurrentLevelNo + 1) % _gameSettings.levels.Length;
        }

        public void Initialize()
        {
            CurrentLevelNo = _dataManager.GameData.currentLevelNumber % _gameSettings.levels.Length;
        }

        private void FireLevelCompleted(int completedLevelIndex)
        {
            GameEvents.LevelCompleted(completedLevelIndex);
        }
    }
}
