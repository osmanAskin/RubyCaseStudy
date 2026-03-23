using UnityEngine;

public class LevelGenerators : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    [SerializeField] private GridSystem gridSystem;

    void Start()
    {
        gridSystem.Init(levelData);
    }
}
