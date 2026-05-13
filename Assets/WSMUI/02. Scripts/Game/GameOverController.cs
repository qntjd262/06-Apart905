using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

// 1. BasePopupUI 상속
public class GameOverController : BasePopupUI
{
    [SerializeField] private GameObject bloodSplatterEffect;
    [SerializeField] private GameObject buttonGroup;

    private Image bloodImage;

    private void Awake()
    {
        popupPanel = this.gameObject; // 부모 변수 연결

        if (UIManager.Instance != null)
        {
            UIManager.Instance.gameOverPanel = this.gameObject;
        }

        if (bloodSplatterEffect != null)
        {
            bloodImage = bloodSplatterEffect.GetComponent<Image>();
        }
    }

    // 2. 외부에서 게임 오버 창을 켤 때 호출할 함수
    public void Open()
    {
        ShowPanel(); // 스택 등록 및 SetActive(true) 자동 실행
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
        if (buttonGroup != null) buttonGroup.SetActive(false);
        PlayGameOverSequence();
    }

    private void PlayGameOverSequence()
    {
        if (bloodSplatterEffect == null) return;

        bloodSplatterEffect.transform.localScale = Vector3.zero;
        if (bloodImage != null) bloodImage.color = new Color(1, 1, 1, 0);

        Sequence mainSeq = DOTween.Sequence();

        mainSeq.Append(bloodSplatterEffect.transform.DOScale(1.2f, 0.15f).SetEase(Ease.OutExpo));
        if (bloodImage != null)
        {
            mainSeq.Join(bloodImage.DOFade(1f, 0.1f));
        }
        mainSeq.Append(bloodSplatterEffect.transform.DOScale(1.0f, 0.1f));

        mainSeq.AppendInterval(0.1f);

        mainSeq.OnComplete(() =>
        {
            if (buttonGroup != null)
            {
                buttonGroup.SetActive(true);
                buttonGroup.transform.localScale = Vector3.zero;
                buttonGroup.transform.DOScale(1.0f, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true);
            }
        });

        mainSeq.SetUpdate(true);
    }

    public void OnLoadClick()
    {
        SaveLoadController saveLoader = FindFirstObjectByType<SaveLoadController>(FindObjectsInactive.Include);
        if (saveLoader != null)
        {
            HidePanel(); // 게임 오버 창 닫기

            saveLoader.onCloseAction = () =>
            {
                this.ShowPanel(); // 불러오기 취소/완료 시 다시 게임 오버 창으로 복귀
            };

            saveLoader.Open(Constants.ESaveLoadType.Load);
        }
    }

    public void OnMainMenuClick()
    {
        // 4. 메인 메뉴로 갈 때는 스택에서 확실히 해제
        HidePanel();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.LoadScene(Constants.ESceneType.PrototypeMain);
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(Constants.ESceneType.PrototypeMain.ToString());
    }
}