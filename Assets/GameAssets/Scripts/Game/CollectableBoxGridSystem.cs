using UnityEngine;
using RubyCase.Pools;

namespace RubyCase.Game
{
    public class CollectableBoxGridSystem : GridSystem
    {
        public override void Init(ObjectPool pool, Vector2Int size = default)
        {
            gridWidth = size.x;
            gridHeight = size.y;

            base.Init(pool, size);
        }

        public void TransferCollectableBoxes(int collectableBoxXValue)
        {
            for (int i = _nodes.GetLength(1) - 2; i >= 0; i--)
            {
                var from = _nodes[collectableBoxXValue, i];
                var to = _nodes[collectableBoxXValue, i + 1];
                if (to.IsFull) continue;
                if (!from.IsFull) continue;

                Transfer(from as CollectableBoxNode, to as CollectableBoxNode);
            }
        }

        public void Transfer(CollectableBoxNode from, CollectableBoxNode to)
        {
            var collectableBox = from.NodeObject as CollectableBox;
            from.SetEmpty(collectableBox);
            to.AssignNodeObject(collectableBox);
            collectableBox.SetNewNode(to);
        }

        public int GetCurrentCollectableBoxCount()
        {
            var count = 0;
            for (int i = 0; i < _nodes.GetLength(0); i++)
            {
                for (int j = 0; j < _nodes.GetLength(1); j++)
                {
                    if (_nodes[i, j].IsFull)
                        count++;
                }
            }

            return count;
        }
    }
}
