using UnityEngine;
using RunnerGame.Core;

namespace RunnerGame.Debugging
{
    public class TestAutoStart : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance.StartGame();
        }
    }
}