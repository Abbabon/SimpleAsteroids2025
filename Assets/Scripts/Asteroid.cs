using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private float _startingSpeed;
    private int _level;
    private bool _isDestroyed;
    public bool IsDestroyed => _isDestroyed;

    public void Setup(Sprite sprite, float startingSpeed, int level)
    {
        spriteRenderer.sprite = sprite;
        _startingSpeed = startingSpeed;
        _level = level;
    }
    
    private void Start()
    {
        WarpManager.Instance.RegisterTransform(transform);
        GameManager.Instance.RegisterAsteroid(this);
        
        var force = Random.insideUnitCircle.normalized * _startingSpeed;
        rigidbody2D.AddForce(force);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        var isShip = other.CompareTag(Constants.ShipTag);
        var isBullet = other.CompareTag(Constants.BulletTag);
        
        if (isShip)
        {
            _isDestroyed = true;
            GameManager.Instance.OnShipHit();
            GameManager.Instance.OnAsteroidDestroyed(false); // No points for ship collision
            DestroyAsteroid();
        }
        else if (isBullet)
        {
            _isDestroyed = true;
            var bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                PlayerController.Instance.ReturnBullet(bullet);
            }
            
            GameManager.Instance.OnAsteroidDestroyed(true); // Award points for bullet hit
            DestroyAsteroid();
        }
    }
    
    private void DestroyAsteroid()
    {
        WarpManager.Instance.UnregisterTransform(transform);
        GameManager.Instance.UnregisterAsteroid(this);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        WarpManager.Instance.UnregisterTransform(transform);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterAsteroid(this);
        }
    }
}