using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class UIPage : MonoBehaviour
{
    protected UIManager _uiManager;
    [HideInInspector] public Action OnClose;

    protected virtual void Awake()
    {
        var canvasScaler = GetComponent<CanvasScaler>();
        if (canvasScaler != null && (float)Screen.height / Screen.width < 1.6f)
        {
            canvasScaler.matchWidthOrHeight = 1f;
        }
    }

    public void Construct(UIManager uiManager)
    {
        _uiManager = uiManager;
    }

    public void OnTapClose()
    {
        Close();
    }

    public virtual void Close()
    {
        OnClose?.Invoke();
        gameObject.SetActive(false);
    }
}