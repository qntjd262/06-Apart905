using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QuestTrackerUI : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI progressText;

    public Image GoalStrikeLine;
    public Image strikeLine;

    [Header("Color Settings")]
    private Color mainQuestColor = new Color(0.8f, 0.4f, 0f); // 어두운 주황색
    private Color subQuestColor = Color.green;

    private Quest targetQuest;
    private bool effectPlayed = false;
    private bool goalEffectPlayed = false;

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
        goalText.DOKill();
        strikeLine.rectTransform.DOKill();
        if (GoalStrikeLine != null) GoalStrikeLine.rectTransform.DOKill();

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
        goalEffectPlayed = false;

        titleText.text = quest.questName;
        titleText.color = quest.isMainQuest ? mainQuestColor : subQuestColor;

        if (goalText != null)
        {
            goalText.text = quest.questGoal;
            goalText.color = Color.white;
        }       

        strikeLine.rectTransform.localScale = new Vector3(0, 1, 1);
        if (GoalStrikeLine != null) GoalStrikeLine.rectTransform.localScale = new Vector3(0, 1, 1);

        progressText.color = Color.white;

        UpdateProgress();
    }

    public void UpdateProgress()
    {
        if (targetQuest == null) return;

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

        if (targetQuest.objectives.Count > 0 && targetQuest.IsAllObjectivesComplete() && !effectPlayed)
        {
            PlayCompleteEffect();
        }

        if (targetQuest.isCompleted && !goalEffectPlayed)
        {
            PlayGoalCompleteEffect();
        }
    }

    private void PlayCompleteEffect()
    {
        effectPlayed = true;

        float textWidth = progressText.preferredWidth;

        RectTransform lineRect = strikeLine.rectTransform;
        lineRect.sizeDelta = new Vector2(textWidth, lineRect.sizeDelta.y);

        lineRect.localScale = new Vector3(0, 1, 1);
        lineRect.DOScaleX(0.6f, 0.5f).SetEase(Ease.OutQuad);

        progressText.DOColor(Color.gray, 0.5f);
    }
    
    public void PlayGoalCompleteEffect()
    {
        if (goalEffectPlayed) return;
        goalEffectPlayed = true;

        if (goalText != null && GoalStrikeLine != null)
        {
            float textWidth = goalText.preferredWidth;
            RectTransform lineRect = GoalStrikeLine.rectTransform;
            lineRect.sizeDelta = new Vector2(textWidth, lineRect.sizeDelta.y);

            lineRect.localScale = new Vector3(0, 1, 1);
            lineRect.DOScaleX(0.6f, 0.5f).SetEase(Ease.OutQuad);

            goalText.DOColor(Color.gray, 0.5f);
        }
    }

    public void HideTracker()
    {
        progressText.DOKill();
        goalText.DOKill();
        strikeLine.rectTransform.DOKill();
        if (GoalStrikeLine != null) GoalStrikeLine.rectTransform.DOKill();

        targetQuest = null;
        effectPlayed = false;
        goalEffectPlayed = false;
        this.gameObject.SetActive(false);
    }
}
