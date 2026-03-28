using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dreamteck.Splines;
using TMPro;
using UnityEngine;
using RubyCase.Data;
using RubyCase.Level;
using RubyCase.Pools;

namespace RubyCase.Game
{
    public class CollectableBox : MonoBehaviour, INodeObject, IPoolObject
    {
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private TMP_Text shootCountText;
        [SerializeField] private SplineFollower splineFollower;
        [SerializeField] private GameObject model;

        private CollectableBoxNode _currentCollectableBoxNode;
        public CollectableBoxNode CurrentCollectableBoxNode => _currentCollectableBoxNode;
        private ReservedSlot _reservedSlot;

        private LayerMask _layerColorCube;
        private int colorID;

        private bool _onConveyor;
        private CollectableBoxDirection _currentDirection;
        private GameObject _currentPlate;

        public int ShootCount { get; set; }

        public bool IsSelectable => _currentCollectableBoxNode == null || _currentCollectableBoxNode.IsFrontNode();

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
            _currentCollectableBoxNode = node as CollectableBoxNode;
            UpdateTextOpacity();
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

            splineFollower.followSpeed = _gameSettings.collectableBoxSpeed;

            _layerColorCube = LayerMask.GetMask("ColorCube");

            colorID = data.colorID;
        }

        private void SetShootCountText()
        {
            shootCountText.text = ShootCount.ToString();
        }

        private void UpdateTextOpacity()
        {
            float targetAlpha = IsSelectable ? 1f : 0.5f;
            shootCountText.DOFade(targetAlpha, 0.15f);
        }

        public void Selected(SplineComputer conveyorSpline, CollectableBoxManager collectableBoxManager, GameObject plate)
        {
            if (_reservedSlot != null)
            {
                _reservedSlot.SetEmpty(this);
                _reservedSlot = null;
            }

            _currentCollectableBoxNode = null;

            var position = conveyorSpline.GetPointPosition(0);
            transform.DOKill();
            transform.DOJump(position, _gameSettings.collectableBoxSlotToConveyorJumpPower, 1,
                    _gameSettings.collectableBoxSlotToConveyorJumpDuration)
                .SetEase(_gameSettings.collectableBoxSlotToConveyorJumpEase)
                .OnComplete(() =>
                {
                    plate.transform.SetParent(transform);
                    splineFollower.spline = conveyorSpline;
                    splineFollower.RebuildImmediate();
                    splineFollower.SetPercent(0f);
                    splineFollower.follow = true;
                    splineFollower.enabled = true;
                    model.transform.localEulerAngles = Vector3.up * -90f;
                    StartAbsorbControl(collectableBoxManager);
                });

            _currentPlate = plate;
            plate.transform.DOJump(position, 1f, 1, _gameSettings.collectableBoxSlotToConveyorJumpDuration);
            plate.transform.DORotate(Vector3.zero, _gameSettings.collectableBoxSlotToConveyorJumpDuration);
        }

        private async UniTask StartAbsorbControl(CollectableBoxManager collectableBoxManager)
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
                    var moveDir = splineFollower.result.forward;
                    moveDir.y = 0f;
                    pos += moveDir.normalized * _gameSettings.absorbLeadOffset;
                    var ray = new Ray(pos, GetRayDirection());

                    if (Physics.Raycast(ray, out var hit, 10f, _layerColorCube))
                    {
                        var colorCube = hit.collider.GetComponentInParent<ColorCube>();
                        if (colorCube != null && colorCube.colorID == colorID &&
                            CanBlast(colorCube.CurrentNode.GridPosition))
                        {
                            AddBlastValue(colorCube.CurrentNode.GridPosition);
                            colorCube.GetAbsorbed(transform, _gameSettings.cubeAbsorbDuration,
                                _gameSettings.cubeAbsorbMoveEase, _gameSettings.cubeAbsorbScaleEase, collectableBoxManager);

                            model.transform.DOKill();
                            model.transform.localScale = Vector3.one;
                            model.transform.DOPunchScale(_gameSettings.absorbPunchScale, _gameSettings.absorbPunchDuration,
                                _gameSettings.absorbPunchVibrato, _gameSettings.absorbPunchElasticity);

                            ShootCount--;
                            SetShootCountText();

                            if (ShootCount == 0)
                            {
                                _onConveyor = false;
                                collectableBoxManager.RemoveCollectableBoxFromConveyor(this, _currentPlate);
                                _currentPlate = null;
                                collectableBoxManager.ControlLastCollectableBoxes();
                                PlayShootCompletedEffect();
                                break;
                            }
                        }
                    }

