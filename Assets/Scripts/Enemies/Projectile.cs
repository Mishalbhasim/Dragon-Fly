using UnityEngine;
using RunnerGame.Interfaces;
using RunnerGame.Pooling;

namespace RunnerGame.Enemies
{
    
    public class Projectile : MonoBehaviour, IPooledObject
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private int damage = 1;
        [SerializeField] private float lifetime = 4f;
        [SerializeField] private float despawnX = -15f;

        public string PoolTag => "EnemyProjectile";

        private Vector3 _direction = Vector3.left;
        private float _timer;

        public void Launch(Vector3 direction)
        {
            _direction = direction.normalized;
        }

        public void OnSpawn()
        {
            _timer = lifetime;
        }

        public void OnDespawn() { }

        private void Update()
        {
            transform.position += _direction * speed * Time.deltaTime;

            _timer -= Time.deltaTime;
            if (_timer <= 0f || transform.position.x < despawnX)
            {
                ObjectPooler.Instance.Despawn(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage);
                ObjectPooler.Instance.Despawn(gameObject);
            }
        }
    }
}
