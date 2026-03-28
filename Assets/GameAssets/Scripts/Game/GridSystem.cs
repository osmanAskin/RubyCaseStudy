using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using RubyCase.Pools;

namespace RubyCase.Game
{
    public abstract class GridSystem : MonoBehaviour
    {
        public int gridWidth;
        public int gridHeight;

        public bool coordinateXY;
        public bool coordinateXZ;
        public bool coordinateYZ;

        [OnValueChanged(nameof(OnInspectorChanged))]
        public float centerX;
        [OnValueChanged(nameof(OnInspectorChanged))]
        public float centerY;
        [OnValueChanged(nameof(OnInspectorChanged))]
        public float centerZ;

        [OnValueChanged(nameof(OnInspectorChanged))]
        public float gridSpaceX = 1f;
        [OnValueChanged(nameof(OnInspectorChanged))]
        public float gridSpaceY = 1f;
        [OnValueChanged(nameof(OnInspectorChanged))]
        public float gridSpaceZ = 1f;

        protected Node[,] _nodes;

        public PoolTags nodePoolTag;

        [Header("Z Position Control")]
        [OnValueChanged(nameof(OnInspectorChanged))]
        public bool useCustomZStart;
        [OnValueChanged(nameof(OnInspectorChanged))]
        public float customZStart;

        [OnValueChanged(nameof(OnInspectorChanged))]
        public bool useCustomZEnd;
        [OnValueChanged(nameof(OnInspectorChanged))]
        public float customZEnd;

        public virtual void Init(ObjectPool pool, Vector2Int size = default)
        {
            SpawnNodes(pool, transform);
        }

        private void SpawnNodes(ObjectPool pool, Transform parent = null, bool editorSpawn = false)
        {
            _nodes = new Node[gridWidth, gridHeight];

            for (var i = 0; i < gridWidth; i++)
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

        public void ResetSystem()
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

        private void OnInspectorChanged()
        {
            UpdateNodePositions();
        }

        protected virtual void UpdateNodePositions()
        {
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    _nodes[i, j].transform.position = GetNodePosition(i, j);
                }
            }
        }
    }
}
