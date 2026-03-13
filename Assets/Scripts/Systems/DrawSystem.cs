using Othello.Object;
using Othello.States;
using UnityEngine;

namespace Othello.Systems
{
    public class DrawSystem : MonoBehaviour
    {
        [SerializeField] private GameObject _stonePrefab;
        [SerializeField] private GameObject _highlightPrefab;
        [SerializeField] private float      _cellSize      = 1f;
        [SerializeField] private Vector2    _boardOrigin;
        [SerializeField] private Vector3    _spawnPosition = new Vector3(-5f, 0f, 0f);

        private GameObject[,]  _stones     = new GameObject[8, 8];
        private GameObject[,]  _highlights = new GameObject[8, 8];
        private SpriteRenderer _operationStoneSr;

        public DraggableStone OperationStone { get; private set; }
        public Vector3        SpawnPosition  => _spawnPosition;

        private void Awake()
        {
            var go = Instantiate(_stonePrefab, _spawnPosition, Quaternion.identity);
            _operationStoneSr = go.GetComponent<SpriteRenderer>();
            OperationStone    = go.AddComponent<DraggableStone>();
        }

        public void Render(ViewState view)
        {
            for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
            {
                DrawCell(x, y, view.Board[x, y]);
                DrawHighlight(x, y, view.LegalMoves[x, y]);
            }
        }

        public void Init()
        {
            SpawnOperationStone(Player.Black);
        }

        public void RenderForSwitchTurn(ViewState view)
        {
            SpawnOperationStone(view.CurrentTurn);
        }

        private void SpawnOperationStone(Player player)
        {
            _operationStoneSr.color = player == Player.Black ? Color.black : Color.white;
            OperationStone.ResetPosition();
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
