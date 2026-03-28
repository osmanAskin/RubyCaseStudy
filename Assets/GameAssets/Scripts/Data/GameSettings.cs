using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using RubyCase.Level;

namespace RubyCase.Data
{
    [CreateAssetMenu(menuName = "Settings/Game Setting", fileName = "New Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [TabGroup("General")] [Header("GAME")] public float levelCompletedUIDelay = 1f;

        [TabGroup("General")] public float levelFailedUIDelay = 2f;

        [Space(15)] [TabGroup("General")] [Header("LEVEL SYSTEM")]
        public int randomStartIndex = 3;

        [Space(15)] [TabGroup("General")] [Header("COLLECTABLE BOX SETTINGS")]
        public float collectableBoxSpeed = 3;

        [TabGroup("General")] public float lastCollectableBoxEffectTimeScale = 2f;
        [Space(10)] [TabGroup("General")] public float collectableBoxSlotToConveyorJumpDuration = 0.5f;
        [TabGroup("General")] public float collectableBoxSlotToConveyorJumpPower = 1f;
        [TabGroup("General")] public Ease collectableBoxSlotToConveyorJumpEase = Ease.Linear;
        [Space(10)] [TabGroup("General")] public float collectableBoxToReservedSlotJumpDuration = 0.5f;
        [TabGroup("General")] public float collectableBoxToReservedSlotJumpPower = 1f;
        [TabGroup("General")] public Ease collectableBoxToReservedSlotJumpEase = Ease.Linear;
        [Space(10)] [TabGroup("General")] public float collectableBoxNodeTransferDuration = 0.3f;
        [TabGroup("General")] public Ease collectableBoxNodeTransferEase = Ease.Linear;
        [Space(10)] [TabGroup("General")] public float collectableBoxCompleteEffectZPositionPlusValue = 1f;
        [TabGroup("General")] public float collectableBoxCompleteEffectDuration = 0.5f;
        [TabGroup("General")] public Ease collectableBoxCompleteEffectScaleDownEase = Ease.InBack;
        [Space(15)] [TabGroup("General")] [Header("ABSORB SETTINGS")]
        public float absorbLeadOffset = 0.5f;
        public float cubeAbsorbDuration = 0.4f;

        [TabGroup("General")] public Ease cubeAbsorbMoveEase = Ease.InBack;
        [TabGroup("General")] public Ease cubeAbsorbScaleEase = Ease.InBack;
        [Space(5)] [TabGroup("General")] public Vector3 absorbPunchScale = new Vector3(0.3f, 0.3f, 0.3f);
        [TabGroup("General")] public float absorbPunchDuration = 0.2f;
        [TabGroup("General")] public int absorbPunchVibrato = 5;
        [TabGroup("General")] public float absorbPunchElasticity = 0.5f;

        [Space(15)] [TabGroup("General")] [Header("CONVEYOR SETTINGS")]
        public int conveyorCollectableBoxLimit = 5;

        [TabGroup("General")] public int conveyorArrowCount;
        [TabGroup("General")] public float conveyorArrowSpeed = 5f;

        [Space(10)] [TabGroup("General")] public Color conveyorIsFullEffectTextColor = Color.red;
        [TabGroup("General")] public float conveyorIsFullEffectTextColorChangeDuration = 0.3f;
        [TabGroup("General")] public float conveyorIsFullEffectTextShakeDuration = 0.35f;
        [TabGroup("General")] public Vector3 conveyorIsFullEffectTextShakeStrength = new Vector3(0.05f, 0.05f, 0f);
        [TabGroup("General")] public int conveyorIsFullEffectTextShakeVibrato = 12;
        [TabGroup("General")] public float conveyorIsFullEffectTextShakeRandomness = 60f;
        [TabGroup("General")] public float conveyorIsFullEffectTextColorFixDuration = 0.2f;
        [Space(10)] [TabGroup("General")] public float conveyorCollectableBoxPlatePositionDistance = 0.1f;
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
}
