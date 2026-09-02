using UnityEngine;
using RunnerGame.Core;

namespace RunnerGame.Environment
{
    
    public class GroundLooper : MonoBehaviour
    {
        [Tooltip("The tiles, in left-to-right order, that make up the loop.")]
        [SerializeField] private Transform[] tiles;

        [Tooltip("Length of a single tile along the scroll axis (X).")]
        [SerializeField] private float tileLength = 20f;

        [Tooltip("0 = locked to camera speed (foreground). <1 = parallax background, moves slower.")]
        [Range(0f, 1f)]
        [SerializeField] private float parallaxFactor = 1f;

        [Tooltip("World X position behind which a tile is considered 'passed' and gets recycled.")]
        [SerializeField] private float recycleThresholdX = -12f;

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            float speed = GameManager.Instance.CurrentScrollSpeed * parallaxFactor;
            float delta = speed * Time.deltaTime;

            foreach (var tile in tiles)
            {
                tile.position += Vector3.left * delta;
            }

            
            foreach (var tile in tiles)
            {
                if (tile.position.x < recycleThresholdX)
                {
                    float rightmostX = GetRightmostX();
                    Vector3 pos = tile.position;
                    pos.x = rightmostX + tileLength;
                    tile.position = pos;
                }
            }
        }

        private float GetRightmostX()
        {
            float max = float.MinValue;
            foreach (var tile in tiles)
                if (tile.position.x > max) max = tile.position.x;
            return max;
        }
    }
}
