using UnityEngine;
using System;

namespace MineSweeper
{
    public interface IInputManagerSource
    {
        Vector3 GetMouseWorldLocation();

        event Action OnRightClickPressed;
        event Action OnLeftClickPressed;
    }
}
