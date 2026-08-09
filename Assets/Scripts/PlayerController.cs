using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    private static readonly int Flying = Animator.StringToHash("Flying");

    [Header("Ship")]
    [SerializeField] private float thrustSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Ship ship;

    [Header("MobileControls")]
    [SerializeField] private RectTransform mobileControlsContainer;

    private BestObjectPool<Bullet> _bulletPool;
    private Vector3 _originalShipScale;

    private InputAction _rotate;
    private InputAction _thrust;
    private InputAction _shoot;
    private InputAction _restart;

    private void Awake()
    {
        // Project-wide actions, which the Input System enables on entering play mode.
        var player = InputSystem.actions.FindActionMap(Constants.PlayerActionMap, throwIfNotFound: true);

        _rotate = player.FindAction(Constants.RotateAction, throwIfNotFound: true);
        _thrust = player.FindAction(Constants.ThrustAction, throwIfNotFound: true);
        _shoot = player.FindAction(Constants.ShootAction, throwIfNotFound: true);
        _restart = player.FindAction(Constants.RestartAction, throwIfNotFound: true);

        _bulletPool = new BestObjectPool<Bullet>(bulletPrefab);

        // Cache original ship scale
        if (ship != null)
        {
            _originalShipScale = ship.transform.localScale;
        }

        // On-screen controls feed the same actions as the keyboard, so this only decides visibility.
        if (mobileControlsContainer != null)
        {
            mobileControlsContainer.gameObject.SetActive(Application.isMobilePlatform);
        }
    }

    private void Update()
    {
        // Handle restart input when game is over or won
        if (GameManager.Instance.GameOver || GameManager.Instance.GameWon)
        {
            if (_restart.WasPressedThisFrame())
            {
                GameManager.Instance.RestartGame();
            }

            return;
        }

        if (!GameManager.Instance.PlayerAlive || !GameManager.Instance.GameStarted)
            return;

        HandleEffects();
        HandleBullets();
    }

    private void HandleEffects()
    {
        var throttle = _thrust.IsPressed();

        if (throttle && ! ship.EngineAudioSource.isPlaying)
        {
            ship.EngineAudioSource.Play();
            ship.Animator.SetBool(Flying, true);
        }
        else if (! throttle && ship.EngineAudioSource.isPlaying)
        {
            ship.EngineAudioSource.Stop();
            ship.Animator.SetBool(Flying, false);
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.PlayerAlive || GameManager.Instance.GameOver || GameManager.Instance.GameWon || !GameManager.Instance.GameStarted)
            return;

        if (_thrust.IsPressed())
        {
            var forceVector = ship.transform.up * (thrustSpeed * Time.fixedDeltaTime);
            ship.Rigidbody2D.AddForce(forceVector, ForceMode2D.Impulse);
        }

        // Negated so left input turns counter-clockwise
        var turn = -_rotate.ReadValue<float>();

        if (turn != 0f)
        {
            ship.Rigidbody2D.MoveRotation(ship.Rigidbody2D.rotation + turn * rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleBullets()
    {
        if (_shoot.WasPressedThisFrame())
        {
            var newBullet = _bulletPool.GetObject();
            newBullet.transform.position = ship.transform.position;
            var bulletDirection = ship.transform.up;
            newBullet.Fire(bulletDirection);
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

        // Reactivate ship
        ship.gameObject.SetActive(true);

        // Re-register with warp manager
        ship.RegisterWithWarpManager();
    }
}
