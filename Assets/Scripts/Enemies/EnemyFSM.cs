using UnityEngine;
using RunnerGame.Core;
using RunnerGame.Interfaces;
using RunnerGame.Pooling;

namespace RunnerGame.Enemies
{
 
    public class EnemyFSM : MonoBehaviour, IPooledObject
    {
        [Header("Movement")]
        [SerializeField] private float ownMoveSpeed = 2f;
        [SerializeField] private float movingDuration = 1.5f;
        [SerializeField] private float despawnX = -15f;

        [Header("Shooting")]
        [SerializeField] private float shootingDuration = 1f;
        [SerializeField] private float fireInterval = 0.5f;
        [SerializeField] private string projectilePoolTag = "EnemyProjectile";
        [SerializeField] private Transform firePoint;

        private EnemyState _state;
        private float _stateTimer;
        private float _fireTimer;
        private Transform _player;

        public string PoolTag => "Enemy";

        private void Awake()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) _player = playerObj.transform;
        }

        public void OnSpawn()
        {
            ChangeState(EnemyState.Moving);
        }

        public void OnDespawn()
        {
            
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            
            transform.position += Vector3.left *
                (GameManager.Instance.CurrentScrollSpeed + ownMoveSpeed) * Time.deltaTime;

            _stateTimer -= Time.deltaTime;

            switch (_state)
            {
                case EnemyState.Moving:
                    if (_stateTimer <= 0f) ChangeState(EnemyState.Shooting);
                    break;

                case EnemyState.Shooting:
                    _fireTimer -= Time.deltaTime;
                    if (_fireTimer <= 0f)
                    {
                        FireProjectile();
                        _fireTimer = fireInterval;
                    }
                    if (_stateTimer <= 0f) ChangeState(EnemyState.Moving);
                    break;

                case EnemyState.Despawning:
                    break;
            }

            if (transform.position.x < despawnX && _state != EnemyState.Despawning)
            {
                ChangeState(EnemyState.Despawning);
                ObjectPooler.Instance.Despawn(gameObject);
            }
        }

        private void ChangeState(EnemyState newState)
        {
            _state = newState;
            switch (newState)
            {
                case EnemyState.Moving:
                    _stateTimer = movingDuration;
                    break;
                case EnemyState.Shooting:
                    _stateTimer = shootingDuration;
                    _fireTimer = 0f; // fire immediately on entering
                    break;
            }
        }

        private void FireProjectile()
        {
            if (ObjectPooler.Instance == null) return;

            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            var projObj = ObjectPooler.Instance.Spawn(projectilePoolTag, spawnPos, Quaternion.identity);

            if (projObj != null && projObj.TryGetComponent<Projectile>(out var proj))
            {
                
                Vector3 direction = _player != null
                    ? (_player.position - spawnPos).normalized
                    : Vector3.left;
                proj.Launch(direction);
            }
        }
    }
}
