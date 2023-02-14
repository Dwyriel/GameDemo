using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject mapCenter;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField, Min(0)] private float spawnZoneVerticalVariation = 5f;
    [SerializeField, Min(0)] private float spawnZoneHorizontalVariation = 50f;
    [SerializeField, Min(0)] private float spawnZoneVerticalOffset = 50f;
    [SerializeField, Min(0)] private float spawnZoneHorizontalOffset = 350f;

    private void Start()
    {
        SpawnEnemies();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void SpawnEnemies()
    {
        var mapCenterPosition = mapCenter.transform.position;
        for (var i = 0; i < ConstValuesAndUtility.NumberOfEnemiesToSpawn; i++)
        {
            var x = Random.Range(-spawnZoneHorizontalVariation, spawnZoneHorizontalVariation) + (Random.value > .5 ? spawnZoneHorizontalOffset : -spawnZoneHorizontalOffset);
            var y = Random.Range(-spawnZoneVerticalVariation, spawnZoneVerticalVariation) + spawnZoneVerticalOffset;
            var z = Random.Range(-spawnZoneHorizontalVariation, spawnZoneHorizontalVariation) + (Random.value > .5 ? spawnZoneHorizontalOffset : -spawnZoneHorizontalOffset);
            var spawnPoint = new Vector3(x, y, z);
            Instantiate(enemyPrefab, spawnPoint, Quaternion.LookRotation(mapCenterPosition - spawnPoint));
        }
    }
}