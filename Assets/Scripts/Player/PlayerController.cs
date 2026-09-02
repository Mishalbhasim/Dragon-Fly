using UnityEngine;
using RunnerGame.Core;

namespace RunnerGame.Player
{

    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float thrustForce = 18f;
        [SerializeField] private float extraGravity = 25f;
        [SerializeField] private float maxUpwardVelocity = 10f;
        [SerializeField] private float maxDownwardVelocity = -14f;
        [SerializeField] private float ceilingY = 6f;
        [SerializeField] private float floorY = 0f;

        public bool IsThrusting { get; private set; }

        private Rigidbody _rb;
        private PlayerAnimatorController _animController;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false; // we control gravity in script
            _animController = GetComponent<PlayerAnimatorController>();
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            {
                IsThrusting = false;
                return;
            }

            // tapping and holding is upward thrust
            IsThrusting = Input.GetMouseButton(0) || Input.touchCount > 0;

            _animController?.SetThrusting(IsThrusting);
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            Vector3 velocity = _rb.velocity;

            if (IsThrusting)
                velocity.y += thrustForce * Time.fixedDeltaTime;
            else
                velocity.y -= extraGravity * Time.fixedDeltaTime;

            velocity.y = Mathf.Clamp(velocity.y, maxDownwardVelocity, maxUpwardVelocity);
            _rb.velocity = new Vector3(0f, velocity.y, 0f); 

            //player Y-position clamped within the movable area
            Vector3 pos = transform.position;
            if (pos.y > ceilingY) { pos.y = ceilingY; if (velocity.y > 0) velocity.y = 0; }
            if (pos.y < floorY) { pos.y = floorY; if (velocity.y < 0) velocity.y = 0; }
            transform.position = pos;
            _rb.velocity = velocity;
        }
    }
}
