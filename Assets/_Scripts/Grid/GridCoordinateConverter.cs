using UnityEngine;

namespace Match3._Scripts.Grid
{
    public abstract class GridCoordinateConverter
    {
        protected float CellSize;
        protected Vector3 Origin;

        protected GridCoordinateConverter(float cellSize, Vector3 origin)
        {
            CellSize = cellSize;
            Origin = origin;
        }
        public abstract Vector3 GridToWorld(int x, int y);
     
        public abstract Vector3 GridToWorldCenter(int x, int y);
     
        public abstract Vector2Int WorldToGrid(Vector3 worldPosition);
     
        public abstract Vector3 Forward { get; }
    }
}