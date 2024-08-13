using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AsteroidSpawn : NetworkBehaviour
{
    public GameObject _asteroidPrefab;
    public float spawnTime = 2f;
    public float spawnRadius = 1f;

    public AsteroidMovement a_movement;

    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        InvokeRepeating("SpawnObject", 0f, spawnTime);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        a_movement = GetComponent<AsteroidMovement>();
    }

    [ServerRpc]
    public void SpawnAsteroidServerRpc()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(_asteroidPrefab, spawnPosition, Quaternion.identity);
        a_movement = GetComponent<AsteroidMovement>();
        gameObject.GetComponent<NetworkObject>().Spawn();
    }

    Vector3 GetRandomSpawnPosition()
    {
        //get how the screens cordinates
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Convert screen bounds to the world space
        Vector3 worldBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0,0, mainCamera.transform.position.z));
        Vector3 worldTopRight = mainCamera.ScreenToWorldPoint(new Vector3(screenWidth, screenHeight, mainCamera.transform.position.z));

        float xPos = Random.Range(worldBottomLeft.x - spawnRadius, worldBottomLeft.x + spawnRadius);
        float yPos = Random.Range(worldTopRight.x - spawnRadius, worldTopRight.x + spawnRadius);
        //The randomizer from the sides
        if(Random.value < 0.5f)
        {
            xPos = Random.value < 0.5f ? worldBottomLeft.x - spawnRadius : worldBottomLeft.x + spawnRadius;
            yPos = Random.Range(worldBottomLeft.y, worldTopRight.y);
        }
        // Spawn asteroids from top/Bottom
        else 
            yPos = Random.value < 0.5f ? worldTopRight.x - spawnRadius : worldTopRight.x + spawnRadius;
            xPos = Random.Range(worldBottomLeft.x, worldTopRight.x);


        return new Vector3(xPos, yPos,0);
    }

    private void Update()
    {
        if (!IsOwner || !Application.isFocused) return;
        a_movement.AsteroidMovementServerRpc();
        SpawnAsteroidServerRpc();
    }
}
