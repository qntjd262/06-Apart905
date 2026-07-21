using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QuestTrackerUI : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI progressText;
    //public Image strikeLine;

    [Header("Color Settings")]
    private Color mainQuestColor = new Color(0.8f, 0.4f, 0f); // 어두운 주황색
    private Color subQuestColor = Color.green;

    private Quest targetQuest;
    private bool effectPlayed = false;

    void Awake()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RegisterTracker(this);
        }
    }

    private void OnEnable()
    {
        if (targetQuest == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        UpdateProgress();
    }

    public void Setup(Quest quest)
    {
        progressText.DOKill();
        //strikeLine.rectTransform.DOKill();

        targetQuest = quest;

        if (UIManager.Instance != null && UIManager.Instance.IsAnyUIOpen)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
        }

        effectPlayed = false;

        titleText.text = quest.questName;
        titleText.color = quest.isMainQuest ? mainQuestColor : subQuestColor;

        // [수정] localScale뿐 아니라 sizeDelta도 함께 초기화.
        // 이전 퀘스트의 취소선 크기가 다음 퀘스트로 그대로 이어지는 것을 방지.
        //RectTransform lineRect = strikeLine.rectTransform;
        //lineRect.localScale = new Vector3(0, 1, 1);
        //lineRect.sizeDelta = new Vector2(0, lineRect.sizeDelta.y);

        progressText.color = Color.white;
        UpdateProgress();
    }

    public void UpdateProgress()
    {
        if (targetQuest == null) return;

        // [수정] objectives가 없는(대사 전용 등) 퀘스트는 표시할 내용이 없으므로
        // 트래커 자체를 숨기고 즉시 완료 연출도 타지 않도록 방어.
        if (targetQuest.objectives == null || targetQuest.objectives.Count == 0)
        {
            progressText.text = "";
            return;
        }

        string progressDisplay = "";

        for (int i = 0; i < targetQuest.objectives.Count; i++)
        {
            var obj = targetQuest.objectives[i];

            progressDisplay += $"ㆍ {obj.targetID} ({obj.currentAmount}/{obj.goalAmount})";

            if (i < targetQuest.objectives.Count - 1)
            {
                progressDisplay += "\n";
            }
        }

        progressText.text = progressDisplay;

        // [수정] objectives.Count > 0 을 보장한 상태에서만 완료 체크.
        // (빈 리스트에서 All()이 true를 반환해 즉시 완료 연출이 튀는 것을 방지)
        if (targetQuest.IsAllObjectivesComplete() && !effectPlayed)
        {
            PlayCompleteEffect();
        }
    }

    private void PlayCompleteEffect()
    {
        effectPlayed = true;

        // [수정] 텍스트를 바꾼 직후 preferredWidth가 아직 갱신되지 않아
        // 이전 텍스트 기준 폭을 반환하는 문제를 막기 위해 강제로 메시 갱신.
        progressText.ForceMeshUpdate();
        float textWidth = progressText.preferredWidth;

        //RectTransform lineRect = strikeLine.rectTransform;
        //lineRect.sizeDelta = new Vector2(textWidth, lineRect.sizeDelta.y);

        //lineRect.localScale = new Vector3(0, 1, 1);
        //lineRect.DOScaleX(0.6f, 0.5f).SetEase(Ease.OutQuad);

        progressText.DOColor(Color.gray, 0.5f);
    }

    public void HideTracker()
    {
        progressText.DOKill();
        //strikeLine.rectTransform.DOKill();

        targetQuest = null;
        effectPlayed = false;
        this.gameObject.SetActive(false);
    }
}