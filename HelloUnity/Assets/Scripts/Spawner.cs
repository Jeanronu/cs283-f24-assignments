using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject coinPrefab;  // Prefab for the coin collectable
    public float range = 5f;       // Radius within which coins will spawn
    public int maxSpawnCount = 10; // Max number of coins that can be spawned at once

    private List<CollectionGame> spawnedCoins = new List<CollectionGame>();

    void Start()
    {
        // Initial spawning of coins up to the maximum count
        for (int i = 0; i < maxSpawnCount; i++)
        {
            SpawnCoin();
        }
    }

    // Method to spawn a new coin
    private void SpawnCoin()
    {
        Vector3 randomPosition = GetRandomPosition();
        GameObject newCoinObject = Instantiate(coinPrefab, randomPosition, Quaternion.identity);
        CollectionGame newCoin = newCoinObject.GetComponent<CollectionGame>();

        // Listen to the OnCollected event to respawn the coin
        newCoin.OnCollected += RespawnCoin;
        spawnedCoins.Add(newCoin);
    }

    // Method to respawn a coin that has been picked up
    private void RespawnCoin(CollectionGame coin)
    {
        Vector3 randomPosition = GetRandomPosition();
        coin.transform.position = randomPosition;
        coin.ResetCoin(); // Reset coin state and make it visible again
    }

    // Generates a random position within the specified range of the spawner
    private Vector3 GetRandomPosition()
    {
        Vector3 randomOffset = Random.insideUnitSphere * range;
        randomOffset.y = 0; // Keep coins on the same level as the spawner
        Vector3 randomPosition = transform.position + randomOffset;

        Collider collider = coinPrefab.GetComponent<Collider>();
        if (collider != null)
        {
            float terrainHeight = Terrain.activeTerrain.SampleHeight(randomPosition);
            float randomYOffset = Random.Range(0.7f, 2.5f);  // Random offset between 0.7 and 2 units
            randomPosition.y = terrainHeight + collider.bounds.extents.y + randomYOffset;
        }
        return randomPosition;
    }
}
