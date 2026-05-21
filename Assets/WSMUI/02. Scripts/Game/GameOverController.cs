using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
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

        if (UIManager.Instance != null)
        {
            UIManager.Instance.gameOverPanel = this.gameObject;
        }

        if (bloodSplatterEffect != null)
        {
            bloodImage = bloodSplatterEffect.GetComponent<Image>();
        }
    }

    // [수정] 외부(PlayerStat 등)에서 사망 시 안전하게 열어주는 관문
    public void Open()
    {
        ShowPanel(); // SetActive(true) 및 스택 등록

        // 일시정지 상태에서 연출이 물리적으로 돌아가도록 Open 시점에 연출 강제 시작
        PlayGameOverSequence();
    }

    private void OnEnable()
    {
        // 버튼 그룹은 연출이 끝나기 전까지 유저 조작을 막기 위해 꺼둔다.
        if (buttonGroup != null) buttonGroup.SetActive(false);
    }

    private void PlayGameOverSequence()
    {
        if (bloodSplatterEffect == null) return;

        // 초기 상태 설정
        bloodSplatterEffect.transform.localScale = Vector3.zero;
        if (bloodImage != null) bloodImage.color = new Color(1, 1, 1, 0);

        Sequence mainSeq = DOTween.Sequence().SetUpdate(true);

        mainSeq.Append(bloodSplatterEffect.transform.DOScale(1.2f, 0.15f).SetEase(Ease.OutExpo).SetUpdate(true));
        if (bloodImage != null)
        {
            mainSeq.Join(bloodImage.DOFade(1f, 0.1f).SetUpdate(true));
        }
        mainSeq.Append(bloodSplatterEffect.transform.DOScale(1.0f, 0.1f).SetUpdate(true));

        mainSeq.AppendInterval(0.1f);

        mainSeq.OnComplete(() =>
        {
            if (buttonGroup != null)
            {
                buttonGroup.SetActive(true);
                buttonGroup.transform.localScale = Vector3.zero;

                // 버튼이 백에서 튕겨 나오며 스케일업 되는 연출도 독립 시간 적용
                buttonGroup.transform.DOScale(1.0f, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true);
            }
        });
    }

    public void OnLoadClick()
    {
        if (UIManager.Instance != null && UIManager.Instance.saveLoadController != null)
        {
            HidePanel();
            UIManager.Instance.saveLoadController.onCloseAction = null;
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
        HidePanel();
        Time.timeScale = 1f;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.LoadScene(Constants.ESceneType.PrototypeMain, false);
            return;
        }
        SceneManager.LoadScene(Constants.ESceneType.PrototypeMain.ToString());
    }
}