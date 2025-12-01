using UnityEngine;

namespace MineSweeper
{
    public struct Cell
    {
        public enum Type
        {
            Invalid,
            Empty,
            Mine,
            Number
        }

        public Vector3Int position;
        public Type type;
        public int value;
        public bool revelead, flagged, exploted;
    }
}
