using System;
using UnityEngine;

namespace MineSweeper
{
    public interface IGameManagerSource
    {
        bool GameOver { get; }

        void NewGame();
        void GetUIValue(Difficulty difficulty, int width, int height, int mines);

        event Action OnGameOver;
    }
}
