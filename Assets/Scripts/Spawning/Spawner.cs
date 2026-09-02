using UnityEngine;
using RunnerGame.Core;
using RunnerGame.Pooling;

namespace RunnerGame.Spawning
{
    
    public class Spawner : MonoBehaviour
    {
        [Header("Spawn Point")]
        [SerializeField] private float spawnX = 15f;
        [SerializeField] private float minY = 0.5f;
        [SerializeField] private float maxY = 5.5f;

        [Header("Timing")]
        [SerializeField] private float baseSpawnInterval = 1.4f;
        [SerializeField] private float minSpawnInterval = 0.6f;

        [Header("Spawn Weights (relative, need not sum to 100)")]
        [SerializeField] private float obstacleWeight = 5f;
        [SerializeField] private float enemyWeight = 2f;
        [SerializeField] private float collectibleWeight = 4f;

        private float _timer;

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                SpawnRandom();
                float speedRatio = GameManager.Instance.CurrentScrollSpeed / GameManager.Instance.baseScrollSpeed;
                _timer = Mathf.Max(minSpawnInterval, baseSpawnInterval / speedRatio);
            }
        }

        private void SpawnRandom()
        {
            float total = obstacleWeight + enemyWeight + collectibleWeight;
            float roll = Random.Range(0f, total);

            string tag;
            if (roll < obstacleWeight) tag = "Obstacle";
            else if (roll < obstacleWeight + enemyWeight) tag = "Enemy";
            else tag = "Collectible";

            Vector3 pos = new Vector3(spawnX, Random.Range(minY, maxY), 0f);
            ObjectPooler.Instance.Spawn(tag, pos, Quaternion.identity);
        }
    }
}
