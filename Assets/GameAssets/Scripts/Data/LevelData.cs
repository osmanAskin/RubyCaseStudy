using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "RubyStudyCase/LevelData")]
public class LevelData : ScriptableObject
{
    public int gridWidth;
    public int gridHeight;
    public BoxInfo[] boxes;
    public ProductInfo[] products;
    public Vector2Int[] disabledCells;
}
