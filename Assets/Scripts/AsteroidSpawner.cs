using System;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private float drawRadius = 1f;
    [SerializeField] private int startingLevel;
    
    public int StartingLevel => startingLevel;

    private void Start()
    {
        GameManager.Instance.RegisterSpawner(this);
    }
    
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterSpawner(this);
        }
    }

    public void SpawnAsteroid()
    {
        AsteroidsFactory.Instance.SpawnAsteroid(startingLevel, transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, drawRadius);
    }
}   
