using UnityEngine;

public class ShooterGridSystem : GridSystem
{
    public override void Init(ObjectPool pool, Vector2Int size = default)
    {
        gridWidth = size.x;
        gridHeight = size.y;

        base.Init(pool, size);
    }

    public void TransferShooters(int shooterXValue)
    {
        for (int i = _nodes.GetLength(1) - 2; i >= 0; i--)
        {
            var from = _nodes[shooterXValue, i];
            var to = _nodes[shooterXValue, i + 1];
            if (to.IsFull) continue;
            if (!from.IsFull) continue;

            Transfer(from as ShooterNode, to as ShooterNode);
        }
    }

    public void Transfer(ShooterNode from, ShooterNode to)
    {
        var shooter = from.NodeObject as Shooter;
        from.SetEmpty(shooter);
        to.AssignNodeObject(shooter);
        shooter.SetNewNode(to);
    }

    public int GetCurrentShooterCount()
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
