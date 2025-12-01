using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MineSweeper
{
    public class DatabaseManager : Singleton<IDatabaseSource>, IDatabaseSource
    {
        [Header("Configuration Match Data")]
        [SerializeField] private string _difficulty;
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private int _mines;
        [Header("Match Data")]
        [SerializeField] private float _time;
        [SerializeField] private int _result;
        [SerializeField] private int _disarmedMines;
        [SerializeField] private int _discoveredCells;
        [SerializeField] private int _totalClicks;
        [Header("API")]
        [SerializeField] private string _apiURL = "http://localhost/minesweeper_api/save_match.php";
        [SerializeField] private string _difficultyKey = "difficulty";
        [SerializeField] private string _widthKey = "width";
        [SerializeField] private string _heightKey = "height";
        [SerializeField] private string _minesKey = "mines";
        [SerializeField] private string _timeKey = "time";
        [SerializeField] private string _resultKey = "result";
        [SerializeField] private string _disarmedMinesKey = "disarmedMines";
        [SerializeField] private string _discoveredCellsKey = "discoveredCells";
        [SerializeField] private string _totalClicksKey = "totalClicks";

        private void Start()
        {
            GameManager.Source.OnGameOver += SendDataToDataBase;
        }

        public void AddStat(MatchStat stat, bool isFlagged = false)
        {
            switch (stat)
            {
                case MatchStat.Disarmedmines:
                    if (!isFlagged)
                    {
                        _disarmedMines++;
                    }
                    else
                    {
                        _disarmedMines--;
                    }

                    break;

                case MatchStat.Discoverescells:
                    _discoveredCells++;
                    break;

                case MatchStat.TotalClicks:
                    _totalClicks++;
                    break;
            }
        }

        public void GetBoardSettings(string difficulty, int width, int height, int mines)
        {
            _difficulty = difficulty;
            _width = width; 
            _height = height;
            _mines = mines;
        }

        public void GetMatchData(float time, int result)
        {
            _time = time;
            _result = result;
        }

        public void RestartData()
        {
            _disarmedMines = 0;
            _discoveredCells = 0;
            _totalClicks = 0;
        }

        public void SendDataToDataBase()
        {
            ConectionToDataBase().Forget();
        }

        private async UniTask ConectionToDataBase()
        {
            WWWForm form = new WWWForm();
            form.AddField(_difficultyKey, _difficulty);
            form.AddField(_widthKey, _width);
            form.AddField(_heightKey, _height);
            form.AddField(_minesKey, _mines);

            form.AddField(_timeKey, _time.ToString("F2"));
            form.AddField(_resultKey, _result);
            form.AddField(_disarmedMinesKey, _disarmedMines);
            form.AddField(_discoveredCellsKey, _discoveredCells);
            form.AddField(_totalClicksKey, _totalClicks);

            using var www = UnityWebRequest.Post(_apiURL, form);

            try
            {
                await www.SendWebRequest();

                if(www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Data sent successfully: " + www.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("Error sending data: " + www.error);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Exception while sending match: " + ex.Message);
            }
        }
    }

    public enum MatchStat
    {
        Disarmedmines,
        Discoverescells,
        TotalClicks
    }
}
