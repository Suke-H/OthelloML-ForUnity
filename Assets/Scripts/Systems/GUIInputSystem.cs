using Othello.Object;
using Othello.States;
using UnityEngine;

namespace Othello.Systems
{
    public class GUIInputSystem : MonoBehaviour, IInputSystem
    {
        [SerializeField] private Camera  _mainCamera;
        [SerializeField] private float   _cellSize = 1f;
        [SerializeField] private Vector2 _boardOrigin;

        private DraggableStone _stone;
        private bool           _actionConfirmed;
        private int            _actionX;
        private int            _actionY;

        public void Init(DraggableStone stone)
        {
            _stone = stone;
        }

        // DraggableStone から呼ばれる
        public void OnStoneDropped(Vector3 worldPos)
        {
            int x = Mathf.RoundToInt((worldPos.x - _boardOrigin.x) / _cellSize);
            int y = Mathf.RoundToInt((_boardOrigin.y - worldPos.y) / _cellSize);

            if (x >= 0 && x < 8 && y >= 0 && y < 8)
            {
                _actionConfirmed = true;
                _actionX         = x;
                _actionY         = y;
            }
            else
            {
                _stone.ResetPosition();
            }
        }

        public void Tick(InputState state)
        {
            state.ActionConfirmed = false;

            if (_actionConfirmed)
            {
                _actionConfirmed      = false;
                state.ActionConfirmed = true;
                state.ActionX         = _actionX;
                state.ActionY         = _actionY;
            }
        }

    }
}
