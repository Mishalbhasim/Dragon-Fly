using System;

namespace RunnerGame.Events
{
   
    public static class GameEvents
    {
        public static event Action<int> OnScoreChanged;
        public static event Action<int, int> OnHealthChanged; // currentHealth, maxHealth
        public static event Action OnPlayerDied;
        public static event Action<RunnerGame.Core.GameState> OnGameStateChanged;

        public static void ScoreChanged(int newScore) => OnScoreChanged?.Invoke(newScore);
        public static void HealthChanged(int current, int max) => OnHealthChanged?.Invoke(current, max);
        public static void PlayerDied() => OnPlayerDied?.Invoke();
        public static void GameStateChanged(RunnerGame.Core.GameState newState) => OnGameStateChanged?.Invoke(newState);
    }
}
