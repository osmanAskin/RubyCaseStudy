using System;

public static class GameEvents
{
    // UI → Game
    public static event Action<bool> OnRestart;
    public static event Action OnLevelSetupRequested;

    // Game → Managers
    public static event Action<int, Level> OnLevelStart;
    public static event Action<bool, int> OnLevelEnd;
    public static event Action<int> OnLevelCompleted;
    public static event Action<int> OnLevelRestart;

    // Internal requests (ShooterManager → GameManager)
    public static event Action<bool> OnLevelEndRequested;

    public static void Restart(bool hard) => OnRestart?.Invoke(hard);
    public static void LevelSetupRequested() => OnLevelSetupRequested?.Invoke();
    public static void LevelStart(int levelIndex, Level level) => OnLevelStart?.Invoke(levelIndex, level);
    public static void LevelEnd(bool isWin, int levelIndex) => OnLevelEnd?.Invoke(isWin, levelIndex);
    public static void LevelCompleted(int levelIndex) => OnLevelCompleted?.Invoke(levelIndex);
    public static void LevelRestart(int levelIndex) => OnLevelRestart?.Invoke(levelIndex);
    public static void LevelEndRequested(bool isWin) => OnLevelEndRequested?.Invoke(isWin);
}
