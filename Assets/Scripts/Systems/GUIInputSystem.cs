using Othello.Object;
using Othello.States;
using UnityEngine;

namespace Othello.Systems
{
    public class GUIInputSystem : MonoBehaviour, IInputSystem
    {
        [SerializeField] private Camera     _mainCamera;
        [SerializeField] private float      _cellSize      = 1f;
        [SerializeField] private Vector2    _boardOrigin;
        [SerializeField] private GameObject _stonePrefab;
        [SerializeField] private Vector3    _spawnPosition = new Vector3(-5f, 0f, 0f);

        private DraggableStone _stone;
        private bool           _actionConfirmed;
        private int            _actionX;
        private int            _actionY;

        private void Start()
        {
            SpawnStone(Player.Black);
        }

        public void SetCurrentPlayer(Player player)
        {
            if (_stone == null) return;
            _stone.GetComponent<SpriteRenderer>().color =
                player == Player.Black ? Color.black : Color.white;
            _stone.ResetPosition();
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

        private void SpawnStone(Player player)
        {
            var go = Instantiate(_stonePrefab, _spawnPosition, Quaternion.identity);
            go.GetComponent<SpriteRenderer>().color =
                player == Player.Black ? Color.black : Color.white;
            _stone = go.AddComponent<DraggableStone>();
            _stone.Init(this, _spawnPosition);
        }
    }
}
