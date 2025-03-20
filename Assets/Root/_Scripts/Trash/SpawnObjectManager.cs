using Root;
using UnityEngine;

public class SpawnObjectManager : MonoBehaviour
{
    //[SerializeField] private static float _spawnCooldownDuration = 20;
    [SerializeField] private GameObject[] _prefabs;

    [SerializeField] private int[] _prefabSizes;

    [SerializeField] private Vector3 _maxDistanceFromPlatform;


    [SerializeField] private GameObject _trashSpot;

    private static int _activeObjectsCounter;
    //private static bool _cooldownStarted = false;
    

    private void Start()
    {
        SpawnResourceWave();
    }

    private void Update()
    {
        if (_activeObjectsCounter==0)
        {
            SpawnResourceWave();
        }
    }


    private void SpawnResourceWave()
    {
        for (int prefabIndex = 0; prefabIndex < _prefabs.Length; prefabIndex++)
        {
            Debug.Log("Spawn Resource Wave " + _prefabs[prefabIndex]);
            SpawnResourceType(_prefabs[prefabIndex]);
        }
    }

    private void SpawnResourceType(GameObject prefab)
    {
        for (int i = 0; i<_prefabSizes.Length; i++)
        {
            Debug.Log("PrefabSize [i], i = " + i);
            for (int sizeNumber = 0; sizeNumber < _prefabSizes[i]; sizeNumber++)
            {
                _activeObjectsCounter++;

                GameObject obj = PoolManager.SpawnObject(prefab, _maxDistanceFromPlatform, gameObject);
                SetObjectSettings(obj, i);
                
                obj.transform.SetParent(_trashSpot.transform);
            }
        }
    }


    private void SetObjectSettings(GameObject obj, int sizeIndex)
    {
        float scaleMultiplier = 0;
        Trash trash = obj.GetComponent<Trash>();
        trash.Value = sizeIndex+1;

        switch (sizeIndex)
        {
            case 0:
                scaleMultiplier = 2;
                break;
            case 1:
                scaleMultiplier = 3;
                break;
            case 2:
                scaleMultiplier = 4;
                break;
            default:
                scaleMultiplier = 0;
                break;
        }
        obj.transform.localScale *= scaleMultiplier;
    }

    public static void SetFalseObject(GameObject obj)
    {
        if (obj.CompareTag("OrbitTrash"))
        {
            _activeObjectsCounter--;
            PoolManager.ReturnObjectToPool(obj);
        }
    }
}
