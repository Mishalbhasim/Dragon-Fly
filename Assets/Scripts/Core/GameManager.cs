using UnityEngine;
using UnityEngine.SceneManagement;
using RunnerGame.Events;

namespace RunnerGame.Core
{
    public enum GameState
    {
        MainMenu,
        Playing,
        GameOver
    }


    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; } = GameState.MainMenu;

        [Header("Difficulty Scaling")]
        [Tooltip("World scroll speed at game start.")]
        public float baseScrollSpeed = 6f;
        [Tooltip("Units/sec the scroll speed gains per second played.")]
        public float scrollSpeedRampPerSecond = 0.05f;
        [Tooltip("Hard cap so difficulty doesn't spiral out of control.")]
        public float maxScrollSpeed = 16f;

        public float CurrentScrollSpeed { get; private set; }

        private float _playTimer;
        private int _score;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (State != GameState.Playing) return;

            _playTimer += Time.deltaTime;
            CurrentScrollSpeed = Mathf.Min(
                baseScrollSpeed + _playTimer * scrollSpeedRampPerSecond,
                maxScrollSpeed);
        }

        // FSM transitions

        public void StartGame()
        {
            _score = 0;
            _playTimer = 0f;
            CurrentScrollSpeed = baseScrollSpeed;

            if (RunnerGame.Pooling.ObjectPooler.Instance != null)
                RunnerGame.Pooling.ObjectPooler.Instance.DespawnAll();

            SetState(GameState.Playing);
            GameEvents.ScoreChanged(_score);
        }

        public void EndGame()
        {
            SetState(GameState.GameOver);
        }

        public void ReturnToMenu()
        {
            SetState(GameState.MainMenu);
        }


        public void RestartGame()
        {
            SceneManager.sceneLoaded += OnSceneReloaded;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnSceneReloaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneReloaded;
            StartGame();
        }

        private void SetState(GameState newState)
        {
            State = newState;
            GameEvents.GameStateChanged(newState);
        }

        // Scoring

        public void AddScore(int amount)
        {
            if (State != GameState.Playing) return;
            _score += amount;
            GameEvents.ScoreChanged(_score);
        }

        public int CurrentScore => _score;
    }
}