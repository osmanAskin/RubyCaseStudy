using System;
using UnityEngine;

public class UIWin : UIPage
{
    [SerializeField] private ParticleSystem[] confettis;
    [SerializeField] private GameObject goPage;
    [SerializeField] private GameObject goButton;

    private Action _onCompleted;
    private GameSettings _gameSettings;

    public void Inject(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;
    }

    public void Build(Action onCompleted = null)
    {
        _onCompleted = onCompleted;
    }

    private void OnEnable()
    {
        if (_gameSettings == null) return;

        goPage.SetActive(true);
        goButton.SetActive(true);

        foreach (var confetti in confettis)
        {
            confetti.Play();
        }
    }

    public void OnTapContinue()
    {
        goButton.SetActive(false);
        _onCompleted?.Invoke();
        GameEvents.LevelSetupRequested();
    }
}
