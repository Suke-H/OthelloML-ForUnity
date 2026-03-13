using Othello.States;
using UnityEngine;

namespace Othello.Systems
{
    public class DrawSystem : MonoBehaviour
    {
        [SerializeField] private GameObject _stonePrefab;
        [SerializeField] private GameObject _highlightPrefab;
        [SerializeField] private float      _cellSize = 1f;
        [SerializeField] private Vector2    _boardOrigin;

        private GameObject[,] _stones     = new GameObject[8, 8];
        private GameObject[,] _highlights = new GameObject[8, 8];

        public void Render(ViewState view)
        {
            for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
            {
                DrawCell(x, y, view.Board[x, y]);
                DrawHighlight(x, y, view.LegalMoves[x, y]);
            }
        }

        private void DrawCell(int x, int y, int value)
        {
            if (_stones[x, y] == null)
                _stones[x, y] = Instantiate(_stonePrefab, CellPos(x, y),
                                            Quaternion.identity);

            var sr = _stones[x, y].GetComponent<SpriteRenderer>();
            if (value == 0) { _stones[x, y].SetActive(false); return; }

            _stones[x, y].SetActive(true);
            sr.color = value == 1 ? Color.black : Color.white;
        }

        private void DrawHighlight(int x, int y, bool active)
        {
            if (_highlights[x, y] == null)
                _highlights[x, y] = Instantiate(_highlightPrefab, CellPos(x, y),
                                                 Quaternion.identity);
            _highlights[x, y].SetActive(active);
        }

        private Vector3 CellPos(int x, int y) =>
            new Vector3(_boardOrigin.x + x * _cellSize,
                        _boardOrigin.y - y * _cellSize, 0f);
    }
}
