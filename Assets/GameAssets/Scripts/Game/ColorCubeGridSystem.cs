using UnityEngine;
using RubyCase.Pools;

namespace RubyCase.Game
{
    public class ColorCubeGridSystem : GridSystem
    {
        public override void Init(ObjectPool pool, Vector2Int size = default)
        {
            gridWidth = size.x;
            gridHeight = size.y;

            base.Init(pool, size);
        }

        public bool IsPictureComplete()
        {
            for (int x = 0; x < _nodes.GetLength(0); x++)
            {
                for (int y = 0; y < _nodes.GetLength(1); y++)
                {
                    if (_nodes[x, y].IsFull) return false;
                }
            }

            return true;
        }
    }
}
