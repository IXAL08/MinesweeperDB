using TMPro;
using UnityEngine;

namespace MineSweeper
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerUI;
        [SerializeField] private float _gameTime = 0f;


        private void FixedUpdate()
        {
            if (GameManager.Source.GameOver) return;

           RefreshGameTime();
        }

        private void RefreshGameTime()
        {
            _gameTime += Time.deltaTime;

            System.TimeSpan time = System.TimeSpan.FromSeconds(_gameTime);
            _timerUI.text = time.ToString(@"mm\:ss\.ff");
        }

        public float GetGameTime() => _gameTime;

        public float ResetTimer() => _gameTime = 0f;
    }
}
