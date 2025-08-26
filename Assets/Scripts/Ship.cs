using System;
using UnityEngine;

public class Ship : MonoBehaviour
{
        [SerializeField] private Rigidbody2D rigidbody2D;
        [SerializeField] private AudioSource engineAudioSource;
        [SerializeField] private Animator animator;
        
        public Rigidbody2D Rigidbody2D => rigidbody2D;
        public AudioSource EngineAudioSource => engineAudioSource;
        public Animator Animator => animator;

        private void Start()
        {
                WarpManager.Instance.RegisterTransform(transform);
        }

        private void OnDestroy()
        {
                WarpManager.Instance.UnregisterTransform(transform);
        }
}