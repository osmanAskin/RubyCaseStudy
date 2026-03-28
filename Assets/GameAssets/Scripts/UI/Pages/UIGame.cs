using TMPro;
using UnityEngine;
using RubyCase.Event;
using RubyCase.Level;

namespace RubyCase.UI.Pages
{
    public class UIGame : UIPage
    {
        [SerializeField] private TextMeshProUGUI lblLevel;
        [SerializeField] private RectTransform rectLevel;

        private LevelManager _levelManager;

        public void Inject(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        private void OnEnable()
        {
            if (_levelManager != null)
                lblLevel.text = $"Level {_levelManager.CurrentLevelNo + 1}";
        }

        public void OnTapToRestart()
        {
            GameEvents.Restart(true);
        }
    }
}
