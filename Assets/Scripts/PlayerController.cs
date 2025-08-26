using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private static readonly int Flying = Animator.StringToHash("Flying");
    [SerializeField] private float thrustSpeed;
    [SerializeField] private float rotationSpeed;
    
    [SerializeField] private Bullet bulletPrefab;
    
    // TODO: make prefab
    [SerializeField] private Ship ship;

    private BestObjectPool<Bullet> bulletPool;
    
    private bool _throttle;
    private Vector3 _rotationDirection;

    private void Awake()
    {
        bulletPool = new BestObjectPool<Bullet>(bulletPrefab);
    }

    private void Update()
    {
        HandleThrust();
        HandleRotation();
        
        HandleEffects();

        HandleBullets();
    }

    private void HandleEffects()
    {
        if (_throttle && ! ship.EngineAudioSource.isPlaying)
        {
            ship.EngineAudioSource.Play();
            ship.Animator.SetBool(Flying, true);
        }
        else if (! _throttle && ship.EngineAudioSource.isPlaying)
        {
            ship.EngineAudioSource.Stop();
            ship.Animator.SetBool(Flying, false);
        }
    }

    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _rotationDirection = new Vector3(0, 0, -1);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _rotationDirection = new Vector3(0, 0, 1);
        }
        else
        {
            _rotationDirection = Vector3.zero;
        }
    }

    private void HandleThrust()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _throttle = true;
        }
        else
        {
            _throttle = false;
        }
    }

    private void FixedUpdate()
    {
        if (_throttle)
        {
            var forceVector = ship.transform.up * thrustSpeed * Time.fixedDeltaTime;
            ship.Rigidbody2D.AddForce(forceVector, ForceMode2D.Impulse);
        }
        
        var rotation = _rotationDirection * rotationSpeed * Time.fixedDeltaTime;
        ship.Rigidbody2D.MoveRotation(ship.Rigidbody2D.rotation + rotation.z);
    }
    
    private void HandleBullets()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var newBullet = bulletPool.GetObject();
            newBullet.transform.position = ship.transform.position;
            var bulletDirection = ship.transform.up;
            newBullet.Fire(bulletDirection);   
        }
    }
    
    public void ReturnBullet(Bullet bullet)
    {
        // TODO extension methods
        bullet.Rigidbody.linearVelocity = Vector2.zero;
        bullet.Rigidbody.angularVelocity = 0f;
        
        bulletPool.ReleaseObject(bullet);
    }
}
