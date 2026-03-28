using UnityEngine;

public class ShooterManager : MonoBehaviour
{
    private LevelManager _levelManager;
    private GameSettings _gameSettings;
    private FXManager _fxManager;

    public bool OnLastShooterEffect { get; set; }

    public void Inject(LevelManager levelManager, GameSettings gameSettings, FXManager fxManager)
    {
        _levelManager = levelManager;
        _gameSettings = gameSettings;
        _fxManager = fxManager;
    }

    public void ShooterSelected(Shooter shooter)
    {
        var conveyor = _levelManager.CurrentLevel.Conveyor;
        if (!conveyor.CanGetNewShooter())
        {
            conveyor.PlayConveyorIsFullEffect();
            return;
        }

        _fxManager.PlayShooterSelectedFX();
        conveyor.AddShooter(shooter);

        if (shooter.CurrentShooterNode != null)
        {
            shooter.CurrentShooterNode.SetEmpty(shooter);
            _levelManager.CurrentLevel.ShooterGridSystem.TransferShooters(shooter.CurrentShooterNode.GridPosition.x);
        }

        shooter.Selected(conveyor.SplineComputer, this, conveyor.GetPlate());

        _levelManager.CurrentLevel.ReservedSlotGridSystem.SetWarningEffect();
        _levelManager.CurrentLevel.ReservedSlotGridSystem.TransferShooters();
    }

    public void SetReservedSlot(Shooter shooter)
    {
        var reservedSlot = _levelManager.CurrentLevel.GetAvailableReservedSlot();
        if (reservedSlot == null)
        {
            GameEvents.LevelEndRequested(false);
            return;
        }

        shooter.SetReservedSlot(reservedSlot);
        reservedSlot.AssignNodeObject(shooter);
        _levelManager.CurrentLevel.ReservedSlotGridSystem.SetWarningEffect();
    }

    public void RemoveShooterFromConveyor(Shooter shooter, GameObject plate)
    {
        _levelManager.CurrentLevel.Conveyor.RemoveShooter(shooter, plate);
    }

    public void ColorCubeBlasted()
    {
        if (_levelManager.CurrentLevel.colorCubeGridSystem.IsPictureComplete())
        {
            GameEvents.LevelEndRequested(true);
        }
    }

    public void ControlLastShooters()
    {
        if (OnLastShooterEffect) return;

        var count = _levelManager.CurrentLevel.ShooterGridSystem.GetCurrentShooterCount();
        count += _levelManager.CurrentLevel.ReservedSlotGridSystem.GetCurrentShooterCount();
        count += _levelManager.CurrentLevel.Conveyor.GetCurrentShooterCount();

        if (count <= _gameSettings.conveyorShooterLimit)
        {
            OnLastShooterEffect = true;
            Time.timeScale = _gameSettings.lastShooterEffectTimeScale;
        }
    }

    public void ResetSystem()
    {
        OnLastShooterEffect = false;
    }
}
