#region

using System;
using UnityEngine;

#endregion

namespace Match3._Scripts.Grid
{
    public class Grid2D<T>
    {
        private GridCoordinateConverter _gridCoordinateConverter;
        private T[,] _gridArray;
        private int _height;
        private int _width;

        public static Grid2D<T> CreateVertical(int width, int height, float cellSize,
            Vector3 origin, bool isDebug = false)
        {
            return new Grid2D<T>(width, height, new VerticalConverter(cellSize, origin), isDebug);
        }

        private Grid2D(int width, int height, GridCoordinateConverter gridCoordinateConverter, bool isDebug = false)
        {
            _width = width;
            _height = height;
            _gridCoordinateConverter = gridCoordinateConverter;

            _gridArray = new T[width, height];

            if (isDebug)
            {
                DrawDebugLines();
            }
        }

        public T this[int x, int y]
        {
            get => GetValue(x, y);
            set => SetValue(x, y, value);
        }

        public T this[Vector3 worldPosition]
        {
            get
            {
                var position = GetXY(worldPosition);
                return GetValue(position.x, position.y);
            }
            set
            {
                var position = GetXY(worldPosition);
                SetValue(position.x, position.y, value);
            }
        }

        private void SetValue(int x, int y, T value)
        {
            if (!CoordinatesAreValid(x, y))
            {
                throw new ArgumentException("Incorrect coordinates");
            }

            _gridArray[x, y] = value;
            OnValueChanged?.Invoke(x, y, value);
        }

        private T GetValue(int x, int y)
        {
            if (!CoordinatesAreValid(x, y))
            {
                throw new ArgumentException("Incorrect coordinates");
            }

            return _gridArray[x, y];
        }

        public event Action<int, int, T> OnValueChanged;

        private bool CoordinatesAreValid(int x, int y) => x >= 0 && y >= 0 && x < _width && y < _height;

        public Vector2Int GetXY(Vector3 worldPosition) =>
            _gridCoordinateConverter.WorldToGrid(worldPosition);

        public Vector3 GetWorldPositionCenter(int x, int y) =>
            _gridCoordinateConverter.GridToWorldCenter(x, y);

        private Vector3 GetWorldPosition(int x, int y)
        {
            return _gridCoordinateConverter.GridToWorld(x, y);
        }

        private void DrawDebugLines()
        {
            const float duration = 100f;
            var parent = new GameObject("Debug Text parent");

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Helper.CreateWorldText(parent, $"{x},{y}", GetWorldPositionCenter(x, y), _gridCoordinateConverter.Forward);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, duration);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, duration);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, duration);
            Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _height), Color.white, duration);
        }
    }
}