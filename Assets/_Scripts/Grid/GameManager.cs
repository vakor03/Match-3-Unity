#region

using System.Collections;
using System.Collections.Generic;
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


        [Header("Animation settings")] [SerializeField]
        private float animationDuration = 0.2f;


        [SerializeField] private float fallAnimationDuration = 0.1f;
        [SerializeField] private Ease ease = Ease.InQuad;

        [SerializeField] private InputReader inputReader;
        [SerializeField] private AudioManager audioManager;

        [SerializeField] private Transform explosionPrefab;


        private Grid2D<GridObject<Gem>> _grid;

        private Vector2Int _selectedGem = Vector2Int.one * -1;

        private void Start()
        {
            CreateGrid();

            FillGrid();

            inputReader.Fire += OnSelectGem;
        }

        private void CreateGrid()
        {
            _grid = Grid2D<GridObject<Gem>>.CreateVertical(width, height, cellSize, originPosition, debug);
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
                audioManager.PlayDeselect();
            }
            else if (_selectedGem == Vector2Int.one * -1)
            {
                Debug.Log($"Select gem at {gridPosition}");
                SelectGem(gridPosition);
                audioManager.PlayClick();
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

            List<Vector2Int> matches = FindMatches();

            //TODO: Calculate score

            yield return StartCoroutine(ClearMatchesRoutine(matches));

            yield return StartCoroutine(MakeGemsFallRoutine());

            yield return StartCoroutine(FillEmptySpacesRoutine());
            
            //TODO: Check if end of game

            DeselectGem();

            yield return null;
        }

        private IEnumerator FillEmptySpacesRoutine()
        {
            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    if (GridSlotIsEmpty(x, y))
                    {
                        var gemSO = gemSOs[Random.Range(0, gemSOs.Length)];
                        CreateGem(x, y, gemSO);
                        audioManager.PlayPop();
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
        }

        private IEnumerator MakeGemsFallRoutine()
        {
            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    if (GridSlotIsEmpty(x, y))
                    {
                        yield return MakeGemsAboveFallRoutine(y, x);
                    }
                }
            }
        }

        private IEnumerator MakeGemsAboveFallRoutine(int y, int x)
        {
            for (int i = y + 1; i < _grid.Height; i++)
            {
                if (!GridSlotIsEmpty(x, i))
                {
                    var gem = _grid[x, i].GetValue();
                    _grid[x, y] = _grid[x, i];
                    _grid[x, i] = null;

                    gem.transform.DOLocalMove(_grid.GetWorldPositionCenter(x, y), fallAnimationDuration)
                        .SetEase(ease);

                    audioManager.PlayWoosh();

                    yield return new WaitForSeconds(fallAnimationDuration);
                    break;
                }
            }
        }

        private bool GridSlotIsEmpty(int x, int y)
        {
            return _grid[x, y] == null;
        }

        private IEnumerator ClearMatchesRoutine(List<Vector2Int> matches)
        {
            audioManager.PlayPop();

            foreach (var match in matches)
            {
                var gem = _grid[match.x, match.y].GetValue();
                _grid[match.x, match.y] = null;

                gem.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f, 1, 0.5f);
                audioManager.PlayPop();
                ExplodeVFX(match);
                yield return new WaitForSeconds(0.1f);


                gem.DestroyGem(0.1f);
            }
        }

        private void ExplodeVFX(Vector2Int gridPosition)
        {
            // TODO: Pooling
            var fx = Instantiate(explosionPrefab, _grid.GetWorldPositionCenter(gridPosition.x, gridPosition.y),
                Quaternion.identity);
            Destroy(fx, 1f);
        }

        private List<Vector2Int> FindMatches()
        {
            HashSet<Vector2Int> matches = new HashSet<Vector2Int>();

            FindHorizontalMatches(matches);

            FindVerticalMatches(matches);

            if (matches.Count == 0)
            {
                audioManager.PlayNoMatch();
            }
            else
            {
                audioManager.PlayMatch();
            }

            return new List<Vector2Int>(matches);
        }

        private void FindVerticalMatches(HashSet<Vector2Int> matches)
        {
            for (int y = 0; y < _grid.Height - 2; y++)
            {
                for (int x = 0; x < _grid.Width; x++)
                {
                    var gemA = _grid[x, y];
                    var gemB = _grid[x, y + 1];
                    var gemC = _grid[x, y + 2];

                    if (AreGemsFormsMatch(gemA, gemB, gemC))
                    {
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x, y + 1));
                        matches.Add(new Vector2Int(x, y + 2));
                    }
                }
            }
        }

        private void FindHorizontalMatches(HashSet<Vector2Int> matches)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                for (int x = 0; x < _grid.Width - 2; x++)
                {
                    var gemA = _grid[x, y];
                    var gemB = _grid[x + 1, y];
                    var gemC = _grid[x + 2, y];

                    if (AreGemsFormsMatch(gemA, gemB, gemC))
                    {
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x + 1, y));
                        matches.Add(new Vector2Int(x + 2, y));
                    }
                }
            }
        }

        private bool AreGemsFormsMatch(GridObject<Gem> gemA, GridObject<Gem> gemB, GridObject<Gem> gemC)
        {
            if (gemA == null || gemB == null || gemC == null)
            {
                return false;
            }

            return gemA.GetValue().GetGemSO() == gemB.GetValue().GetGemSO() &&
                   gemA.GetValue().GetGemSO() == gemC.GetValue().GetGemSO();
        }

        private IEnumerator SwapGemsRoutine(Vector2Int gridPositionA, Vector2Int gridPositionB)
        {
            GridObject<Gem> gridObjectA = _grid[gridPositionA.x, gridPositionA.y];
            GridObject<Gem> gridObjectB = _grid[gridPositionB.x, gridPositionB.y];

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
}