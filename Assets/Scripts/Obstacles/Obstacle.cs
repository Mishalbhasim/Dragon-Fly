using UnityEngine;
using RunnerGame.Core;
using RunnerGame.Interfaces;
using RunnerGame.Pooling;

namespace RunnerGame.Obstacles
{

    public class Obstacle : MonoBehaviour, IPooledObject
    { 
        [SerializeField] private int damage = 1;
        [SerializeField] private float despawnX = -15f;

        public string PoolTag => "Obstacle";

        public void OnSpawn() { }
        public void OnDespawn() { }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            transform.position += Vector3.left * GameManager.Instance.CurrentScrollSpeed * Time.deltaTime;

            if (transform.position.x < despawnX)
                ObjectPooler.Instance.Despawn(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage);          
            }
        }
    }
}
