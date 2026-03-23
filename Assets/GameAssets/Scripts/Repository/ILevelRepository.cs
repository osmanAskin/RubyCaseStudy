public interface ILevelRepository
{
    LevelData GetLevel(int levelIndex);
    int GetLevelCount();
}