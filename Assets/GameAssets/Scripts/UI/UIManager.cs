using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private UIGame pageGame;
    [SerializeField] private UIWin pageWin;
    [SerializeField] private UIFail pageFail;

    private LevelManager _levelManager;
    private GameSettings _gameSettings;

    public void Inject(LevelManager levelManager, GameSettings gameSettings, ISettingsProvider settingsProvider)
    {
        _levelManager = levelManager;
        _gameSettings = gameSettings;

        pageGame.Inject(levelManager);
        pageGame.Construct(this);
        pageWin.Inject(gameSettings);
        pageWin.Construct(this);
        pageFail.Construct(this);
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
