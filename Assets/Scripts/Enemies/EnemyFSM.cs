using UnityEngine;
using RunnerGame.Core;
using RunnerGame.Interfaces;
using RunnerGame.Pooling;

namespace RunnerGame.Enemies
{
    /// <summary>
    /// Simple, explicit Finite State Machine — no NavMesh, no pathfinding.
    /// Moving: drifts left with the world scroll (plus its own fixed
    ///         approach speed) until it reaches a randomly-rolled stop
    ///         position — a position target, not a timer, so it stays
    ///         unpredictable regardless of how fast the world is scrolling.
    ///         The roll can land past despawnX, in which case the enemy
    ///         never stops/shoots at all before it's recycled.
    /// Shooting: pauses movement, brief wind-up, fires exactly one shot,
    ///           then resumes Moving. Each enemy only ever fires once.
    /// Despawning: once off the left edge of the screen, returns to pool.
    ///
    /// Implements IPooledObject so ObjectPooler can reset its state cleanly
    /// every time it's reused.
    /// </summary>
    public class EnemyFSM : MonoBehaviour, IPooledObject
    {
        [Header("Movement")]
        [SerializeField] private float ownMoveSpeed = 2f;
        [SerializeField] private float despawnX = -15f;
        [Tooltip("How far past despawnX the random stop-target can roll. Bigger = more likely this enemy never stops/shoots at all before despawning.")]
        [SerializeField] private float neverStopMargin = 5f;

        [Header("Shooting")]
        [SerializeField] private float fireDelay = 0.35f; // wind-up before the single shot
        [SerializeField] private GameObject projectilePrefab; // used only for pool tag reference in inspector
        [SerializeField] private string projectilePoolTag = "EnemyProjectile";
        [SerializeField] private Transform firePoint;

        private EnemyState _state;
        private float _fireTimer;
        private float _targetStopX;
        private bool _hasFired;
        private Transform _player;

        public string PoolTag => "Enemy";

        private void Awake()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) _player = playerObj.transform;
        }

        public void OnSpawn()
        {
            // Random point somewhere between "shoot immediately from spawn"
            // and "past despawn, so it never shoots at all" — decoupled
            // from scroll speed entirely, since it's a position target,
            // not a timer.
            _targetStopX = Random.Range(despawnX - neverStopMargin, transform.position.x);
            _hasFired = false;
            ChangeState(EnemyState.Moving);
        }

        public void OnDespawn()
        {
            // Nothing to clean up currently; placeholder for future coroutines/VFX.
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            // Always drift with the world scroll so it stays visually locked
            // to the environment, plus a little independent motion — except
            // while Shooting, where it holds position.
            if (_state != EnemyState.Shooting)
            {
                transform.position += Vector3.left *
                    (GameManager.Instance.CurrentScrollSpeed + ownMoveSpeed) * Time.deltaTime;
            }

            switch (_state)
            {
                case EnemyState.Moving:
                    // Only ever transitions to Shooting once per enemy.
                    if (!_hasFired && transform.position.x <= _targetStopX)
                        ChangeState(EnemyState.Shooting);
                    break;

                case EnemyState.Shooting:
                    _fireTimer -= Time.deltaTime;
                    if (_fireTimer <= 0f)
                    {
                        FireProjectile();
                        _hasFired = true;
                        ChangeState(EnemyState.Moving); // resume drifting after the single shot
                    }
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
            if (newState == EnemyState.Shooting)
                _fireTimer = fireDelay;
        }

        private void FireProjectile()
        {
            if (ObjectPooler.Instance == null) return;

            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            var projObj = ObjectPooler.Instance.Spawn(projectilePoolTag, spawnPos, Quaternion.identity);

            if (projObj != null && projObj.TryGetComponent<Projectile>(out var proj))
            {
                // Straight-line shot toward the player's current position —
                // no lead/prediction required by spec.
                Vector3 direction = _player != null
                    ? (_player.position - spawnPos).normalized
                    : Vector3.left;

                // World only scrolls left — never let a shot point
                // backward/right, even if the enemy somehow ends up left
                // of the player.
                if (direction.x > -0.15f)
                    direction = Vector3.left;

                proj.Launch(direction);
            }
        }
    }
}