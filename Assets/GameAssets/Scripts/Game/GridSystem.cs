using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public abstract class GridSystem : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;

    public bool coordinateXY;
    public bool coordinateXZ;
    public bool coordinateYZ;

#if UNITY_EDITOR
    [OnValueChanged(nameof(OnInspectorChanged))]
#endif
    public float centerX;
#if UNITY_EDITOR
    [OnValueChanged(nameof(OnInspectorChanged))]
#endif
    public float centerY;
#if UNITY_EDITOR
    [OnValueChanged(nameof(OnInspectorChanged))]
#endif
    public float centerZ;

#if UNITY_EDITOR
    [OnValueChanged(nameof(OnInspectorChanged))]
#endif
    public float gridSpaceX = 1f;
#if UNITY_EDITOR
    [OnValueChanged(nameof(OnInspectorChanged))]
#endif
    public float gridSpaceY = 1f;
#if UNITY_EDITOR
    [OnValueChanged(nameof(OnInspectorChanged))]
#endif
    public float gridSpaceZ = 1f;

    protected Node[,] _nodes;

    public PoolTags nodePoolTag;

    [Header("Z Position Control")]
#if UNITY_EDITOR
    [OnValueChanged(nameof(OnInspectorChanged))]
#endif
    public bool useCustomZStart;
#if UNITY_EDITOR
    [OnValueChanged(nameof(OnInspectorChanged))]
#endif
    public float customZStart;

#if UNITY_EDITOR
    [OnValueChanged(nameof(OnInspectorChanged))]
#endif
    public bool useCustomZEnd;
#if UNITY_EDITOR
    [OnValueChanged(nameof(OnInspectorChanged))]
#endif
    public float customZEnd;

    public virtual void Init(ObjectPool pool, Vector2Int size = default)
    {
        SpawnNodes(pool, transform);
    }

    protected virtual void SpawnNodes(ObjectPool pool, Transform parent = null, bool editorSpawn = false)
    {
        _nodes = new Node[gridWidth, gridHeight];

        float halfRowSize = (gridWidth - 1) * (1 + gridSpaceX) / 2f;
        float halfColumnSizeY = (gridHeight - 1) * (1 + gridSpaceY) / 2f;
        float halfColumnSizeZ = (gridHeight - 1) * (1 + gridSpaceZ) / 2f;

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                var position = GetNodePosition(i, j);

                var node = pool.SpawnFromPool(nodePoolTag, position, Quaternion.identity).GetComponent<Node>();
                node.transform.SetParent(parent);

                node.SetCoordination(i, j);
                node.Initialize(this);

                _nodes[i, j] = node;

                node.gameObject.name = "Node_" + i + "_" + j;
            }
        }
    }

    public Node[] GetAllNodes()
    {
        var nodeList = new List<Node>();
        for (int i = 0; i < _nodes.GetLength(0); i++)
        {
            for (int j = 0; j < _nodes.GetLength(1); j++)
            {
                nodeList.Add(_nodes[i, j]);
            }
        }

        return nodeList.ToArray();
    }

    public virtual void ResetSystem()
    {
        for (int i = 0; i < _nodes.GetLength(0); i++)
        {
            for (int j = 0; j < _nodes.GetLength(1); j++)
            {
                _nodes[i, j].Reset();
            }
        }
    }

    public Node GetNode(int x, int y)
    {
        if (x < 0 || x >= _nodes.GetLength(0) || y < 0 || y >= _nodes.GetLength(1))
        {
            return null;
        }

        return _nodes[x, y];
    }

    public virtual Vector3 GetNodePosition(int row, int column)
    {
        float halfRowSize = (gridWidth - 1) * (gridSpaceX) / 2f;
        float halfColumnSizeY = (gridHeight - 1) * (gridSpaceY) / 2f;
        float halfColumnSizeZ = (gridHeight - 1) * (gridSpaceZ) / 2f;

        float xPosition = coordinateYZ ? centerX : row * (gridSpaceX) - halfRowSize + centerX;
        float yPosition = coordinateXZ ? centerY : column * (gridSpaceY) - halfColumnSizeY + centerY;
        float zPosition;

        if (coordinateXY)
        {
            zPosition = centerZ;
        }
        else if (useCustomZStart)
        {
            zPosition = customZStart + column * (gridSpaceZ);
        }
        else if (useCustomZEnd)
        {
            zPosition = customZEnd - (gridHeight - 1 - column) * (gridSpaceZ);
        }
        else
        {
            zPosition = column * (gridSpaceZ) - halfColumnSizeZ + centerZ;
        }

        return new Vector3(xPosition, yPosition, zPosition);
    }

#if UNITY_EDITOR
    private void OnInspectorChanged()
    {
        if (_nodes == null || _nodes.Length == 0)
            return;

        UpdateNodePositions();
        SceneView.RepaintAll();
    }
#endif

    protected virtual void UpdateNodePositions()
    {
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                if (_nodes[i, j] == null) continue;
                _nodes[i, j].transform.position = GetNodePosition(i, j);
#if UNITY_EDITOR
                EditorUtility.SetDirty(_nodes[i, j]);
#endif
            }
        }
    }
}
