using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class LoadingPanelController : MonoBehaviour
{
    [SerializeField] private Image gaugeImage;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 1;

        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 150;

        if (GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();
    }

    /// <summary>
    /// 로딩바에 게이지를 표시하는 함수
    /// </summary>
    /// <param name="progress">게이지 값</param>
    public void SetProgress(float progress)
    {
        if (gaugeImage != null)
        {
            gaugeImage.fillAmount = progress;
        }
    }

    public void Show(Action onComplete)
    {
        SetProgress(0f);
        _canvasGroup.DOFade(1f, 1.0f)
            .SetUpdate(true)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void Hide(Action onComplete)
    {
        SetProgress(1f);
        _canvasGroup.DOFade(0f, 0.2f)
            .SetUpdate(true)
            .OnComplete(() => onComplete?.Invoke());
    }
}
