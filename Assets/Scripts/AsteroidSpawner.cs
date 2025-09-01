using System;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private float drawRadius = 1f;
    [SerializeField] private int startingLevel;

    private void Start()
    {
        AsteroidsFactory.Instance.SpawnAsteroid(startingLevel, transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, drawRadius);
    }
}   