                    if (splineFollower.GetPercent() >= 1f)
                    {
                        if (collectableBoxManager.OnLastCollectableBoxEffect)
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
                            collectableBoxManager.RemoveCollectableBoxFromConveyor(this, _currentPlate);
                            _currentPlate = null;
                            collectableBoxManager.SetReservedSlot(this);
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
            transform.DOJump(reservedSlot.transform.position, _gameSettings.collectableBoxToReservedSlotJumpPower, 1,
                _gameSettings.collectableBoxToReservedSlotJumpDuration).SetEase(_gameSettings.collectableBoxToReservedSlotJumpEase);
            transform.DORotate(reservedSlot.transform.rotation.eulerAngles,
                _gameSettings.collectableBoxToReservedSlotJumpDuration).OnUpdate(() =>
            {
                shootCountText.transform.eulerAngles = _defaultTextEulerAngles;
            });
            model.transform.DOLocalRotate(Vector3.zero, _gameSettings.collectableBoxToReservedSlotJumpDuration);
            _reservedSlot = reservedSlot;
        }

        public void ResetDirection()
        {
            _currentDirection = CollectableBoxDirection.Forward;
            ResetBlastData();
        }

        private Vector3 GetRayDirection()
        {
            switch (_currentDirection)
            {
                case CollectableBoxDirection.Forward: return Vector3.forward;
                case CollectableBoxDirection.Left: return Vector3.left;
                case CollectableBoxDirection.Back: return Vector3.back;
                case CollectableBoxDirection.Right: return Vector3.right;
            }
            return Vector3.forward;
        }

        private bool CanBlast(Vector2Int coordinate)
        {
            if (_currentDirection == CollectableBoxDirection.Forward || _currentDirection == CollectableBoxDirection.Back)
                return !blastedCoordinateValues.Contains(coordinate.x);

            return !blastedCoordinateValues.Contains(coordinate.y);
        }

        private void AddBlastValue(Vector2Int coordinate)
        {
            if (_currentDirection == CollectableBoxDirection.Forward || _currentDirection == CollectableBoxDirection.Back)
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

        public void SetNewNode(CollectableBoxNode to)
        {
            _currentCollectableBoxNode = to;
            transform.DOMove(to.transform.position, _gameSettings.collectableBoxNodeTransferDuration)
                .SetEase(_gameSettings.collectableBoxNodeTransferEase);
            UpdateTextOpacity();
        }

        public void SetDirection(CollectableBoxDirection collectableBoxNextDirection)
        {
            _currentDirection = collectableBoxNextDirection;
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
            transform.DOMoveZ(transform.position.z + _gameSettings.collectableBoxCompleteEffectZPositionPlusValue,
                _gameSettings.collectableBoxCompleteEffectDuration);
            transform.DORotate(Vector3.up * 180f, _gameSettings.collectableBoxCompleteEffectDuration, RotateMode.WorldAxisAdd);
            transform.DOScale(Vector3.zero, _gameSettings.collectableBoxCompleteEffectDuration)
                .SetEase(_gameSettings.collectableBoxCompleteEffectScaleDownEase)
                .OnComplete(() => { gameObject.SetActive(false); });
        }

        public void Reset()
        {
            splineFollower.follow = false;
            splineFollower.enabled = false;
            model.transform.localEulerAngles = Vector3.zero;
            _currentCollectableBoxNode = null;
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

    public enum CollectableBoxDirection
    {
        Forward,
        Left,
        Back,
        Right
    }
}
