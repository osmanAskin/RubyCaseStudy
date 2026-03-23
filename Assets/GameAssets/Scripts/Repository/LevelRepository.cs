using UnityEngine;

public class LevelRepository : ILevelRepository
{
    public LevelData GetLevel(int levelIndex)
    {
        return Resources.Load<LevelData>("Levels/Level_" + levelIndex);
    }

    public int GetLevelCount()
    {
        // TODO: Resources.LoadAll<LevelData>("Levels") ile gerçek sayıyı döndür
        return 0;
    }
}