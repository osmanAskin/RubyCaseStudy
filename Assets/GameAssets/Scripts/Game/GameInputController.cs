using UnityEngine;

public class GameInputController : MonoBehaviour, IInputListener
{
    private InputManager _inputManager;
    private ShooterManager _shooterManager;

    private Shooter _selectedShooter;
    private Camera _mainCamera;
    private LayerMask _layerShooter;

    public void Inject(InputManager inputManager, ShooterManager shooterManager)
    {
        _inputManager = inputManager;
        _shooterManager = shooterManager;
    }

    private void Awake()
    {
        _mainCamera = Camera.main;
        _layerShooter = LayerMask.GetMask("Shooter");
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

    private void OnLevelStart(int levelIndex, Level level)
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
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerShooter))
        {
            var shooter = hit.collider.gameObject.GetComponentInParent<Shooter>();
            if (shooter == null)
            {
                return;
            }
            if (!shooter.IsSelectable)
            {
                return;
            }
            _selectedShooter = shooter;
        }
    }

    public void OnDrag(Vector2 dragVector)
    {
    }

    public void OnReleased(Vector2 dragVector)
    {
        if (_selectedShooter == null)
        {
            return;
        }

        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerShooter))
        {
            var shooter = hit.collider.gameObject.GetComponentInParent<Shooter>();
            if (shooter != null || shooter == _selectedShooter)
            {
                _shooterManager.ShooterSelected(shooter);
            }
        }

        _selectedShooter = null;
    }
}
