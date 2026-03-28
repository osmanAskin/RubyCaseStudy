using UnityEngine;

public class CollectableBoxManager : MonoBehaviour
{
    private LevelManager _levelManager;
    private GameSettings _gameSettings;

    public bool OnLastCollectableBoxEffect { get; set; }

    public void Inject(LevelManager levelManager, GameSettings gameSettings)
    {
        _levelManager = levelManager;
        _gameSettings = gameSettings;
    }

    public void CollectableBoxSelected(CollectableBox collectableBox)
    {
        var conveyor = _levelManager.CurrentLevel.Conveyor;
        if (!conveyor.CanGetNewCollectableBox())
        {
            conveyor.PlayConveyorIsFullEffect();
            return;
        }

        conveyor.AddCollectableBox(collectableBox);

        if (collectableBox.CurrentCollectableBoxNode != null)
        {
            collectableBox.CurrentCollectableBoxNode.SetEmpty(collectableBox);
            _levelManager.CurrentLevel.CollectableBoxGridSystem.TransferCollectableBoxes(collectableBox.CurrentCollectableBoxNode.GridPosition.x);
        }

        collectableBox.Selected(conveyor.SplineComputer, this, conveyor.GetPlate());

        _levelManager.CurrentLevel.ReservedSlotGridSystem.SetWarningEffect();
        _levelManager.CurrentLevel.ReservedSlotGridSystem.TransferCollectableBoxes();
    }

    public void SetReservedSlot(CollectableBox collectableBox)
    {
        var reservedSlot = _levelManager.CurrentLevel.GetAvailableReservedSlot();
        if (reservedSlot == null)
        {
            GameEvents.LevelEndRequested(false);
            return;
        }

        collectableBox.SetReservedSlot(reservedSlot);
        reservedSlot.AssignNodeObject(collectableBox);
        _levelManager.CurrentLevel.ReservedSlotGridSystem.SetWarningEffect();
    }

    public void RemoveCollectableBoxFromConveyor(CollectableBox collectableBox, GameObject plate)
    {
        _levelManager.CurrentLevel.Conveyor.RemoveCollectableBox(collectableBox, plate);
    }

    public void ColorCubeBlasted()
    {
        if (_levelManager.CurrentLevel.colorCubeGridSystem.IsPictureComplete())
        {
            GameEvents.LevelEndRequested(true);
        }
    }

    public void ControlLastCollectableBoxes()
    {
        if (OnLastCollectableBoxEffect) return;

        var count = _levelManager.CurrentLevel.CollectableBoxGridSystem.GetCurrentCollectableBoxCount();
        count += _levelManager.CurrentLevel.ReservedSlotGridSystem.GetCurrentCollectableBoxCount();
        count += _levelManager.CurrentLevel.Conveyor.GetCurrentCollectableBoxCount();

        if (count <= _gameSettings.conveyorCollectableBoxLimit)
        {
            OnLastCollectableBoxEffect = true;
            Time.timeScale = _gameSettings.lastCollectableBoxEffectTimeScale;
        }
    }

    public void ResetSystem()
    {
        OnLastCollectableBoxEffect = false;
    }
}
