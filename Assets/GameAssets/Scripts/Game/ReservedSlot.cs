using System;
using DG.Tweening;
using UnityEngine;

public class ReservedSlot : Node
{
    [SerializeField] private SpriteRenderer warningEffect;
    
    private float _warningEffectAlphaValue;
    private float _warningEffectDuration;
    private int _warningEffectCount;

    private void Start()
    {
        _warningEffectAlphaValue = warningEffect.color.a;
        warningEffect.DOFade(0f, 0f);
    }

    public void ActivateWarningEffect(bool isActive)
    {
        warningEffect.gameObject.SetActive(isActive);

        if (isActive)
        {
            warningEffect.DOFade(_warningEffectAlphaValue, _warningEffectDuration)
                .SetLoops(_warningEffectCount * 2, LoopType.Yoyo).SetEase(Ease.Linear);
        }
        else
        {
            warningEffect.DOKill();
            warningEffect.DOFade(0f, 0f);
        }
    }

    public void SetEffectValues(float duration, int count)
    {
        _warningEffectDuration = duration;
        _warningEffectCount = count;
    }
}
