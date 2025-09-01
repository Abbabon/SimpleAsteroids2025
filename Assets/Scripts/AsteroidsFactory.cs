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
}