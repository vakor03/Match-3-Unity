#region

using UnityEngine;

#endregion

namespace Match3._Scripts.Grid
{
    public class VerticalConverter : GridCoordinateConverter
    {
        public VerticalConverter(float cellSize, Vector3 origin) : base(cellSize, origin)
        {
        }

        public override Vector3 Forward => Vector3.forward;

        public override Vector3 GridToWorld(int x, int y)
        {
            return new Vector3(x, y, 0) * CellSize + Origin;
        }

        public override Vector3 GridToWorldCenter(int x, int y)
        {
            return new Vector3(x * CellSize + CellSize * 0.5f, y * CellSize + CellSize * 0.5f, 0) + Origin;
        }

        public override Vector2Int WorldToGrid(Vector3 worldPosition)
        {
            Vector3 gridPosition = (worldPosition - Origin) / CellSize;
            var x = Mathf.FloorToInt(gridPosition.x);
            var y = Mathf.FloorToInt(gridPosition.y);
            return new Vector2Int(x, y);
        }
    }
}