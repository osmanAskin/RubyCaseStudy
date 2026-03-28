using System;
using UnityEngine;

[Serializable]
public class CellData
{
    public Vector2Int coordinates;
    public Color cellColor;
    public int shootCount;
    public int colorID;

    public CellData(Vector2Int coordinates)
    {
        this.coordinates = coordinates;
        shootCount = 0;
        cellColor = Color.gray;
    }
}
