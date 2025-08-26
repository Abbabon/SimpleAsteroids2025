using System.Collections;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private float speed;
    [SerializeField] private float timeout = 4f;
    
    public Rigidbody2D Rigidbody => rigidbody;
    
    private void Start()
    {
        WarpManager.Instance.RegisterTransform(transform);
    }
    
    public void Fire(Vector2 direction)
    {
        var force = direction.normalized * speed;
        rigidbody.AddForce(force);

        StartCoroutine(Timeout());
    }

    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(timeout);
        PlayerController.Instance.ReturnBullet(this);
    }
    
    private void OnDestroy()
    {
        WarpManager.Instance.UnregisterTransform(transform);
    }
}