using UnityEngine;
using UnityEngine.UI;
using RunnerGame.Core;
using RunnerGame.Events;

namespace RunnerGame.UI
{
    
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Screens")]
        [SerializeField] private GameObject startScreen;
        [SerializeField] private GameObject hud;
        [SerializeField] private GameObject gameOverScreen;

        [Header("HUD")]
        [SerializeField] private Text scoreText;
        [SerializeField] private Text healthText;

        [Header("Game Over")]
        [SerializeField] private Text finalScoreText;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()
        {
            GameEvents.OnScoreChanged += HandleScoreChanged;
            GameEvents.OnHealthChanged += HandleHealthChanged;
            GameEvents.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnScoreChanged -= HandleScoreChanged;
            GameEvents.OnHealthChanged -= HandleHealthChanged;
            GameEvents.OnGameStateChanged -= HandleGameStateChanged;
        }

        private void Start()
        {
            HandleGameStateChanged(GameState.MainMenu);
        }

        // button hooks

        public void OnStartButtonPressed() => GameManager.Instance.StartGame();
        public void OnRestartButtonPressed() => GameManager.Instance.StartGame();
        public void OnMainMenuButtonPressed() => GameManager.Instance.ReturnToMenu();

        // Event handlers 

        private void HandleScoreChanged(int score)
        {
            if (scoreText != null) scoreText.text = $"Score: {score}";
        }

        private void HandleHealthChanged(int current, int max)
        {
            if (healthText != null) healthText.text = $"Health: {current}/{max}";
        }

        private void HandleGameStateChanged(GameState state)
        {
            startScreen?.SetActive(state == GameState.MainMenu);
            hud?.SetActive(state == GameState.Playing);
            gameOverScreen?.SetActive(state == GameState.GameOver);

            if (state == GameState.GameOver && finalScoreText != null)
                finalScoreText.text = $"Final Score: {GameManager.Instance.CurrentScore}";
        }
    }
}
