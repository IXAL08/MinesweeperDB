using UnityEngine;

namespace MineSweeper
{
    [CreateAssetMenu(fileName = "DifficultySettings", menuName = "Minesweeper/DifficultySettings")]
    public class GameSettingsSO : ScriptableObject
    {
        public Difficulty DifficultyGame;
        public int WidthBoard;
        public int HeightBoard;
        public int MineQuantity;
    }
}
