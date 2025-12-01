using System;
using UnityEngine;

namespace MineSweeper
{
    public class InputManager : Singleton<IInputManagerSource>, IInputManagerSource
    {
        public event Action OnRightClickPressed;
        public event Action OnLeftClickPressed;

        private void Update()
        {
            GetMouseWorldLocation();

            if (Input.GetMouseButtonDown(1))
            {
                OnRightClickPressed?.Invoke();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                OnLeftClickPressed?.Invoke();
            }
        }

        public Vector3 GetMouseWorldLocation()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
