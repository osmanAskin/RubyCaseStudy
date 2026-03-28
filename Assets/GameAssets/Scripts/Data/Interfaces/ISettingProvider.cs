public interface ISettingsProvider
{
    bool IsVibrationEnabled { get; }
    void SetVibration(bool value);
}