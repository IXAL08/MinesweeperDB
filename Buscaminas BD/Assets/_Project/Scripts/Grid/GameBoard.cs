using UnityEngine;
using UnityEngine.Tilemaps;

namespace MineSweeper
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField] private Tile[] _tileAssets;

        public Tilemap _tileMap { get; private set; }

        private void Awake()
        {
            _tileMap = GetComponent<Tilemap>();
        }

        public void Draw(Cell[,] state)
        {
            _tileMap.ClearAllTiles();

            int width = state.GetLength(0);
            int height = state.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = state[x, y];
                    _tileMap.SetTile(cell.position, GetTile(cell));
                }
            }
        }

        private Tile GetTile(Cell cell)
        {
            if (cell.revelead)
            {
                return GetRevealedTile(cell);
            }
            else if (cell.flagged) 
            {
                return _tileAssets[10];
            }
            else
            {
                return _tileAssets[12];
            }
        }

        private Tile GetRevealedTile(Cell cell)
        {
            switch (cell.type)
            {
                case Cell.Type.Empty: return _tileAssets[0];
                case Cell.Type.Mine: return cell.exploted ? _tileAssets[9] : _tileAssets[11];
                case Cell.Type.Number: return GetNumberTile(cell);
                default: return null;
            }
        }

        private Tile GetNumberTile(Cell cell)
        {
            switch (cell.value)
            {
                case 1: return _tileAssets[1];
                case 2: return _tileAssets[2];
                case 3: return _tileAssets[3];
                case 4: return _tileAssets[4];
                case 5: return _tileAssets[5];
                case 6: return _tileAssets[6];
                case 7: return _tileAssets[7];
                case 8: return _tileAssets[8];
                default: return null;
            }
        }
    }
}
