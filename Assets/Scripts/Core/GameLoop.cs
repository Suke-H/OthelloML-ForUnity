using Othello.States;
using Othello.Systems;
using UnityEngine;

namespace Othello.Core
{
    public class GameLoop : MonoBehaviour
    {
        public enum InputMode { GUI, CLI }

        [SerializeField] private InputMode      _inputMode = InputMode.GUI;
        [SerializeField] private GUIInputSystem _guiInput;
        [SerializeField] private CLIInputSystem _cliInput;
        [SerializeField] private DrawSystem     _draw;

        private EnvState     _envState;
        private InputState   _inputState;
        private IInputSystem _input;

        private void Start()
        {
            _input      = _inputMode == InputMode.GUI ? _guiInput : _cliInput;
            _envState   = EnvSystem.CreateInitial();
            _inputState = new InputState();

            if (_inputMode == InputMode.GUI)
            {
                _guiInput.Init(_draw.OperationStone);
                _draw.OperationStone.Init(_guiInput, _draw.SpawnPosition);
                _draw.Init();
            }
            else
            {
                _draw.OperationStone.gameObject.SetActive(false);
            }

            
            Debug.Log($"[GameLoop] 開始。手番={_envState.CurrentTurn}、合法手={LegalMovesToString(_envState.LegalMoves)}");
        }

        private void Update()
        {
            _input.Tick(_inputState);

            if (_inputState.ActionConfirmed)
            {
                Debug.Log($"[GameLoop] アクション ({_inputState.ActionX},{_inputState.ActionY})");
                var nextEnvState = EnvSystem.Apply(_envState, _inputState.ActionX, _inputState.ActionY);
                _envState = nextEnvState;
                if (nextEnvState.IsActionSuccess)
                {
                    Debug.Log($"[GameLoop] 着手成功。手番={_envState.CurrentTurn}、合法手={LegalMovesToString(_envState.LegalMoves)}");
                    var switchViewState = UpdateSystem.CreateViewState(_envState, _inputState);
                    _draw.RenderForSwitchTurn(switchViewState);
                }
                else
                {
                    Debug.LogWarning($"[GameLoop] ({_inputState.ActionX},{_inputState.ActionY}) は非合法手です");
                }
            }

            var viewState = UpdateSystem.CreateViewState(_envState, _inputState);
            _draw.Render(viewState);
        }

        private string LegalMovesToString(bool[,] legal)
        {
            var sb = new global::System.Text.StringBuilder();
            for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                if (legal[x, y]) sb.Append($"{x},{y} ");
            return sb.ToString().Trim();
        }
    }
}
