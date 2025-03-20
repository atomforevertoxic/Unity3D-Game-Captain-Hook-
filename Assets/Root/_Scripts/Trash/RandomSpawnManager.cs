using Root;
using System.Collections;
using UnityEngine;

public class RandomSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;

    [SerializeField] private GameObject[] spawnPositions;

    [SerializeField] private float spawnCooldown = 10;

    [SerializeField] private GameObject _trashSpot;

    private bool _spawnAvailable = true;

    // Update is called once per frame
    private void Update()
    {
        if (!_spawnAvailable) return;
        StartCoroutine(Cooldown(spawnCooldown));


        int randPrefabIndex = Random.Range(0, prefabs.Length);

        int randSpawnPositionIndex = Random.Range(0, spawnPositions.Length);

        GameObject obj = PoolManager.SpawnObject(prefabs[randPrefabIndex], new Vector3(0, 0, 0), spawnPositions[randSpawnPositionIndex]);
        obj.transform.SetParent(_trashSpot.transform);
        
    }

    private IEnumerator Cooldown(float duration)
    {
        Debug.Log("Cooldown");
        _spawnAvailable = false;
        yield return new WaitForSeconds(spawnCooldown);
        _spawnAvailable = true;
    }
}
