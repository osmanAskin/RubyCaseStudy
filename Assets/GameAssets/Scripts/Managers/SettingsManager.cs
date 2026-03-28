using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private bool _vibrationEnabled;

    public bool IsVibrationEnabled => _vibrationEnabled;

    public void Inject()
    {
        // No dependencies needed
    }

    public void Initialize()
    {
        _vibrationEnabled = PlayerPrefs.GetInt(Constants.HapticKey, 1) == 1;
    }

    public void SetVibration(bool value)
    {
        _vibrationEnabled = value;
        PlayerPrefs.SetInt(Constants.HapticKey, value ? 1 : 0);
        PlayerPrefs.Save();
    }
}
