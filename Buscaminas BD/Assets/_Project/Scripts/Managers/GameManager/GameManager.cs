using Cysharp.Threading.Tasks;
using UnityEngine;


namespace MineSweeper
{
    public class GameManager : Singleton<IGameManagerSource>, IGameManagerSource
    {
        [Header("Game Stats")]
        [SerializeField] private float _gameTime;
        [Header("Game Settings")]
        [SerializeField] private Difficulty _difficultyGame; 
        [SerializeField] private int _widthBoard;
        [SerializeField] private int _heightBoard;
        [SerializeField] private int _mineQuantity;
        [SerializeField] private bool _isGameover = true;
        [Header("Reference")]
        [SerializeField] private GameBoard _gameboard;
        [SerializeField] private Timer _timer;
        [SerializeField] private GameSettings _gamesettings;

        private Cell[,] _state;

        public event System.Action OnGameOver;
        public bool GameOver => _isGameover; 

        private void OnEnable()
        {
            InputManager.Source.OnRightClickPressed += Flag;
            InputManager.Source.OnLeftClickPressed += Reveal;
        }

        private void OnDisable()
        {
            InputManager.Source.OnRightClickPressed -= Flag;
            InputManager.Source.OnLeftClickPressed -= Reveal;
        }

        private void Update()
        {
            if (_isGameover) return;
            _gameTime = _timer.GetGameTime();
        }

        public void NewGame()
        {
            SetUpDifficulty();
            _state = new Cell[_widthBoard, _heightBoard];
            GenerateCells();
            GenerateMines();
            GenerateNumbers();

            _gameboard.Draw(_state);
            Camera.main.transform.position = new Vector3(_widthBoard/2, _heightBoard/2, -10f); //20 camera 30x30
            _timer.ResetTimer();
            _gamesettings.HideSettings().Forget();
        }

        private void GenerateCells()
        {
            for (int x = 0; x < _widthBoard; x++)
            {
                for (int y = 0; y < _heightBoard; y++)
                {
                    Cell cell = new Cell();
                    cell.position = new Vector3Int(x, y, 0);
                    cell.type = Cell.Type.Empty;
                    _state[x, y] = cell;
                }
            }
        }

        private void GenerateMines()
        {
            int x, y;

            for (int i = 0; i < _mineQuantity; i++)
            {
                do
                {
                    x = Random.Range(0, _widthBoard);
                    y = Random.Range(0, _heightBoard);
                }while (_state[x,y].type == Cell.Type.Mine);

                _state[x, y].type = Cell.Type.Mine;
            }
        }

        private void GenerateNumbers()
        {
            for (int x = 0; x < _widthBoard; x++)
            {
                for (int y = 0; y < _heightBoard; y++)
                {
                    Cell cell = _state[x, y];

                    if (cell.type == Cell.Type.Mine) continue;

                    cell.value = CountMines(x,y);
                    if (cell.value > 0)
                    {
                        cell.type = Cell.Type.Number;
                    }

                    _state[x, y] = cell;
                }
            }
        }

        private int CountMines(int cellX, int cellY)
        {
            int count = 0;

            for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
            {
                for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
                {
                    if (adjacentX == 0 &&  adjacentY == 0) continue;

                    int x = cellX + adjacentX;
                    int y = cellY + adjacentY;

                    if (GetCell(x,y).type == Cell.Type.Mine) count++;
                }
            }

            return count;
        }

        private void Flag()
        {
            if (_isGameover) return;
            DatabaseManager.Source.AddStat(MatchStat.TotalClicks);

            Vector3 worldPosition = InputManager.Source.GetMouseWorldLocation();
            Vector3Int cellPosition = _gameboard._tileMap.WorldToCell(worldPosition);
            Cell cell = GetCell(cellPosition.x, cellPosition.y);

            if (cell.type == Cell.Type.Invalid || cell.revelead) return;
            if (cell.type == Cell.Type.Mine)
            {
                DatabaseManager.Source.AddStat(MatchStat.Disarmedmines,cell.flagged);
            }
            cell.flagged = !cell.flagged;
            _state[cellPosition.x, cellPosition.y] = cell;
            _gameboard.Draw(_state);
        }

