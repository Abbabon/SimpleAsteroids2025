using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private float _startingSpeed;
    private int _level;
    
    public void Setup(Sprite sprite, float startingSpeed, int level)
    {
        spriteRenderer.sprite = sprite;
        _startingSpeed = startingSpeed;
        _level = level;
    }
    
    private void Start()
    {
        WarpManager.Instance.RegisterTransform(transform);
        
        var force = Random.insideUnitCircle.normalized * _startingSpeed;
        rigidbody2D.AddForce(force);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        var isShip = other.CompareTag(Constants.ShipTag);
        var isBullet = other.CompareTag(Constants.BulletTag);
        
        if (isShip)
        {
            GameManager.Instance.OnShipHit();
        }
        else if (isBullet)
        {
            var bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                PlayerController.Instance.ReturnBullet(bullet);
            }
            
            GameManager.Instance.OnAsteroidDestroyed();
            DestroyAsteroid();
        }
    }
    
    private void DestroyAsteroid()
    {
        WarpManager.Instance.UnregisterTransform(transform);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        WarpManager.Instance.UnregisterTransform(transform);
    }
}