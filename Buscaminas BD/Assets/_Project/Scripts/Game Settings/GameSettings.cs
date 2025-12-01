using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MineSweeper
{
    public class GameSettings : MonoBehaviour
    {
        [Header("UI components")]
        [SerializeField] private RectTransform _SettingMenu;
        [SerializeField] private TMP_Dropdown _difficultyDropdown;
        [SerializeField] private TMP_InputField _widthInputField;
        [SerializeField] private TMP_InputField _heightInputField;
        [SerializeField] private TMP_InputField _minesInputField;
        [SerializeField] private Button _playButton;

        private int _minValue = 0, _maxValue = 25;


        private void Start()
        {
            GameManager.Source.OnGameOver += ActiveSettingMenu;
            _difficultyDropdown.onValueChanged.AddListener(delegate { ChangeUI(); });
            InitializeDropDown();
            InitializeInputFields();
            _playButton.onClick.AddListener(InitializeGame);
        }

        private void InitializeDropDown()
        {
            _difficultyDropdown.ClearOptions();

            _difficultyDropdown.AddOptions(System.Enum.GetNames(typeof(Difficulty)).ToList());
            ChangeUI();
        }

        private void InitializeInputFields()
        {
            _widthInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            _heightInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            _minesInputField.contentType = TMP_InputField.ContentType.IntegerNumber;

            _widthInputField.onValueChanged.AddListener(value => RestrictValues(_widthInputField, value));
            _heightInputField.onValueChanged.AddListener(value => RestrictValues(_heightInputField, value));
            _minesInputField.onValueChanged.AddListener(value => RestrictValues(_minesInputField, value));
        }

        private void InitializeGame()
        {
            DatabaseManager.Source.RestartData();
            Difficulty difficulty = (Difficulty)_difficultyDropdown.value;
            int width = 0, height = 0, mines = 0;

            int.TryParse(_widthInputField.text, out width);
            int.TryParse(_heightInputField.text, out height);
            int.TryParse(_minesInputField.text, out mines);

            GameManager.Source.GetUIValue(difficulty, width, height, mines);
            DatabaseManager.Source.GetBoardSettings(difficulty.ToString(), width, height, mines);
        }

        private void RestrictValues(TMP_InputField inputField, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            // Evitar recursión: no ejecutar lógica si el valor no cambió realmente
            bool parsingSuccess = int.TryParse(value, out int number);

            if (!parsingSuccess)
            {
                inputField.SetTextWithoutNotify("");
                return;
            }

            int min = _minValue;
            int max = _maxValue;

            if (inputField == _minesInputField)
            {
                if (int.TryParse(_widthInputField.text, out int width) &&
                    int.TryParse(_heightInputField.text, out int height))
                {
                    max = width * height;
                }
                else
                {
                    max = 25;
                }
            }

            if (number < min)
                inputField.SetTextWithoutNotify(min.ToString());
            else if (number > max)
                inputField.SetTextWithoutNotify(max.ToString());
        }

        private void ChangeUI()
        {
            if (_difficultyDropdown.value < 3)
            {
                DeactivateInputField(_difficultyDropdown.value);
            }
            else
            {
                ActivateInputField();
            }
        }

        private void DeactivateInputField(int value)
        {
            if (value == 0)
            {
                _widthInputField.text = "9";
                _widthInputField.interactable = false;
                _heightInputField.text = "9";
                _heightInputField.interactable = false;
                _minesInputField.text = "10";
                _minesInputField.interactable = false;
            }

            else if (value == 1)
            {
                _widthInputField.text = "16";
                _widthInputField.interactable = false;
                _heightInputField.text = "16";
                _heightInputField.interactable = false;
                _minesInputField.text = "40";
                _minesInputField.interactable = false;
            }

            else if (value == 2)
            {
                _widthInputField.text = "30";
                _widthInputField.interactable = false;
                _heightInputField.text = "16";
                _heightInputField.interactable = false;
                _minesInputField.text = "99";
                _minesInputField.interactable = false;
            }
        }

        private void ActivateInputField()
        {
            _widthInputField.interactable = true;
            _heightInputField.interactable = true;
            _minesInputField.interactable = true;
        }

        public void ActiveSettingMenu() => ShowSettings().Forget();

        public async UniTask HideSettings()
        {
            await _SettingMenu.DOAnchorPosX(-300, 1f).AsyncWaitForCompletion(); ;
        }

        private async UniTask ShowSettings()
        {
            await _SettingMenu.DOAnchorPosX(0, 1f).AsyncWaitForCompletion();
        }
    }
}