        private void Reveal()
        {
            if (_isGameover) return;
            DatabaseManager.Source.AddStat(MatchStat.TotalClicks);

            Vector3 worldPosition = InputManager.Source.GetMouseWorldLocation();
            Vector3Int cellPosition = _gameboard._tileMap.WorldToCell(worldPosition);
            Cell cell = GetCell(cellPosition.x, cellPosition.y);

            if (cell.type == Cell.Type.Invalid || cell.revelead || cell.flagged) return;

            switch (cell.type)
            {
                case Cell.Type.Mine:
                    Explote(cell);
                    break;

                case Cell.Type.Empty:
                    Flood(cell);
                    CheckWinCondition();
                    break;

                default:
                    cell.revelead = true;
                    _state[cellPosition.x, cellPosition.y] = cell;
                    CheckWinCondition();
                    break;
            }

            _gameboard.Draw(_state);
        }

        private void Flood(Cell cell)
        {
            if (cell.revelead || cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) return;

            cell.revelead = true;
            _state[cell.position.x, cell.position.y] = cell;
            DatabaseManager.Source.AddStat(MatchStat.Discoverescells);

            if (cell.type == Cell.Type.Empty)
            {
                Flood(GetCell(cell.position.x - 1, cell.position.y));
                Flood(GetCell(cell.position.x + 1, cell.position.y));
                Flood(GetCell(cell.position.x, cell.position.y - 1));
                Flood(GetCell(cell.position.x, cell.position.y + 1));
                Flood(GetCell(cell.position.x - 1, cell.position.y - 1));
                Flood(GetCell(cell.position.x - 1, cell.position.y + 1));
                Flood(GetCell(cell.position.x + 1, cell.position.y - 1));
                Flood(GetCell(cell.position.x + 1, cell.position.y + 1));
            }
        }

        private void Explote(Cell cell)
        {
            _isGameover = true;
            DatabaseManager.Source.GetMatchData(_gameTime, 0);
            OnGameOver?.Invoke();

            cell.revelead = true;
            cell.exploted = true;
            _state[cell.position.x, cell.position.y] = cell;

            for (int x = 0; x < _widthBoard; x++)
            {
                for (int y = 0; y < _heightBoard; y++)
                {
                    cell = _state[x, y];

                    if (cell.type == Cell.Type.Mine)
                    {
                        cell.revelead = true;
                        _state[x,y] = cell;
                    }
                }
            }
        }

        private void CheckWinCondition()
        {
            for (int x = 0; x < _widthBoard; x++)
            {
                for (int y = 0; y < _heightBoard; y++)
                {
                    Cell cell = _state[x, y];

                    if ( cell.type != Cell.Type.Mine && !cell.revelead) return;
                }
            }

            Debug.Log("Win");
            RevealCells();
            _isGameover = true;
            DatabaseManager.Source.GetMatchData(_gameTime, 1);
            OnGameOver?.Invoke();
        }

        private Cell GetCell(int x, int y)
        {
            if (IsValid(x, y))
            {
                return _state[x, y];
            }
            else
            {
                return new Cell();
            }
        }

        private bool IsValid(int x, int y)
        {
            return x >= 0 && x < _widthBoard && y >= 0 && y < _heightBoard;
        }

        private void SetUpDifficulty()
        {
            _isGameover = false;

            switch (_difficultyGame)
            {
                case Difficulty.Beginner:
                    _widthBoard = 9;
                    _heightBoard = 9;
                    _mineQuantity = 10;
                    break;

                case Difficulty.Intermediate:
                    _widthBoard = 16;
                    _heightBoard = 16;
                    _mineQuantity = 40;
                    break;

                case Difficulty.Expert:
                    _widthBoard = 30;
                    _heightBoard = 16;
                    _mineQuantity = 99;
                    break;

                case Difficulty.Custom:
                    break;
            }
        }

        private void RevealCells()
        {
            for (int x = 0; x < _widthBoard; x++)
            {
                for (int y = 0; y < _heightBoard; y++)
                {
                    Cell cell = _state[x, y];

                    if (cell.type == Cell.Type.Number || cell.type == Cell.Type.Empty)
                    {
                        cell.revelead = true;
                        _state[x, y] = cell;
                    }
                    if (cell.type == Cell.Type.Mine && !cell.flagged)
                    {
                        cell.flagged = true;
                        DatabaseManager.Source.AddStat(MatchStat.Disarmedmines);
                        _gameboard.Draw(_state);
                    }
                }
            }
        }

        public void GetUIValue(Difficulty difficulty, int width, int height, int mines)
        {
            _difficultyGame = difficulty;
            _widthBoard = width;
            _heightBoard = height;
            _mineQuantity = mines;

            if (_widthBoard == 0 || _heightBoard == 0 || _mineQuantity > (_widthBoard * _heightBoard)) return;
            NewGame();
        }

    }

    public enum Difficulty
    {
        Beginner,
        Intermediate,
        Expert,
        Custom
    }
}
