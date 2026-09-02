using UnityEngine;
using RunnerGame.Interfaces;
using RunnerGame.Events;
using RunnerGame.Core;

namespace RunnerGame.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 3;
        [Tooltip("Brief invulnerability after being hit so one obstacle can't drain all lives in one collision.")]
        [SerializeField] private float invulnerabilityDuration = 1f;

        private int _currentHealth;
        private float _invulnerableUntil;

        private void OnEnable()
        {
            ResetHealth();
            GameEvents.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStateChanged -= HandleGameStateChanged;
        }

        private void HandleGameStateChanged(GameState newState)
        {
            if (newState == GameState.Playing)
                ResetHealth();
        }

        public void ResetHealth()
        {
            _currentHealth = maxHealth;
            _invulnerableUntil = 0f;
            GameEvents.HealthChanged(_currentHealth, maxHealth);
        }

        public void TakeDamage(int amount)
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;
            if (Time.time < _invulnerableUntil) return;

            _currentHealth = Mathf.Max(0, _currentHealth - amount);
            _invulnerableUntil = Time.time + invulnerabilityDuration;
            GameEvents.HealthChanged(_currentHealth, maxHealth);

            if (_currentHealth <= 0)
            {
                GameEvents.PlayerDied();
                GameManager.Instance.EndGame();
            }
        }
    }
}