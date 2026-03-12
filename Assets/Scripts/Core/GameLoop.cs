using Othello.Draw;
using Othello.Input;
using Othello.Update;
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
            _input      = _inputMode == InputMode.GUI
                            ? _guiInput : (IInputSystem)_cliInput;
            _envState   = EnvState.CreateInitial();
            _inputState = new InputState();

            var legal = UpdateSystem.GetLegalMoves(_envState);
            Debug.Log($"[GameLoop] 開始。手番={_envState.CurrentTurn}、合法手={LegalMovesToString(legal)}");
        }

        private void Update()
        {
            _input.Tick(_inputState);

            if (_inputState.ActionConfirmed)
            {
                Debug.Log($"[GameLoop] アクション ({_inputState.ActionX},{_inputState.ActionY})");
                var next = UpdateSystem.Apply(_envState, _inputState.ActionX, _inputState.ActionY);
                if (next != null)
                {
                    _envState = next;
                    var legal = UpdateSystem.GetLegalMoves(_envState);
                    Debug.Log($"[GameLoop] 着手成功。手番={_envState.CurrentTurn}、合法手={LegalMovesToString(legal)}");
                }
                else
                {
                    Debug.LogWarning($"[GameLoop] ({_inputState.ActionX},{_inputState.ActionY}) は非合法手です");
                }
            }

            var legalMoves = UpdateSystem.GetLegalMoves(_envState);
            var viewState  = new ViewState(_envState, legalMoves, _inputState);
            _draw.Render(viewState);
        }

        private string LegalMovesToString(bool[,] legal)
        {
            var sb = new System.Text.StringBuilder();
            for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                if (legal[x, y]) sb.Append($"{x},{y} ");
            return sb.ToString().Trim();
        }
    }
}
