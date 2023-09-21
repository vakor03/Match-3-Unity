using UnityEngine;

namespace Match3._Scripts.Grid
{
    public class GameManager : MonoBehaviour
    {
        [Header("Grid settings")] 
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 8;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Vector3 originPosition = Vector3.zero;
        [SerializeField] private bool debug = true;

        private Grid2D<GameObject> _grid2D;
        private void Awake()
        {
            _grid2D = Grid2D<GameObject>.CreateVertical(width, height, cellSize, originPosition, debug);
        }
    }
}