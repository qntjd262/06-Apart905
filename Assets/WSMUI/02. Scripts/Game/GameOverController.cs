using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using UnityEngine.UI; // Image 컴포넌트를 사용하기 위해 필요

public class GameOverController : MonoBehaviour
{
    [SerializeField] private GameObject bloodSplatterEffect;
    [SerializeField] private GameObject buttonGroup;

    public SaveLoadController saveLoadController;
    private Image bloodImage; // Image 컴포넌트 참조 저장용

    private void Awake()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.gameOverPanel = this.gameObject;
        }

        // bloodSplatterEffect에서 Image 컴포넌트 미리 캐싱
        if (bloodSplatterEffect != null)
        {
            bloodImage = bloodSplatterEffect.GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        // 1. 게임 일시정지
        Time.timeScale = 0f;

        // 2. 초기화: 버튼 그룹은 숨기고, 블러드는 투명하게
        if (buttonGroup != null) buttonGroup.SetActive(false);
        
        // 3. 연출 시작
        PlayGameOverSequence();
    }

    private void PlayGameOverSequence()
    {
        if (bloodSplatterEffect == null) return;

        // 초기 상태 강제 설정 (스케일 0, 투명도 0)
        bloodSplatterEffect.transform.localScale = Vector3.zero;
        if (bloodImage != null) bloodImage.color = new Color(1, 1, 1, 0);

        Sequence mainSeq = DOTween.Sequence();

        // [단계 1] 블러드 효과 연출: 확 커졌다가(1.2f) 원래 크기(1.0f)로
        mainSeq.Append(bloodSplatterEffect.transform.DOScale(1.2f, 0.15f).SetEase(Ease.OutExpo));
        if (bloodImage != null)
        {
            mainSeq.Join(bloodImage.DOFade(1f, 0.1f));
        }
        mainSeq.Append(bloodSplatterEffect.transform.DOScale(1.0f, 0.1f));

        // [단계 2] 아주 짧은 대기 (0.1초)
        mainSeq.AppendInterval(0.1f);

        // [단계 3] 연출 완료 후 버튼 그룹 등장
        mainSeq.OnComplete(() =>
        {
            if (buttonGroup != null)
            {
                buttonGroup.SetActive(true);
                
                // 버튼 그룹도 스케일 업 연출 (통통 튀는 느낌을 위해 OutBack 사용)
                buttonGroup.transform.localScale = Vector3.zero;
                buttonGroup.transform.DOScale(1.0f, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true); // 중요: 타임스케일 0일 때 작동
            }
        });

        // 시퀀스 전체가 타임스케일 영향을 받지 않도록 설정
        mainSeq.SetUpdate(true);
    }

    public void OnLoadClick()
    {
        SaveLoadController saveLoader = FindFirstObjectByType<SaveLoadController>(FindObjectsInactive.Include);
        if (saveLoader != null)
        {
            saveLoader.currentMode = Constants.ESaveLoadType.Load;
            saveLoader.ReturnToPauseMenuOnClose = true;
            gameObject.SetActive(false);
            saveLoader.gameObject.SetActive(true);
        }
    }

    public void OnMainMenuClick()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.LoadScene(Constants.ESceneType.Main);
            gameObject.SetActive(false);
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(Constants.ESceneType.Main.ToString());
    }
}