using UnityEngine;
using RubyCase.Event;
using RubyCase.Level;
using RubyCase.Managers;

namespace RubyCase.Game
{
    public class GameInputController : MonoBehaviour, IInputListener
    {
        private InputManager _inputManager;
        private CollectableBoxManager _collectableBoxManager;

        private CollectableBox _selectedCollectableBox;
        private Camera _mainCamera;
        private LayerMask _layerCollectableBox;

        public void Inject(InputManager inputManager, CollectableBoxManager collectableBoxManager)
        {
            _inputManager = inputManager;
            _collectableBoxManager = collectableBoxManager;
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
            _layerCollectableBox = LayerMask.GetMask("CollectableBox");
        }

        private void OnEnable()
        {
            GameEvents.OnLevelStart += OnLevelStart;
            GameEvents.OnLevelEnd += OnLevelEnd;
        }

        private void OnDisable()
        {
            GameEvents.OnLevelStart -= OnLevelStart;
            GameEvents.OnLevelEnd -= OnLevelEnd;
        }

        private void OnDestroy()
        {
            _inputManager.RemoveListener(this);
        }

        private void OnLevelStart(int levelIndex, Level.Level level)
        {
            _inputManager.AddListener(this);
        }

        private void OnLevelEnd(bool isWin, int levelIndex)
        {
            _inputManager.RemoveListener(this);
        }

        public void OnPressed()
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerCollectableBox))
            {
                var collectableBox = hit.collider.gameObject.GetComponentInParent<CollectableBox>();
                if (collectableBox == null)
                {
                    return;
                }
                if (!collectableBox.IsSelectable)
                {
                    return;
                }
                _selectedCollectableBox = collectableBox;
            }
        }

        public void OnDrag(Vector2 dragVector)
        {
        }

        public void OnReleased(Vector2 dragVector)
        {
            if (_selectedCollectableBox == null)
            {
                return;
            }

            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerCollectableBox))
            {
                var collectableBox = hit.collider.gameObject.GetComponentInParent<CollectableBox>();
                if (collectableBox != null || collectableBox == _selectedCollectableBox)
                {
                    _collectableBoxManager.CollectableBoxSelected(collectableBox);
                }
            }

            _selectedCollectableBox = null;
        }
    }
}
