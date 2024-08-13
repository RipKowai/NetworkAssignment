using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AsteroidMovement : MonoBehaviour
{
    public Transform AsteroidDirection;
    public float _asteroidSpeed = 10f;

    [ServerRpc]
    public void AsteroidMovementServerRpc()
    {
        GameObject asteroid = Instantiate(gameObject, AsteroidDirection.position, AsteroidDirection.rotation);

        asteroid.GetComponent<NetworkObject>().Spawn();

        Quaternion playerRotation = transform.rotation;

        asteroid.transform.rotation = playerRotation;
        
        Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();
        rb.AddForce(AsteroidDirection.up * _asteroidSpeed, ForceMode2D.Impulse);
    }
}