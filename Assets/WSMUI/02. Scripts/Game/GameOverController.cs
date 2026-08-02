using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class GameOverController : BasePopupUI
{
    [SerializeField] private GameObject bloodSplatterEffect;
    [SerializeField] private GameObject buttonGroup;

    private Image bloodImage;

    private void Awake()
    {
        popupPanel = this.gameObject;

        if (bloodSplatterEffect != null)
        {
            bloodImage = bloodSplatterEffect.GetComponent<Image>();
        }
    }

    public void Open()
    {
        ShowPanel();

        PlayGameOverSequence();
    }

    private void OnEnable()
    {
        if (buttonGroup != null) buttonGroup.SetActive(false);
    }

    private void PlayGameOverSequence()
    {
        if (bloodSplatterEffect == null) return;

        // [초기 상태] 피 이미지를 절반 크기(0.5)와 투명한 상태로 시작
        bloodSplatterEffect.transform.localScale = Vector3.one * 0.5f;
        if (bloodImage != null) bloodImage.color = new Color(1, 1, 1, 0);

        Sequence mainSeq = DOTween.Sequence().SetUpdate(true);

        // 1. 피가 천천히 번지는 연출 (크기가 커짐과 동시에 서서히 진해짐)
        // SetEase(Ease.OutCubic)을 사용하여 처음엔 확 퍼지다가 끝에서 끈적하게 느려지는 느낌을 줍니다.
        mainSeq.Append(bloodSplatterEffect.transform.DOScale(1.1f, 1.0f).SetEase(Ease.OutCubic).SetUpdate(true));

        if (bloodImage != null)
        {
            mainSeq.Join(bloodImage.DOFade(1f, 1f).SetEase(Ease.OutCubic).SetUpdate(true));
        }

        mainSeq.AppendInterval(0.2f);

        mainSeq.OnComplete(() =>
        {
            if (buttonGroup != null)
            {
                buttonGroup.SetActive(true);
                buttonGroup.transform.localScale = Vector3.one; // 크기는 정상 크기 유지

                CanvasGroup cg = buttonGroup.GetComponent<CanvasGroup>();
                if (cg == null) cg = buttonGroup.AddComponent<CanvasGroup>();

                cg.alpha = 0f;
                cg.DOFade(1f, 0.5f).SetUpdate(true);
            }
        });
    }

    private void ReopenAfterLoadCancel()
    {
        ShowPanel();

        if (bloodSplatterEffect != null)
        {
            bloodSplatterEffect.transform.localScale = Vector3.one * 1.1f;
            if (bloodImage != null) bloodImage.color = new Color(1, 1, 1, 1);
        }

        if (buttonGroup != null)
        {
            buttonGroup.SetActive(true);
            buttonGroup.transform.localScale = Vector3.one;

            CanvasGroup cg = buttonGroup.GetComponent<CanvasGroup>();
            if (cg == null) cg = buttonGroup.AddComponent<CanvasGroup>();
            cg.alpha = 1f;
        }
    }

    public void OnLoadClick()
    {
        if (UIManager.Instance != null && UIManager.Instance.saveLoadController != null)
        {
            HidePanel();
            UIManager.Instance.saveLoadController.onCloseAction = ReopenAfterLoadCancel;
            UIManager.Instance.saveLoadController.Open(Constants.ESaveLoadType.Load, false);
        }
        else
        {
            Debug.LogError("GameOverController: UIManager 또는 SaveLoadController 참조가 누락되었습니다.");
            ShowPanel();
        }
    }

    public void OnMainMenuClick()
    {
        if (buttonGroup != null) buttonGroup.SetActive(false);

        Time.timeScale = 1f;

        if (UIManager.Instance != null)
        {
            // 1. 씬 로드 호출 (0.5초 동안 서서히 페이드 아웃 됨)
            UIManager.Instance.LoadScene(Constants.ESceneType.PrototypeMain, true);

            // 2. 화면이 완전히 까매지는 타이밍(0.5초 뒤)에 맞춰 패널을 숨기고 스택에서 제거
            DOVirtual.DelayedCall(0.5f, () =>
            {
                HidePanel();
            }).SetUpdate(true); // 타임스케일 영향을 받지 않도록 안전장치 추가

            return;
        }

        HidePanel();
        SceneManager.LoadScene(Constants.ESceneType.PrototypeMain.ToString());
    }
}