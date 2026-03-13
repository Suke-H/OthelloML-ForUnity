using Othello.Systems;
using UnityEngine;

namespace Othello.Object
{
    public class DraggableStone : MonoBehaviour
    {
        private GUIInputSystem _gui;
        private Vector3        _spawnPosition;
        private Vector3        _offset;
        private Camera         _mainCamera;

        public void Init(GUIInputSystem gui, Vector3 spawnPosition)
        {
            _gui           = gui;
            _spawnPosition = spawnPosition;
            _mainCamera    = Camera.main;
        }

        private void OnMouseDown()
        {
            _offset = transform.position - GetMouseWorldPos();
        }

        private void OnMouseDrag()
        {
            transform.position = GetMouseWorldPos() + _offset;
        }

        private void OnMouseUp()
        {
            _gui.OnStoneDropped(transform.position);
        }

        public void ResetPosition()
        {
            transform.position = _spawnPosition;
        }

        private Vector3 GetMouseWorldPos()
        {
            var pos = Input.mousePosition;
            pos.z = 10f;
            return _mainCamera.ScreenToWorldPoint(pos);
        }
    }
}
