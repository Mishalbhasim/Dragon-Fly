using UnityEngine;
using UnityEngine.UI;
using RunnerGame.Core;
using RunnerGame.Events;
using TMPro;

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
        [SerializeField] private TMP_Text finalScoreText;

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
            HandleGameStateChanged(GameManager.Instance != null ? GameManager.Instance.State : GameState.MainMenu);
        }

        // button wiring

        public void OnStartButtonPressed() => GameManager.Instance.StartGame();
        public void OnRestartButtonPressed() => GameManager.Instance.RestartGame();
        public void OnMainMenuButtonPressed() => GameManager.Instance.ReturnToMenu();

        public void OnQuitButtonPressed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // stop Play Mode when testing in-editor
#else
    Application.Quit(); // only actually quits in a real build, not in the Editor
#endif
        }


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