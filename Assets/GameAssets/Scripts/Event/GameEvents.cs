using System;

public static class GameEvents
{
    public static event Action OnLevelStarted;
    public static event Action OnLevelCompleted;
    public static event Action OnLevelFailed;

    public static event Action<BoxInfo> OnBoxSpawned;
    public static event Action<BoxInfo> OnBoxFilled;
    public static event Action<BoxInfo> OnBoxBenched;

    public static event Action OnBenchFull;

    public static void ClearAll()
    {
        OnLevelStarted = null;
        OnLevelCompleted = null;
        OnLevelFailed = null;
        OnBoxSpawned = null;
        OnBoxFilled = null;
        OnBoxBenched = null;
        OnBenchFull = null;
    }
}