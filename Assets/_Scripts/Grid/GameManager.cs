#region

using System;
using System.Collections;
using DG.Tweening;
using Match3._Scripts.Core;
using Match3._Scripts.Gems;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

namespace Match3._Scripts.Grid
{
    public class GameManager : MonoBehaviour
    {
        [Header("Grid settings")] [SerializeField]
        private int width = 8;

        [SerializeField] private int height = 8;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Vector3 originPosition = Vector3.zero;
        [SerializeField] private bool debug = true;

        [SerializeField] private Gem gemPrefab;
        [SerializeField] private GemSO[] gemSOs;

        [SerializeField] private Ease ease = Ease.InQuad;
        [SerializeField] private InputReader inputReader;

        private Grid2D<GridObject<Gem>> _grid;

        private Vector2Int _selectedGem = Vector2Int.one * -1;

        private void Start()
        {
            _grid = Grid2D<GridObject<Gem>>.CreateVertical(width, height, cellSize, originPosition, debug);

            FillGrid();

            inputReader.Fire += OnSelectGem;
        }

        private void OnDestroy()
        {
            inputReader.Fire -= OnSelectGem;
        }

        private void OnSelectGem()
        {
            var gridPosition = _grid.GetXY(Camera.main.ScreenToWorldPoint(inputReader.Selected));

            if (!IsValidPosition(gridPosition) || IsEmptyPosition(gridPosition))
            {
                return;
            }
            
            if (_selectedGem == gridPosition)
            {
                Debug.Log($"Deselect gem at {gridPosition}");
                DeselectGem();
            }
            else if (_selectedGem == Vector2Int.one * -1)
            {
                Debug.Log($"Select gem at {gridPosition}");
                SelectGem(gridPosition);
            }
            else
            {
                Debug.Log($"Swap gem at {_selectedGem} with gem at {gridPosition}");
                StartCoroutine(RunGameLoopRoutine(_selectedGem, gridPosition));
            }
        }

        private bool IsEmptyPosition(Vector2Int gridPosition)
        {
            return _grid[gridPosition.x, gridPosition.y] == null;
        }

        private bool IsValidPosition(Vector2Int gridPosition)
        {
            return _grid.CoordinatesAreValid(gridPosition.x, gridPosition.y);
        }

        private IEnumerator RunGameLoopRoutine(Vector2Int selectedGem, Vector2Int gridPosition)
        {
            yield return StartCoroutine(SwapGemsRoutine(selectedGem, gridPosition));

            DeselectGem();
            
            yield return null;
        }
        
        private IEnumerator SwapGemsRoutine(Vector2Int gridPositionA, Vector2Int gridPositionB)
        {
            GridObject<Gem> gridObjectA = _grid[gridPositionA.x, gridPositionA.y];
            GridObject<Gem> gridObjectB = _grid[gridPositionB.x, gridPositionB.y];
            
            // Swap animation
            var animationDuration = 0.2f;
            
            gridObjectA.GetValue().transform
                .DOLocalMove(_grid.GetWorldPositionCenter(gridPositionB.x, gridPositionB.y), animationDuration)
                .SetEase(ease);

            gridObjectB.GetValue().transform
                .DOLocalMove(_grid.GetWorldPositionCenter(gridPositionA.x, gridPositionA.y), animationDuration)
                .SetEase(ease);
            
            _grid[gridPositionA.x, gridPositionA.y] = gridObjectB;
            _grid[gridPositionB.x, gridPositionB.y] = gridObjectA;
            //TODO: Grid object (x, y) is not updated
            
            yield return new WaitForSeconds(animationDuration);
        }

        private void SelectGem(Vector2Int gridPosition)
        {
            _selectedGem = gridPosition;
        }

        private void DeselectGem()
        {
            _selectedGem = Vector2Int.one * -1;
        }

        private void FillGrid()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GemSO gemSO = gemSOs[Random.Range(0, gemSOs.Length)];
                    CreateGem(x, y, gemSO);
                }
            }
        }

        private void CreateGem(int x, int y, GemSO gemSO)
        {
            Gem gem = Instantiate(gemPrefab, _grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
            gem.SetGemSO(gemSO);

            var gridObject = new GridObject<Gem>(_grid, x, y);
            gridObject.SetValue(gem);

            _grid[x, y] = gridObject;
        }
    }

    // Init grid

    // Read player input

    // Start coroutine
    // Swap animation
    // Check if swap is valid
    // Matches
    // Make gems explode
    // Fill empty spaces
    // Check if game is over
}