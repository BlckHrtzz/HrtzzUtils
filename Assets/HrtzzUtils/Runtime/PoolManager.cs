using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hrtzz
{
    public class PoolManager : AutoSingleton<PoolManager>
    {
        Dictionary<int, Stack<GameObject>> poolDictionary = new Dictionary<int, Stack<GameObject>> ();

        public GameObject GetObjectFromPool (GameObject prefab, int initialPoolAmount = 5)
        {
            GameObject item;
            int poolKey = prefab.GetInstanceID ();

            if (!poolDictionary.ContainsKey (poolKey))
            {
                poolDictionary.Add (poolKey, new Stack<GameObject> ());
                GameObject poolParent = new GameObject (prefab.name + "Pool");
                poolParent.transform.SetParent (transform);

                for (int i = 0; i < initialPoolAmount; i++)
                {
                    InstantiatePoolObject (prefab, poolKey, poolParent);
                }
            }

            if (poolDictionary [poolKey].Count > 0)
            {
                item = poolDictionary [poolKey].Pop ();
            }
            else
            {
                InstantiatePoolObject (prefab, poolKey);
                item = poolDictionary [poolKey].Pop ();
            }

            item.transform.SetParent (null);
            item.SetActive (true);

            return item;
        }

        public void ReturnObjectToPool (GameObject toReturn)
        {
            PooledObject poolObject = toReturn.GetComponent<PooledObject> ();

            if (poolObject && poolDictionary.ContainsKey (poolObject.poolKey))
            {
                toReturn.transform.SetParent (poolObject.poolParent.transform, false);
                toReturn.SetActive (false);
                poolDictionary [poolObject.poolKey].Push (toReturn);
            }
            else
            {
                Destroy (toReturn);
            }

        }

        protected void InstantiatePoolObject (GameObject prefab, int poolKey, GameObject parent = null)
        {
            if (!parent)
            {
                parent = transform.Find (prefab.name + "Pool").gameObject;
            }

            GameObject spawnedItem = (GameObject) Instantiate (prefab, parent.transform);
            PooledObject poolObject = spawnedItem.AddComponent<PooledObject> ();

            poolObject.poolParent = parent;
            poolObject.poolKey = poolKey;
            spawnedItem.SetActive (false);
            poolDictionary [poolKey].Push (spawnedItem);
        }
    }

    [System.Serializable]
    public class PooledObject : MonoBehaviour
    {
        public GameObject poolParent;
        public int poolKey;
    }
}