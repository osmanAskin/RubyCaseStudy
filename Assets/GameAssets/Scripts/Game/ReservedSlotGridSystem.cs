using UnityEngine;
using RubyCase.Pools;

namespace RubyCase.Game
{
    public class ReservedSlotGridSystem : GridSystem
    {
        public override void Init(ObjectPool pool, Vector2Int size = default)
        {
            gridWidth = size.x;
            gridHeight = size.y;

            base.Init(pool, size);
        }

        public ReservedSlot GetAvailableSlot()
        {
            for (int i = 0; i < _nodes.GetLength(0); i++)
            {
                for (int j = 0; j < _nodes.GetLength(1); j++)
                {
                    if (_nodes[i, j].IsFull) continue;

                    return _nodes[i, j] as ReservedSlot;
                }
            }

            return null;
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

        public void SetWarningEffect()
        {
            for (int i = 0; i < _nodes.GetLength(0); i++)
            {
                for (int j = 0; j < _nodes.GetLength(1); j++)
                {
                    if (!_nodes[i, j].IsFull)
                    {
                        ActivateWarningEffect(false);
                        return;
                    }
                }
            }

            ActivateWarningEffect(true);
        }

        private void ActivateWarningEffect(bool isActive)
        {
            for (int i = 0; i < _nodes.GetLength(0); i++)
            {
                for (int j = 0; j < _nodes.GetLength(1); j++)
                {
                    var node = _nodes[i, j] as ReservedSlot;
                    node.ActivateWarningEffect(isActive);
                }
            }
        }

        public void SetSlotValues(float reservedSlotWarningEffectDuration, int reservedSlotWarningEffectCount)
        {
            for (int i = 0; i < _nodes.GetLength(0); i++)
            {
                for (int j = 0; j < _nodes.GetLength(1); j++)
                {
                    var slot = _nodes[i, j] as ReservedSlot;
                    slot.SetEffectValues(reservedSlotWarningEffectDuration, reservedSlotWarningEffectCount);
                }
            }
        }

        public void TransferCollectableBoxes()
        {
            for (int i = 1; i < _nodes.GetLength(0); i++)
            {
                var from = _nodes[i, 0];
                var to = _nodes[i - 1, 0];
                if (to.IsFull) continue;
                if (!from.IsFull) continue;

                Transfer(from as ReservedSlot, to as ReservedSlot);
            }
        }

        public void Transfer(ReservedSlot from, ReservedSlot to)
        {
            var collectableBox = from.NodeObject as CollectableBox;
            from.SetEmpty(collectableBox);
            to.AssignNodeObject(collectableBox);
            collectableBox.SetReservedSlot(to);
        }
    }
}
