using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Serializable]
    public class Pool
    {
        public PoolTags tag;
        public GameObject prefab;
        public int size;
        [HideInInspector] public Transform poolParent;
    }

    private GameObject POOL_PARENT;
    public List<Pool> pools;
    private Dictionary<PoolTags, Queue<GameObject>> _poolDictionary;

    private void Awake()
    {
        PreparePools();
    }

    private void PreparePools()
    {
        POOL_PARENT = new GameObject("POOL_PARENT");
        _poolDictionary = new Dictionary<PoolTags, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            if (pool.tag == PoolTags.None)
            {
                continue;
            }

            pool.poolParent = new GameObject(pool.tag.ToString()).transform;
            pool.poolParent.SetParent(POOL_PARENT.transform);

            var objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                var obj = Instantiate(pool.prefab, pool.poolParent, true);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                obj.name = obj.name + " clone " + i;
            }

            _poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(PoolTags poolTag, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(poolTag))
        {
            return null;
        }

        if (_poolDictionary[poolTag].Count == 0)
        {
            SpawnNewObject(poolTag, 1);
        }

        var objectToSpawn = _poolDictionary[poolTag].Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    public void DestroyToPool(PoolTags poolTag, GameObject poolObj)
    {
        if (!_poolDictionary.ContainsKey(poolTag))
        {
            return;
        }

        poolObj.SetActive(false);

        var iPool = poolObj.GetComponent<IPoolObject>();
        iPool?.Reset();

        _poolDictionary[poolTag].Enqueue(poolObj);
    }

    private void SpawnNewObject(PoolTags poolTag, int count)
    {
        for (int i = 0; i < pools.Count; i++)
        {
            if (pools[i].tag != poolTag)
            {
                continue;
            }

            for (int j = 0; j < count; j++)
            {
                var obj = Instantiate(pools[i].prefab, pools[i].poolParent, true);
                obj.SetActive(false);
                _poolDictionary[poolTag].Enqueue(obj);
            }

            break;
        }
    }
}

public enum PoolTags
{
    None,
    ColorCube,
    Shooter,
    Bullet,
    ConveyorArrow,
    ReservedSlot,
    ShooterNode,
    ColorCubeNode,
    ShooterPlate,
}

public interface IPoolObject
{
    void Reset();
}
