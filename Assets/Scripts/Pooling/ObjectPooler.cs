using System.Collections.Generic;
using UnityEngine;
using RunnerGame.Interfaces;

namespace RunnerGame.Pooling
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size = 10;
    }

    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler Instance { get; private set; }

        [SerializeField] private List<Pool> pools = new List<Pool>();

        private readonly Dictionary<string, Queue<GameObject>> _poolDict = new Dictionary<string, Queue<GameObject>>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            foreach (var pool in pools)
            {
                var queue = new Queue<GameObject>();
                var parent = new GameObject($"Pool_{pool.tag}").transform;
                parent.SetParent(transform);

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, parent);
                    obj.SetActive(false);
                    queue.Enqueue(obj);
                }
                _poolDict[pool.tag] = queue;
            }
        }

        public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
        {
            if (!_poolDict.TryGetValue(tag, out var queue))
            {
                Debug.LogWarning($"ObjectPooler: pool with tag '{tag}' doesn't exist.");
                return null;
            }

            GameObject obj = null;
            int attempts = queue.Count;
            for (int i = 0; i < attempts; i++)
            {
                var candidate = queue.Dequeue();
                queue.Enqueue(candidate);
                if (!candidate.activeInHierarchy)
                {
                    obj = candidate;
                    break;
                }
            }
            if (obj == null) obj = queue.Peek();

            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);

            if (obj.TryGetComponent<IPooledObject>(out var pooled))
                pooled.OnSpawn();

            return obj;
        }

        public void Despawn(GameObject obj)
        {
            if (obj.TryGetComponent<IPooledObject>(out var pooled))
                pooled.OnDespawn();
            obj.SetActive(false);
        }

        public void DespawnAll()
        {
            foreach (var queue in _poolDict.Values)
            {
                foreach (var obj in queue)
                {
                    if (obj.activeInHierarchy)
                        Despawn(obj);
                }
            }
        }
    }
}