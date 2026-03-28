using RubyCase.Data;
using RubyCase.Event;
using RubyCase.Level;
using RubyCase.UI;

namespace RubyCase.StateMachine
{
    public class UIStateMachine : StateManager<UIStateMachine.UIState>
    {
        public enum UIState
        {
            Start,
            InGame,
            LevelEnd,
        }

        private UIManager _uiManager;
        private GameSettings _settings;

        public void Inject(UIManager uiManager, GameSettings settings)
        {
            _uiManager = uiManager;
            _settings = settings;
        }

        private void OnEnable()
        {
            GameEvents.OnLevelEnd += OnLevelEnd;
            GameEvents.OnLevelStart += OnLevelStart;
        }

        private void OnDisable()
        {
            GameEvents.OnLevelEnd -= OnLevelEnd;
            GameEvents.OnLevelStart -= OnLevelStart;
        }

        protected override void SetStates()
        {
            var startUI = new StartUIState(UIState.Start, UIState.InGame);
            var inGameUI = new InGameUIState(UIState.InGame, UIState.LevelEnd, _uiManager);
            var endUI = new LevelEndUIState(UIState.LevelEnd, UIState.Start, _settings, _uiManager);

            States.Add(UIState.Start, startUI);
            States.Add(UIState.InGame, inGameUI);
            States.Add(UIState.LevelEnd, endUI);

            SetStateWithKey(UIState.Start);
        }

        private void OnLevelEnd(bool isWin, int levelIndex)
        {
            if (GetCurrentState() == UIState.LevelEnd)
            {
                return;
            }

            (States[UIState.LevelEnd] as LevelEndUIState)?.SetWin(isWin);
            SetStateWithKey(UIState.LevelEnd);
        }

        private void OnLevelStart(int levelIndex, Level.Level level)
        {
            if (GetCurrentState() == UIState.InGame)
            {
                return;
            }
            SetStateWithKey(UIState.InGame);
        }
    }
}
