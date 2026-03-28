using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using TMPro;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    [SerializeField] private SplineComputer _spline;
    [SerializeField] private TMP_Text collectableBoxCountText;
    [SerializeField] private Transform plateStartPosition;

    private int _conveyorLimit;
    private List<CollectableBox> _collectableBoxesOnConveyor = new List<CollectableBox>();
    private ConveyorArrow[] _arrows;
    private List<GameObject> _collectableBoxPlates;
    private GameSettings _gameSettings;

    public SplineComputer SplineComputer => _spline;

    private void Start()
    {
        SetCollectableBoxCountText();
    }

    public void Init(ObjectPool pool, Level level, GameSettings settings)
    {
        _gameSettings = settings;
        this._conveyorLimit = settings.conveyorCollectableBoxLimit;
        SetArrows(pool, level);
        SetPlates(pool, level);
    }

    public bool CanGetNewCollectableBox()
    {
        return _collectableBoxesOnConveyor.Count < _conveyorLimit;
    }

    public void AddCollectableBox(CollectableBox collectableBox)
    {
        if (_collectableBoxesOnConveyor.Contains(collectableBox)) return;

        _collectableBoxesOnConveyor.Add(collectableBox);
        SetCollectableBoxCountText();
    }

    public void RemoveCollectableBox(CollectableBox collectableBox, GameObject plate)
    {
        _collectableBoxesOnConveyor.Remove(collectableBox);
        SetCollectableBoxCountText();

        PutPlate(plate);
    }

    public int GetCurrentCollectableBoxCount()
    {
        return _collectableBoxesOnConveyor.Count;
    }

    private void SetCollectableBoxCountText()
    {
        collectableBoxCountText.text = (_conveyorLimit - _collectableBoxesOnConveyor.Count).ToString() + "/" + _conveyorLimit;
    }

    public void PlayConveyorIsFullEffect()
    {
        collectableBoxCountText.DOComplete();
        collectableBoxCountText.transform.DOComplete();
        collectableBoxCountText.DOColor(_gameSettings.conveyorIsFullEffectTextColor,
            _gameSettings.conveyorIsFullEffectTextColorChangeDuration);
        collectableBoxCountText.transform.DOShakePosition(
            duration: _gameSettings.conveyorIsFullEffectTextShakeDuration,
            strength: _gameSettings.conveyorIsFullEffectTextShakeStrength,
            vibrato: _gameSettings.conveyorIsFullEffectTextShakeVibrato,
            randomness: _gameSettings.conveyorIsFullEffectTextShakeRandomness,
            fadeOut: true
        ).OnComplete(() =>
        {
            collectableBoxCountText.DOColor(Color.white, _gameSettings.conveyorIsFullEffectTextColorFixDuration);
        });
    }

    public void LevelFailed()
    {
        foreach (var collectableBox in _collectableBoxesOnConveyor)
        {
            collectableBox.Stop();
        }

        foreach (var arrow in _arrows)
        {
            arrow.Stop();
        }
    }

    public void SetArrows(ObjectPool pool, Level level)
    {
        _arrows = new ConveyorArrow[_gameSettings.conveyorArrowCount];

        if (_spline == null || _gameSettings.conveyorArrowCount <= 0) return;

        double splineLength = _spline.CalculateLength();
        double stepDistance = splineLength / (_gameSettings.conveyorArrowCount - 1);
        double distance = 0;

        for (int i = 0; i < _gameSettings.conveyorArrowCount; i++)
        {
            var arrowObj = pool.SpawnFromPool(PoolTags.ConveyorArrow, transform.position, Quaternion.identity);
            var arrow = arrowObj.GetComponent<ConveyorArrow>();

            arrow.transform.SetParent(transform, worldPositionStays: true);

            double percent = _spline.Travel(0.0, (float)distance, Spline.Direction.Forward);

            arrow.SetSpline(_spline, percent, _gameSettings.conveyorArrowSpeed);

            distance += stepDistance;

            _arrows[i] = arrow;

            level.SpawnedNewArrows(arrow);
        }
    }

    public void SetPlates(ObjectPool pool, Level level)
    {
        _collectableBoxPlates = new List<GameObject>();
        for (int i = 0; i < _gameSettings.conveyorCollectableBoxLimit; i++)
        {
            var position = plateStartPosition.position -
                           Vector3.right * (_gameSettings.conveyorCollectableBoxPlatePositionDistance * i);
            var plate = pool.SpawnFromPool(PoolTags.ShooterPlate, position, plateStartPosition.rotation);
            _collectableBoxPlates.Add(plate);

            level.SpawnedPlate(plate);
        }
    }

    public GameObject GetPlate()
    {
        var plate = _collectableBoxPlates[0];
        _collectableBoxPlates.RemoveAt(0);
        plate.transform.DOKill();
        FixNextPlatesPosition();
        return plate;
    }

    private void PutPlate(GameObject plate)
    {
        plate.transform.SetParent(transform);
        plate.transform.DOMove(
            plateStartPosition.position - Vector3.right *
            (_gameSettings.conveyorCollectableBoxPlatePositionDistance * _collectableBoxPlates.Count),
            _gameSettings.plateMoveDuration);
        plate.transform.DORotate(plateStartPosition.eulerAngles, _gameSettings.plateMoveDuration);
        _collectableBoxPlates.Add(plate);
    }

    private void FixNextPlatesPosition()
    {
        for (int i = 0; i < _collectableBoxPlates.Count; i++)
        {
            _collectableBoxPlates[i].transform
                .DOMove(
                    plateStartPosition.position -
                    Vector3.right * (_gameSettings.conveyorCollectableBoxPlatePositionDistance * i), 0.1f);
        }
    }
}
