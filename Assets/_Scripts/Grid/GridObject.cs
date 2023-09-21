namespace Match3._Scripts.Grid
{
    public class GridObject<T>
    {
        private Grid2D<GridObject<T>> _grid;
        private int _x;
        private int _y;

        public GridObject(Grid2D<GridObject<T>> grid, int x, int y)
        {
            _grid = grid;
            _x = x;
            _y = y;
        }
    }
}