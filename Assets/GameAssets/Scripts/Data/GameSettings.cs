using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Game Setting", fileName = "New Game Settings")]
public class GameSettings : ScriptableObject
{
    [TabGroup("General")] [Header("GAME")] public float levelCompletedUIDelay = 1f;

    [TabGroup("General")] public float levelFailedUIDelay = 2f;

    [Space(15)] [TabGroup("General")] [Header("LEVEL SYSTEM")]
    public int randomStartIndex = 3;

    [Space(15)] [TabGroup("General")] [Header("SHOOTER SETTINGS")]
    public float shooterSpeed = 3;

    [TabGroup("General")] public float lastShooterEffectTimeScale = 2f;
    [Space(10)] [TabGroup("General")] public float shooterSlotToConveyorJumpDuration = 0.5f;
    [TabGroup("General")] public float shooterSlotToConveyorJumpPower = 1f;
    [TabGroup("General")] public Ease shooterSlotToConveyorJumpEase = Ease.Linear;
    [Space(10)] [TabGroup("General")] public float shooterToReservedSlotJumpDuration = 0.5f;
    [TabGroup("General")] public float shooterToReservedSlotJumpPower = 1f;
    [TabGroup("General")] public Ease shooterToReservedSlotJumpEase = Ease.Linear;
    [Space(10)] [TabGroup("General")] public float shooterNodeTransferDuration = 0.3f;
    [TabGroup("General")] public Ease shooterNodeTransferEase = Ease.Linear;
    [Space(10)] [TabGroup("General")] public float shooterCompleteEffectZPositionPlusValue = 1f;
    [TabGroup("General")] public float shooterCompleteEffectDuration = 0.5f;
    [TabGroup("General")] public Ease shooterCompleteEffectScaleDownEase = Ease.InBack;
    [Space(15)] [TabGroup("General")] [Header("ABSORB SETTINGS")]
    public float cubeAbsorbDuration = 0.4f;

    [TabGroup("General")] public Ease cubeAbsorbMoveEase = Ease.InBack;
    [TabGroup("General")] public Ease cubeAbsorbScaleEase = Ease.InBack;
    [Space(5)] [TabGroup("General")] public Vector3 absorbPunchScale = new Vector3(0.3f, 0.3f, 0.3f);
    [TabGroup("General")] public float absorbPunchDuration = 0.2f;
    [TabGroup("General")] public int absorbPunchVibrato = 5;
    [TabGroup("General")] public float absorbPunchElasticity = 0.5f;

    [Space(15)] [TabGroup("General")] [Header("CONVEYOR SETTINGS")]
    public int conveyorShooterLimit = 5;

    [TabGroup("General")] public int conveyorArrowCount;
    [TabGroup("General")] public float conveyorArrowSpeed = 5f;

    [Space(10)] [TabGroup("General")] public Color conveyorIsFullEffectTextColor = Color.red;
    [TabGroup("General")] public float conveyorIsFullEffectTextColorChangeDuration = 0.3f;
    [TabGroup("General")] public float conveyorIsFullEffectTextShakeDuration = 0.35f;
    [TabGroup("General")] public Vector3 conveyorIsFullEffectTextShakeStrength = new Vector3(0.05f, 0.05f, 0f);
    [TabGroup("General")] public int conveyorIsFullEffectTextShakeVibrato = 12;
    [TabGroup("General")] public float conveyorIsFullEffectTextShakeRandomness = 60f;
    [TabGroup("General")] public float conveyorIsFullEffectTextColorFixDuration = 0.2f;
    [Space(10)] [TabGroup("General")] public float conveyorShooterPlatePositionDistance = 0.1f;
    [TabGroup("General")] public float plateMoveDuration = 0.3f;

    [Space(15)] [TabGroup("General")] [Header("RESERVED SLOT SETTINGS")]
    public int reservedSlotCount = 5;

    [TabGroup("General")] public int reservedSlotWarningEffectCount = 2;
    [TabGroup("General")] public float reservedSlotWarningEffectDuration = 0.5f;

    [Space(15)] [TabGroup("General")] [Header("COLOR CUBE & TEXTURE SETTİNGS")]
    public float defaultTextureWidth;

    [TabGroup("General")] public float defaultTextureHeight;

    [TabGroup("Levels")] [ListDrawerSettings(Expanded = true, DraggableItems = false, ShowIndexLabels = true)]
    public LevelData[] levels;

    public LevelData GetLevel(int index)
    {
        if (index >= 0 && index < levels.Length)
            return levels[index];

        if (randomStartIndex < 0 || randomStartIndex >= levels.Length)
        {
            Debug.LogWarning($"[GameSettings] RandomStartIndex ({randomStartIndex}) is invalid, defaulting to 0.");
            randomStartIndex = 0;
        }

        var randomIndex = Random.Range(randomStartIndex, levels.Length);
        Debug.Log($"[GameSettings] Level {index} -> randomly selected: {randomIndex}");
        return levels[randomIndex];
    }
}
