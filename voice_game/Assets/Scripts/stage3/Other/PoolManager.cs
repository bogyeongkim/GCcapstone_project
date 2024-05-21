using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    public List<Pool> pools;

    protected override void Awake()
    {
        base.Awake();

        foreach (Pool pool in pools)
        {
            pool.objs = new List<GameObject>();
        }
    }

    public GameObject GetPool(int id)
    {
        return GetPool(id, transform);
    }
    public GameObject GetPool(int id, Transform parent)
    {
        GameObject select = null;

        foreach (GameObject obj in pools[id].objs)
        {
            if (obj.activeSelf) continue;

            select = obj;
        }

        if (!select)
        {
            select = Instantiate(pools[id].objPrefab, parent);
            pools[id].objs.Add(select);
        }

        select.SetActive(true);

        return select;
    }
    
    [Serializable]
    public class Pool
    {
        public GameObject objPrefab;
        public List<GameObject> objs;
    }
}

