using UnityEngine;

public class Node : MonoBehaviour, IPoolObject
{
    public bool IsFull { get; protected set; }
    public INodeObject NodeObject { get; private set; }
    protected GridSystem _gridSystem;

    public Vector2Int GridPosition { get; private set; }

    public void Initialize(GridSystem system)
    {
        _gridSystem = system;
    }

    public void SetCoordination(int x, int y)
    {
        GridPosition = new Vector2Int(x, y);
    }

    public virtual void AssignNodeObject(INodeObject nodeObj)
    {
        NodeObject = nodeObj;
        IsFull = true;
    }

    public virtual void SetEmpty(INodeObject nodeObj)
    {
        if (nodeObj != NodeObject) return;
        IsFull = false;
        NodeObject = null;
    }

    public virtual void Reset()
    {
        IsFull = false;
        NodeObject = null;
        _gridSystem = null;
        GridPosition = new Vector2Int(-1, -1);
    }
}
