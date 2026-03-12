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
        }

        private void Update()
        {
            _input.Tick(_inputState);

            if (_inputState.ActionConfirmed)
            {
                var next = UpdateSystem.Apply(_envState,
                               _inputState.ActionX, _inputState.ActionY);
                if (next != null) _envState = next;
            }

            var legal     = UpdateSystem.GetLegalMoves(_envState);
            var viewState = new ViewState(_envState, legal, _inputState);
            _draw.Render(viewState);
        }
    }
}
