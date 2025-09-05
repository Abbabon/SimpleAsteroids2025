using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private static readonly int Flying = Animator.StringToHash("Flying");
    [SerializeField] private float thrustSpeed;
    [SerializeField] private float rotationSpeed;
    
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Ship ship;

    private BestObjectPool<Bullet> bulletPool;
    
    private bool _throttle;
    private Vector3 _rotationDirection;
    private Vector3 _originalShipScale;

    private void Awake()
    {
        bulletPool = new BestObjectPool<Bullet>(bulletPrefab);
        
        // Cache original ship scale
        if (ship != null)
        {
            _originalShipScale = ship.transform.localScale;
        }
    }

    private void Update()
    {
        // Handle restart input when game is over or won
        if ((GameManager.Instance.GameOver || GameManager.Instance.GameWon) && Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.RestartGame();
            return;
        }
        
        if (!GameManager.Instance.PlayerAlive || GameManager.Instance.GameOver || GameManager.Instance.GameWon || !GameManager.Instance.GameStarted)
            return;
            
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
        if (!GameManager.Instance.PlayerAlive || GameManager.Instance.GameOver || GameManager.Instance.GameWon || !GameManager.Instance.GameStarted)
            return;
            
        if (_throttle)
        {
            var forceVector = ship.transform.up * (thrustSpeed * Time.fixedDeltaTime);
            ship.Rigidbody2D.AddForce(forceVector, ForceMode2D.Impulse);
        }
        
        var rotation = _rotationDirection * (rotationSpeed * Time.fixedDeltaTime);
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
    
    public void DestroyShip()
    {
        ship.gameObject.SetActive(false);
        
        // Stop all movement and audio
        ship.Rigidbody2D.linearVelocity = Vector2.zero;
        ship.Rigidbody2D.angularVelocity = 0f;
        ship.EngineAudioSource.Stop();
        ship.Animator.SetBool(Flying, false);
        
        // Reset input state
        _throttle = false;
        _rotationDirection = Vector3.zero;
    }
    
    public void RespawnShip()
    {
        // Reset ship position and rotation
        ship.transform.position = Vector3.zero;
        ship.transform.rotation = Quaternion.identity;
        ship.transform.localScale = _originalShipScale;
        
        // Reset physics
        ship.Rigidbody2D.linearVelocity = Vector2.zero;
        ship.Rigidbody2D.angularVelocity = 0f;
        
        // Reset input state
        _throttle = false;
        _rotationDirection = Vector3.zero;
        
        // Reactivate ship
        ship.gameObject.SetActive(true);
        
        // Re-register with warp manager
        ship.RegisterWithWarpManager();
    }
}
