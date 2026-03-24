using UnityEngine;              
                                  
public class ColorBlock :       
    MonoBehaviour   
{
    public Color BlockColor {   
        get; private set; }
    public Vector2Int           
        GridPosition { get; private set;
    }
    public bool IsCollected {
        get; private set; }

    public void Init(Color
        color, Vector2Int gridPos)
    {
        BlockColor = color;
        GridPosition = gridPos;
        GetComponent<Renderer>()
            .material.color = color;
    }

    public void Collect()
    {
        IsCollected = true;
        Destroy(gameObject);
    }
}