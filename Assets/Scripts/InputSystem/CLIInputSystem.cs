using TMPro;
using UnityEngine;

namespace Othello.InputSystem
{
    public class CLIInputSystem : MonoBehaviour, IInputSystem
    {
        [SerializeField] private TMP_InputField _inputField;

        private bool _hasPending;
        private int  _pendingX;
        private int  _pendingY;

        private void Start()
        {
            if (_inputField == null)
                _inputField = FindFirstObjectByType<TMP_InputField>();

            if (_inputField != null)
                _inputField.onEndEdit.AddListener(SubmitCommand);
            else
                Debug.LogError("[CLIInputSystem] TMP_InputField が見つかりません");
        }

        public void SubmitCommand(string input)
        {
            Debug.Log($"[CLIInput] 入力受信: '{input}'");
            if (TryParse(input, out int x, out int y))
            {
                _hasPending = true;
                _pendingX   = x;
                _pendingY   = y;
                Debug.Log($"[CLIInput] パース成功: ({x},{y})");
            }
            else
            {
                Debug.LogWarning($"[CLIInput] パース失敗: '{input}' → x,y 形式(0-7)で入力");
            }

            if (_inputField != null)
            {
                _inputField.text = "";
                _inputField.ActivateInputField();
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
