using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Root
{

    public class PoolManager : MonoBehaviour
    {
        [SerializeField] private static float _maxSpeedOffset = 20;


        public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 _maxDistanceFromPlatform, GameObject platform)
        {
            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

            if (pool == null)
            {
                pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
                ObjectPools.Add(pool);
                Debug.Log("Object pools: " + ObjectPools.Count);
            }

            GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

            if (spawnableObj == null)
            {

                Vector3 posOffset = GetSpawnPosition(_maxDistanceFromPlatform);

                spawnableObj = Instantiate(objectToSpawn, platform.transform.position + posOffset, objectToSpawn.transform.rotation);
    

            }
            else
            {
                Vector3 posOffset = GetSpawnPosition(_maxDistanceFromPlatform);

                spawnableObj.transform.position = platform.transform.position+posOffset;
                spawnableObj.transform.rotation = objectToSpawn.transform.rotation;

                pool.ActiveObjects.Add(spawnableObj);
                pool.InactiveObjects.Remove(spawnableObj);

                spawnableObj.SetActive(true);
            }

            spawnableObj.transform.SetParent(platform.transform);

            TrashMovement TM = spawnableObj.GetComponent<TrashMovement>();
            if (TM != null)
            {
                TM._pivotObject = platform;
                TM._rotationSpeed = Random.Range(-_maxSpeedOffset, _maxSpeedOffset);

            }
            return spawnableObj;
        }


        private static Vector3 GetSpawnPosition(Vector3 _maxDistanceFromPlatform)
        {
            float x = _maxDistanceFromPlatform.x,
                y = _maxDistanceFromPlatform.y,
                z = _maxDistanceFromPlatform.z;


            return new Vector3(
                Random.Range(-x, x),
                Random.Range(-y, y),
                Random.Range(-z, z));
        }

        public static void ReturnObjectToPool(GameObject obj)
        {
            string goName = obj.name.Substring(0, obj.name.Length - 7);

            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == goName);

            if (pool == null)
            {
                Debug.LogWarning("Trying to release an object that is not pooled " + obj.name);
            }
            else
            {
                obj.SetActive(false);
                pool.ActiveObjects.Remove(obj);
                pool.InactiveObjects.Add(obj);
            }

        }

        
    }




    public class PooledObjectInfo
    {
        public string LookupString;
        public List<GameObject> InactiveObjects = new List<GameObject>();
        public List<GameObject> ActiveObjects = new List<GameObject>();
        public bool noActiveObjects = true;
    }

}

