using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dreamteck.Splines;
using TMPro;
using UnityEngine;

public class Shooter : MonoBehaviour, INodeObject, IPoolObject
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private TMP_Text shootCountText;
    [SerializeField] private SplineFollower splineFollower;
    [SerializeField] private GameObject model;

    private ShooterNode _currentShooterNode;
    public ShooterNode CurrentShooterNode => _currentShooterNode;
    private ReservedSlot _reservedSlot;

    private LayerMask _layerColorCube;
    private int colorID;

    private bool _onConveyor;
    private ShooterDirection _currentDirection;
    private GameObject _currentPlate;

    public int ShootCount { get; set; }

    public bool IsSelectable => _currentShooterNode == null || _currentShooterNode.IsFrontNode();

    private CancellationTokenSource _shootCts;

    private System.Collections.Generic.List<int> blastedCoordinateValues = new System.Collections.Generic.List<int>();

    private static MaterialPropertyBlock _mpb;
    private Vector3 _defaultTextEulerAngles;

    private GameSettings _gameSettings;

    private void Start()
    {
        _defaultTextEulerAngles = shootCountText.transform.eulerAngles;
    }

    public void Initialize(Node node)
    {
        _currentShooterNode = node as ShooterNode;
    }

    public void Init(CellData data, GameSettings settings)
    {
        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();

        foreach (var renderer in renderers)
        {
            renderer.GetPropertyBlock(_mpb);
            _mpb.SetColor("_BaseColor", data.cellColor.linear);
            renderer.SetPropertyBlock(_mpb);
        }

        _gameSettings = settings;
        ShootCount = data.shootCount;
        SetShootCountText();

        splineFollower.followSpeed = _gameSettings.shooterSpeed;

        _layerColorCube = LayerMask.GetMask("ColorCube");

        colorID = data.colorID;
    }

    private void SetShootCountText()
    {
        shootCountText.text = ShootCount.ToString();
    }

    public void Selected(SplineComputer conveyorSpline, ShooterManager shooterManager, GameObject plate)
    {
        if (_reservedSlot != null)
        {
            _reservedSlot.SetEmpty(this);
            _reservedSlot = null;
        }

        _currentShooterNode = null;

        var position = conveyorSpline.GetPointPosition(0);
        transform.DOKill();
        transform.DOJump(position, _gameSettings.shooterSlotToConveyorJumpPower, 1,
                _gameSettings.shooterSlotToConveyorJumpDuration)
            .SetEase(_gameSettings.shooterSlotToConveyorJumpEase)
            .OnComplete(() =>
            {
                plate.transform.SetParent(transform);
                splineFollower.spline = conveyorSpline;
                splineFollower.RebuildImmediate();
                splineFollower.SetPercent(0f);
                splineFollower.follow = true;
                splineFollower.enabled = true;
                model.transform.localEulerAngles = Vector3.up * -90f;
                StartAbsorbControl(shooterManager);
            });

        _currentPlate = plate;
        plate.transform.DOJump(position, 1f, 1, _gameSettings.shooterSlotToConveyorJumpDuration);
        plate.transform.DORotate(Vector3.zero, _gameSettings.shooterSlotToConveyorJumpDuration);
    }

    private async UniTask StartAbsorbControl(ShooterManager shooterManager)
    {
        _shootCts?.Cancel();
        _shootCts = new CancellationTokenSource();
        ResetDirection();
        _onConveyor = true;
        try
        {
            while (_onConveyor && !_shootCts.IsCancellationRequested && gameObject.activeSelf)
            {
                shootCountText.transform.eulerAngles = _defaultTextEulerAngles;
                var pos = transform.position;
                pos.y = 0.282f;
                var ray = new Ray(pos, GetRayDirection());

                if (Physics.Raycast(ray, out var hit, 10f, _layerColorCube))
                {
                    var colorCube = hit.collider.GetComponentInParent<ColorCube>();
                    if (colorCube != null && colorCube.colorID == colorID &&
                        CanBlast(colorCube.CurrentNode.GridPosition))
                    {
                        AddBlastValue(colorCube.CurrentNode.GridPosition);
                        colorCube.GetAbsorbed(transform, _gameSettings.cubeAbsorbDuration,
                            _gameSettings.cubeAbsorbMoveEase, _gameSettings.cubeAbsorbScaleEase, shooterManager);

                        model.transform.DOKill();
                        model.transform.localScale = Vector3.one;
                        model.transform.DOPunchScale(_gameSettings.absorbPunchScale, _gameSettings.absorbPunchDuration,
                            _gameSettings.absorbPunchVibrato, _gameSettings.absorbPunchElasticity);

                        ShootCount--;
                        SetShootCountText();

                        if (ShootCount == 0)
                        {
                            _onConveyor = false;
                            shooterManager.RemoveShooterFromConveyor(this, _currentPlate);
                            _currentPlate = null;
                            shooterManager.ControlLastShooters();
                            PlayShootCompletedEffect();
                            break;
                        }
                    }
                }

                if (splineFollower.GetPercent() >= 1f)
                {
                    if (shooterManager.OnLastShooterEffect)
                    {
                        splineFollower.SetPercent(0);
                        splineFollower.Rebuild();
                        ResetDirection();
                    }
                    else
                    {
                        _onConveyor = false;
                        splineFollower.follow = false;
                        splineFollower.enabled = false;
                        shooterManager.RemoveShooterFromConveyor(this, _currentPlate);
                        _currentPlate = null;
                        shooterManager.SetReservedSlot(this);
                    }
                }

                await UniTask.Yield(PlayerLoopTiming.Update, _shootCts.Token);
            }
        }
        catch (System.OperationCanceledException)
        {
            // normal iptal senaryosu
        }
        finally
        {
            _onConveyor = false;
            _shootCts.Dispose();
            _shootCts = null;
        }
    }

    public void SetReservedSlot(ReservedSlot reservedSlot)
    {
        transform.DOKill();
        transform.DOJump(reservedSlot.transform.position, _gameSettings.shooterToReservedSlotJumpPower, 1,
            _gameSettings.shooterToReservedSlotJumpDuration).SetEase(_gameSettings.shooterToReservedSlotJumpEase);
        transform.DORotate(reservedSlot.transform.rotation.eulerAngles,
            _gameSettings.shooterToReservedSlotJumpDuration).OnUpdate(() =>
        {
            shootCountText.transform.eulerAngles = _defaultTextEulerAngles;
        });
        model.transform.DOLocalRotate(Vector3.zero, _gameSettings.shooterToReservedSlotJumpDuration);
        _reservedSlot = reservedSlot;
    }

    public void ResetDirection()
    {
        _currentDirection = ShooterDirection.Forward;
        ResetBlastData();
    }

    private Vector3 GetRayDirection()
    {
        switch (_currentDirection)
        {
            case ShooterDirection.Forward: return Vector3.forward;
            case ShooterDirection.Left: return Vector3.left;
            case ShooterDirection.Back: return Vector3.back;
            case ShooterDirection.Right: return Vector3.right;
        }
        return Vector3.forward;
    }

    private bool CanBlast(Vector2Int coordinate)
    {
        if (_currentDirection == ShooterDirection.Forward || _currentDirection == ShooterDirection.Back)
            return !blastedCoordinateValues.Contains(coordinate.x);

        return !blastedCoordinateValues.Contains(coordinate.y);
    }

    private void AddBlastValue(Vector2Int coordinate)
    {
        if (_currentDirection == ShooterDirection.Forward || _currentDirection == ShooterDirection.Back)
        {
            blastedCoordinateValues.Add(coordinate.x);
            return;
        }
        blastedCoordinateValues.Add(coordinate.y);
    }

    private void ResetBlastData()
    {
        blastedCoordinateValues.Clear();
    }

    public void SetNewNode(ShooterNode to)
    {
        _currentShooterNode = to;
        transform.DOMove(to.transform.position, _gameSettings.shooterNodeTransferDuration)
            .SetEase(_gameSettings.shooterNodeTransferEase);
    }

    public void SetDirection(ShooterDirection shooterNextDirection)
    {
        _currentDirection = shooterNextDirection;
        ResetBlastData();
    }

    public void SetSpeed(float speed)
    {
        splineFollower.followSpeed = speed;
    }

    public void Stop()
    {
        _shootCts?.Cancel();
        splineFollower.follow = false;
        transform.DOKill();
    }

    private void PlayShootCompletedEffect()
    {
        splineFollower.follow = false;
        transform.DOKill();
        transform.DOMoveZ(transform.position.z + _gameSettings.shooterCompleteEffectZPositionPlusValue,
            _gameSettings.shooterCompleteEffectDuration);
        transform.DORotate(Vector3.up * 180f, _gameSettings.shooterCompleteEffectDuration, RotateMode.WorldAxisAdd);
        transform.DOScale(Vector3.zero, _gameSettings.shooterCompleteEffectDuration)
            .SetEase(_gameSettings.shooterCompleteEffectScaleDownEase)
            .OnComplete(() => { gameObject.SetActive(false); });
    }

    public void Reset()
    {
        splineFollower.follow = false;
        splineFollower.enabled = false;
        model.transform.localEulerAngles = Vector3.zero;
        _currentShooterNode = null;
        _reservedSlot = null;
        colorID = -1;
        _onConveyor = false;
        ResetDirection();
        ShootCount = 0;
        _shootCts?.Cancel();
        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.zero;
        shootCountText.transform.eulerAngles = _defaultTextEulerAngles;
    }
}

public enum ShooterDirection
{
    Forward,
    Left,
    Back,
    Right
}
