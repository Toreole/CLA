﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LATwo
{
    public class Pool<T> : MonoBehaviour where T : Component
    {
        public int amountOfObjects;
        public GameObject prefab;

        Queue<T> pooledObjects;
        //HashSet<T> activeObjects;

        protected static Pool<T> _instance;

        protected virtual void Start()
        {
            pooledObjects = new Queue<T>();
            //activeObjects = new HashSet<T>();
            for (int i = 0; i < amountOfObjects; i++)
            {
                var obj = Instantiate(prefab, transform);
                pooledObjects.Enqueue(obj.GetComponent<T>());
            }
        }

        protected virtual void OnEnable() { Message<ReturnToPool<T>>.Add(ReturnObjectToPool); _instance = this; }
        protected virtual void OnDisable(){ Message<ReturnToPool<T>>.Remove(ReturnObjectToPool); _instance = null; }

        public T M_GetPoolObject()
        {
            if (pooledObjects.Count > 0)
                return pooledObjects.Dequeue();
#if UNITY_EDITOR
            Debug.LogWarning("Trying to get object from empty pool", this);
#endif
            return null;
        }

        public static T GetPoolObject()
        {
            if (_instance)
                return _instance.M_GetPoolObject();
            Debug.LogError("No pool of this type in the scene:" + typeof(T).ToString());
            return null;
        }

        public void ReturnObjectToPool(ReturnToPool<T> obj)
        {
            //print("hello world");
            obj.value.gameObject.SetActive(false);
            pooledObjects.Enqueue(obj); 
        }
    }
}