using UnityEngine;

namespace MineSweeper
{
    public interface IDatabaseSource
    {
        void GetBoardSettings(string difficulty, int width, int height, int mines);
        void GetMatchData(float time, int result);
        void AddStat(MatchStat stat, bool isFlagged = false);
        void RestartData();
    }
}
