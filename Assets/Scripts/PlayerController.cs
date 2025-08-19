using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static readonly int Flying = Animator.StringToHash("Flying");
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private float thrustSpeed;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private AudioSource engineAudioSource;
    [SerializeField] private Animator animator;
    
    private bool _throttle;
    private Vector3 _rotationDirection;

    private void Update()
    {
        HandleThrust();
        HandleRotation();

        HandleEffects();
    }

    private void HandleEffects()
    {
        if (_throttle && ! engineAudioSource.isPlaying)
        {
            engineAudioSource.Play();
            animator.SetBool(Flying, true);
        }
        else if (! _throttle && engineAudioSource.isPlaying)
        {
            engineAudioSource.Stop();
            animator.SetBool(Flying, false);
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
            var forceVector = transform.up * thrustSpeed * Time.fixedDeltaTime;
            rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
        }
        
        var rotation = _rotationDirection * rotationSpeed * Time.fixedDeltaTime;
        rigidbody.MoveRotation(rigidbody.rotation + rotation.z);
    }
}
