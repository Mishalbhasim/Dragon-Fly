using UnityEngine;

namespace RunnerGame.Player
{
   
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimatorController : MonoBehaviour
    {
        private static readonly int FlyBlendHash = Animator.StringToHash("FlyBlend");

        [SerializeField] private float blendSpeed = 6f;

        private Animator _animator;
        private float _currentBlend;
        private float _targetBlend;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetThrusting(bool thrusting)
        {
            _targetBlend = thrusting ? 1f : 0f;
        }

        private void Update()
        {
            _currentBlend = Mathf.MoveTowards(_currentBlend, _targetBlend, blendSpeed * Time.deltaTime);
            _animator.SetFloat(FlyBlendHash, _currentBlend);
        }
    }
}
