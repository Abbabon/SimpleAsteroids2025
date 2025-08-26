using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private float forceMagnitude = 3f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Sprite> sprites;

    private void Awake()
    {
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)];
    }

    private void Start()
    {
        WarpManager.Instance.RegisterTransform(transform);
        
        var force = Random.insideUnitCircle.normalized * forceMagnitude;
        rigidbody2D.AddForce(force);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        var isPlayer = other.CompareTag(Constants.PlayerTag);
        var isBullet = other.CompareTag(Constants.BulletTag);
        
        if (isPlayer)
        {
            
        }
        else if (isBullet)
        {
            
        }
    }

    private void OnDestroy()
    {
        WarpManager.Instance.UnregisterTransform(transform);
    }
}