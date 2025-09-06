using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private static readonly int Flying = Animator.StringToHash("Flying");
    [SerializeField] private float thrustSpeed;
    [SerializeField] private float rotationSpeed;
    
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Ship ship;

    [Header("MobileControls")]
    [SerializeField] private RectTransform mobileControlsContainer;
    [SerializeField] private MobileButton leftButton;
    [SerializeField] private MobileButton rightButton;
    [SerializeField] private MobileButton aButton;
    [SerializeField] private MobileButton bButton;
    
    private BestObjectPool<Bullet> _bulletPool;
    
    private bool _throttle;
    private Vector3 _rotationDirection;
    private Vector3 _originalShipScale;
    
    // Mobile input state
    private bool _mobileThrust;
    private bool _mobileRotateLeft;
    private bool _mobileRotateRight;
    private bool _mobileFire;

    private void Awake()
    {
        _bulletPool = new BestObjectPool<Bullet>(bulletPrefab);
        
        // Cache original ship scale
        if (ship != null)
        {
            _originalShipScale = ship.transform.localScale;
        }
        
        SetupMobileControls();
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
        bool rotateRight = Input.GetKey(KeyCode.D) || _mobileRotateRight;
        bool rotateLeft = Input.GetKey(KeyCode.A) || _mobileRotateLeft;
        
        if (rotateRight)
        {
            _rotationDirection = new Vector3(0, 0, -1);
        }
        else if (rotateLeft)
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
        _throttle = Input.GetKey(KeyCode.W) || _mobileThrust;
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
        if (Input.GetKeyDown(KeyCode.Space) || _mobileFire)
        {
            var newBullet = _bulletPool.GetObject();
            newBullet.transform.position = ship.transform.position;
            var bulletDirection = ship.transform.up;
            newBullet.Fire(bulletDirection);
            
            // Reset mobile fire state (single shot)
            _mobileFire = false;
        }
    }
    
    public void ReturnBullet(Bullet bullet)
    {
        bullet.MarkAsReturned();
        bullet.Rigidbody.linearVelocity = Vector2.zero;
        bullet.Rigidbody.angularVelocity = 0f;
        
        _bulletPool.ReleaseObject(bullet);
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
        ResetMobileInputState();
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
        ResetMobileInputState();
        
        // Reactivate ship
        ship.gameObject.SetActive(true);
        
        // Re-register with warp manager
        ship.RegisterWithWarpManager();
    }
    
    private void SetupMobileControls()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (mobileControlsContainer != null)
        {
            mobileControlsContainer.gameObject.SetActive(true);
            SetupButtonEvents();
        }
#else
        if (mobileControlsContainer != null)
        {
            mobileControlsContainer.gameObject.SetActive(false);
        }
#endif
    }
    
    private void SetupButtonEvents()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (leftButton != null)
        {
            leftButton.OnPressed += () => _mobileRotateLeft = true;
            leftButton.OnReleased += () => _mobileRotateLeft = false;
        }
        
        if (rightButton != null)
        {
            rightButton.OnPressed += () => _mobileRotateRight = true;
            rightButton.OnReleased += () => _mobileRotateRight = false;
        }
        
        if (aButton != null)
        {
            aButton.OnPressed += () => _mobileThrust = true;
            aButton.OnReleased += () => _mobileThrust = false;
        }
        
        if (bButton != null)
        {
            bButton.OnPressed += () => _mobileFire = true;
        }
#endif
    }
    
    private void ResetMobileInputState()
    {
        _mobileThrust = false;
        _mobileRotateLeft = false;
        _mobileRotateRight = false;
        _mobileFire = false;
    }
}
