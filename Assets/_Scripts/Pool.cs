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

        protected static Pool<T> _instance;

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

        private void OnEnable() { Message<ReturnToPool<T>>.Add(ReturnObjectToPool); _instance = this; }
        private void OnDisable(){ Message<ReturnToPool<T>>.Remove(ReturnObjectToPool); _instance = null; }

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
            print("hello world"); 
            pooledObjects.Enqueue(obj); 
        }
    }

    public struct ReturnToPool<T> where T : MonoBehaviour
    {
        public T value;

        public Transform transform => value.transform;

        public static implicit operator ReturnToPool<T>(T obj) { return new ReturnToPool<T>() { value = obj }; }
        public static implicit operator T(ReturnToPool<T> obj) { return obj.value; }
    }
}