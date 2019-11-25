using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LATwo
{
    public class Pool<T> : MonoBehaviour where T : MonoBehaviour
    {
        public int amountOfObjects;
        public GameObject prefab;

        Queue<T> pooledObjects;
        //HashSet<T> activeObjects;

        protected virtual void Start()
        {
            pooledObjects = new Queue<T>();
            //activeObjects = new HashSet<T>();
            for (int i = 0; i < amountOfObjects; i++)
            {
                var obj = Instantiate(prefab);
                obj.SetActive(false);
                pooledObjects.Enqueue(obj.GetComponent<T>());
            }
        }

        private void OnEnable() { Message<T>.Add(ReturnObjectToPool); }
        private void OnDisable(){ Message<T>.Remove(ReturnObjectToPool); }

        public T GetPoolObject()
        {
            if (pooledObjects.Count > 0)
                return pooledObjects.Dequeue();
#if UNITY_EDITOR
            Debug.LogWarning("Trying to get object from empty pool", this);
#endif
            return null;
        }

        public void ReturnObjectToPool(T obj)
        {
            pooledObjects.Enqueue(obj);
        }
    }
}