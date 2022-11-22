using UnityEngine;

namespace ShmupKit.Controls
{
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        public float speed = 5f;
        
        private Animator _animator;
        private Vector2 _movement;
        
        private static readonly int VerticalVelocity = Animator.StringToHash("VerticalVelocity");
        private static readonly int HorizontalVelocity = Animator.StringToHash("HorizontalVelocity");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        
        private void Update()
        {
            _animator.SetFloat(VerticalVelocity, _movement.y);
            _animator.SetFloat(HorizontalVelocity, _movement.x);
            
            transform.Translate(_movement * (speed * Time.deltaTime));
        }
        
        public void HandleMovement(Vector3 movement)
        {
            _movement = Vector3.right * movement.x + Vector3.up * movement.y;
        }
    }
}