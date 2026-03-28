using UnityEngine;
using UnityEngine.UI;

public class UISetting : UIPopup
{
    [SerializeField] private Image imgHapticBtn;
    [SerializeField] private Sprite[] sprButton;

    private ISettingsProvider _settingsProvider;

    public void Inject(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }

    protected override void Start()
    {
        base.Start();
        UpdateButtonSprites();
    }

    public void OnToggleHapticBtn()
    {
        bool newValue = !_settingsProvider.IsVibrationEnabled;
        _settingsProvider.SetVibration(newValue);
        UpdateButtonSprites();
    }

    public override void OnTapClose()
    {
        Close();
    }

    public void OnTapHomeBtn()
    {
        _uiManager.ShowHome();
        Close();
    }

    private void UpdateButtonSprites()
    {
        imgHapticBtn.sprite = sprButton[_settingsProvider.IsVibrationEnabled ? 1 : 0];
    }
}
