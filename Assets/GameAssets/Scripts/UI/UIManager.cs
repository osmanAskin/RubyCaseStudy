using System;
using UnityEditor;
using UnityEngine;
using RubyCase.Data;
using RubyCase.Level;
using RubyCase.UI.Pages;

namespace RubyCase.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Pages")]
        [SerializeField] private UIGame pageGame;
        [SerializeField] private UIWin pageWin;
        [SerializeField] private UIFail pageFail;

        public void Inject(LevelManager levelManager, GameSettings gameSettings)
        {
            pageGame.Inject(levelManager);
            pageWin.Inject(gameSettings);
        }

        private void Awake()
        {
            pageGame.gameObject.SetActive(false);
            pageWin.gameObject.SetActive(false);
            pageFail.gameObject.SetActive(false);
        }

        private void HideAllPages()
        {
            pageGame.gameObject.SetActive(false);
            pageWin.gameObject.SetActive(false);
            pageFail.gameObject.SetActive(false);
        }

        public UIGame ShowGame()
        {
            HideAllPages();
            pageGame.gameObject.SetActive(true);
            return pageGame;
        }

        public void ShowCompleted(Action onCompleted = null)
        {
            HideAllPages();
            pageWin.Build(onCompleted);
            pageWin.gameObject.SetActive(true);
        }

        public void ShowFail()
        {
            HideAllPages();
            pageFail.gameObject.SetActive(true);
        }

        public void Hide(GameObject instance)
        {
            if (instance != null)
                instance.SetActive(false);
        }
    }
}
