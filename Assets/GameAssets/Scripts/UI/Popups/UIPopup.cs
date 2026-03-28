using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasScaler))]
public class UIPopup : MonoBehaviour
{
    [HideInInspector] public Action OnClose = null;

    protected Transform trnPopup;
    protected Image imgBackground;
    protected CanvasScaler _canvasScaler;

    protected UIManager _uiManager;

    public void Construct(UIManager uiManager)
    {
        _uiManager = uiManager;
    }

    protected virtual void Awake()
    {
        trnPopup = transform.Find("Popup");
        trnPopup.localScale = Vector3.zero;

        imgBackground = transform.Find("Background").GetComponent<Image>();
        imgBackground.color = new Color(0f, 0f, 0f, 0.78f);

        _canvasScaler = GetComponent<CanvasScaler>();
        if (_canvasScaler != null)
        {
            if ((float)Screen.height / Screen.width < 1.6f)
            {
                _canvasScaler.matchWidthOrHeight = 0.5f;
            }
        }
    }

    protected virtual void Start()
    {
        imgBackground.DOFade(0.78f, 0.2f);

        trnPopup.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutFlash)
            .OnComplete(() => { trnPopup.DOScale(Vector3.one, 0.1f); });
    }

    public virtual void OnTapClose()
    {
        Close();
    }

    protected virtual void Close()
    {
        imgBackground.DOFade(0f, 0.2f);
        trnPopup.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InFlash).OnComplete(() =>
        {
            OnClose?.Invoke();
            Destroy(gameObject);
        });
    }
}
