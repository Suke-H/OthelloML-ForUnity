using UnityEngine;

namespace Othello.Input
{
    public class CLIInputSystem : MonoBehaviour, IInputSystem
    {
        private bool _hasPending;
        private int  _pendingX;
        private int  _pendingY;

        public void SubmitCommand(string input)
        {
            if (TryParse(input, out int x, out int y))
            {
                _hasPending = true;
                _pendingX   = x;
                _pendingY   = y;
            }
        }

        private bool TryParse(string s, out int x, out int y)
        {
            x = y = -1;
            var parts = s.Trim().Split(',');
            if (parts.Length != 2) return false;
            if (!int.TryParse(parts[0].Trim(), out x)) return false;
            if (!int.TryParse(parts[1].Trim(), out y)) return false;
            return x >= 0 && x <= 7 && y >= 0 && y <= 7;
        }

        public void Tick(InputState state)
        {
            state.ActionConfirmed = false;

            if (_hasPending)
            {
                _hasPending           = false;
                state.ActionConfirmed = true;
                state.ActionX         = _pendingX;
                state.ActionY         = _pendingY;
            }
        }
    }
}
