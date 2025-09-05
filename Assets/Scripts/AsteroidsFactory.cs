using UnityEngine;

public class AsteroidsFactory : Singleton<AsteroidsFactory>
{
    [SerializeField] private Asteroid asteroidPrefab;
    [SerializeField] private AsteroidsConfig asteroidsConfig;
        
    public void SpawnAsteroid(int level, Vector3 position)
    {
        var asteroid = Instantiate(asteroidPrefab, position, Quaternion.identity);
        var config = asteroidsConfig.GetConfigForLevel(level);
        
        if (config != null)
        {
            var sprite = config.Sprites[Random.Range(0, config.Sprites.Count)];
            asteroid.Setup(sprite, config.SpawnSpeed, level);
        }
        else
        {
            Debug.LogError($"No config found for level {level}");
            Destroy(asteroid.gameObject);
        }
    }
    
    public AsteroidData GetConfigForLevel(int level)
    {
        return asteroidsConfig.GetConfigForLevel(level);
    }
    
    public void SpawnChildAsteroids(int parentLevel, Vector3 parentPosition, Vector3 impactPosition)
    {
        int childLevel = parentLevel + 1;
        
        // Check if a higher level exists in the configuration
        var childConfig = GetConfigForLevel(childLevel);
        if (childConfig == null) return;
        
        // Spawn 2 child asteroids
        for (int i = 0; i < 2; i++)
        {
            // Calculate direction away from impact point
            Vector2 directionFromImpact = (parentPosition - impactPosition).normalized;
            
            // Add some random spread to avoid asteroids moving in exact same direction
            float randomAngle = Random.Range(-45f, 45f);
            Vector2 rotatedDirection = Quaternion.Euler(0, 0, randomAngle) * directionFromImpact;
            
            // Slightly offset spawn position to avoid overlap
            Vector3 spawnOffset = rotatedDirection * 0.5f;
            Vector3 spawnPosition = parentPosition + spawnOffset;
            
            SpawnAsteroid(childLevel, spawnPosition);
        }
    }
}