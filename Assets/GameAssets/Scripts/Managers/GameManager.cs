using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using RubyCase.Event;
using RubyCase.Game;
using RubyCase.Level;
using RubyCase.StateMachine;

namespace RubyCase.Managers
{
    public class GameManager : MonoBehaviour
    {
        private MainStateMachine _mainStateMachine;
        private UIStateMachine _uiStateMachine;
        private LevelManager _levelManager;
        private CollectableBoxManager _collectableBoxManager;

        private CancellationTokenSource _restartCts;

        public void Inject(MainStateMachine mainStateMachine, UIStateMachine uiStateMachine,
            LevelManager levelManager, CollectableBoxManager collectableBoxManager)
        {
            _mainStateMachine = mainStateMachine;
            _uiStateMachine = uiStateMachine;
            _levelManager = levelManager;
            _collectableBoxManager = collectableBoxManager;
        }

        private void OnEnable()
        {
            GameEvents.OnRestart += OnRestart;
            GameEvents.OnLevelSetupRequested += HandleSetupRequest;
            GameEvents.OnLevelEndRequested += HandleLevelEndRequest;
        }

        private void OnDisable()
        {
            GameEvents.OnRestart -= OnRestart;
            GameEvents.OnLevelSetupRequested -= HandleSetupRequest;
            GameEvents.OnLevelEndRequested -= HandleLevelEndRequest;
        }

        public void Initialize()
        {
            SetUpLevel();
            StartLevel();
        }

        private void SetUpLevel()
        {
            _collectableBoxManager.ResetSystem();
            _mainStateMachine.SetStateWithKey(MainStateMachine.MainState.Start);
            _levelManager.SetupLevel();
            _uiStateMachine.SetStateWithKey(UIStateMachine.UIState.InGame);
            _mainStateMachine.SetStateWithKey(MainStateMachine.MainState.Game);
        }

        private void OnRestart(bool hard)
        {
            RestartLevel();
        }

        private void HandleSetupRequest()
        {
            RestartLevel();
        }

        private void HandleLevelEndRequest(bool isWin)
        {
            if (_mainStateMachine.GetCurrentState() == MainStateMachine.MainState.Finish)
            {
                return;
            }
            EndLevel(isWin);
        }

        public void RestartLevel()
        {
            GameEvents.LevelRestart(_levelManager.CurrentLevelNo);
            SetUpLevel();
            StartLevel();
        }

        public void StartLevel()
        {
            GameEvents.LevelStart(_levelManager.CurrentLevelNo, _levelManager.CurrentLevel);
        }

        public void EndLevel(bool isWin)
        {
            if (isWin)
            {
                LevelCompleted();
            }
            else
            {
                LevelFailed();
            }

            GameEvents.LevelEnd(isWin, _levelManager.CurrentLevelNo);
        }

        private void LevelCompleted()
        {
            _levelManager.NextLevel();
            Debug.Log("Level completed.");
        }

        private void LevelFailed()
        {
            Debug.Log("Level failed.");
            _levelManager.CurrentLevel.Conveyor.LevelFailed();
        }
    }
}
