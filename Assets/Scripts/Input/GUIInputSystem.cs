using UnityEngine;
using UnityEngine.InputSystem;

namespace Othello.Input
{
    public class GUIInputSystem : MonoBehaviour, IInputSystem
    {
        [SerializeField] private Camera  _mainCamera;
        [SerializeField] private float   _cellSize = 1f;
        [SerializeField] private Vector2 _boardOrigin;

        private Vector2 _currentScreenPos;
        private bool    _dragEnded;

        public void OnDrag(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                _currentScreenPos = ctx.ReadValue<Vector2>();
        }

        public void OnDragEnd(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                _dragEnded = true;
        }

        public void Tick(InputState state)
        {
            var world = _mainCamera.ScreenToWorldPoint(
                new Vector3(_currentScreenPos.x, _currentScreenPos.y, 10f));

            state.IsDragging      = true;
            state.DragWorldX      = world.x;
            state.DragWorldY      = world.y;
            state.ActionConfirmed = false;

            if (_dragEnded)
            {
                _dragEnded = false;
                int x = Mathf.FloorToInt((world.x - _boardOrigin.x) / _cellSize);
                int y = Mathf.FloorToInt((world.y - _boardOrigin.y) / _cellSize);
                if (x >= 0 && x < 8 && y >= 0 && y < 8)
                {
                    state.ActionConfirmed = true;
                    state.ActionX = x;
                    state.ActionY = y;
                }
            }
        }
    }
}
