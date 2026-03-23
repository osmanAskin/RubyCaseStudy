using
  System.Collections.Generic;
  using UnityEngine;

  public class GridSystem :
  MonoBehaviour
  {
      [SerializeField] private
  GameObject colorBlockPrefab;
      [SerializeField] private
  float cellSize = 1f;

      private ColorBlock[,]
  _blocks;
      public Vector2Int GridSize {
   get; private set; }

      public void Init(LevelData
  data)
      {
          GridSize =
  data.colorCubeGridSize;
          _blocks = new
  ColorBlock[GridSize.x,
  GridSize.y];

          for (int x = 0; x <
  GridSize.x; x++)
          {
              for (int y = 0; y <
  GridSize.y; y++)
              {
                  Color pixel =
  data.levelTexture.GetPixel(x,
  y);
                  Color closest =
  FindClosestColor(pixel,
  data.levelColors,
  data.colorThreshold);

                  Vector3 worldPos
   = GetWorldPosition(x, y);
                  GameObject go =
  Instantiate(colorBlockPrefab,
  worldPos, Quaternion.identity,
  transform);
                  ColorBlock block
   =
  go.GetComponent<ColorBlock>();

  block.Init(closest, new
  Vector2Int(x, y));
                  _blocks[x, y] =
  block;
              }
          }
      }

      public Vector3
  GetWorldPosition(int x, int y)
      {
          return
  transform.position + new
  Vector3(x * cellSize, 0f, y *
  cellSize);
      }

      public ColorBlock
  GetBlock(int x, int y)
      {
          if (x < 0 || x >=
  GridSize.x || y < 0 || y >=
  GridSize.y) return null;
          return _blocks[x, y];
      }

      public void RemoveBlock(int 
  x, int y)
      {
          if (_blocks[x, y] ==
  null) return;
          _blocks[x, y].Collect();
          _blocks[x, y] = null;
      }

      public bool IsEmpty()
      {
          foreach (var block in
  _blocks)
              if (block != null)
  return false;
          return true;
      }

      private Color
  FindClosestColor(Color pixel,
  List<LevelData.LevelColorData>
  colors, float threshold)
      {
          Color closest = pixel;
          float minDist =
  float.MaxValue;

          foreach (var lc in
  colors)
          {
              float dist =
  ColorDistance(pixel, lc.color);
              if (dist < minDist)
              {
                  minDist = dist;
                  closest =
  lc.color;
              }
          }
          return closest;
      }

      private float 
  ColorDistance(Color a, Color b)
      {
          float dr = a.r - b.r;
          float dg = a.g - b.g;
          float db = a.b - b.b;
          return Mathf.Sqrt(dr *
  dr + dg * dg + db * db);
      }
  }