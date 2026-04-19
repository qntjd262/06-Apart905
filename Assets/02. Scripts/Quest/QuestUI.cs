using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public GameObject questTextPrefab;
    public Transform contentParent;

    private List<GameObject> activeUIObjects = new List<GameObject>();

    void OnEnable()
    {
        CancelInvoke("RefreshQuestList");
        Invoke("RefreshQuestList", 0.05f);     
    }

    public void RefreshQuestList()
    {
        if(QuestManager.Instance == null) 
        {
            Debug.Log("퀘스트매니저의 인스턴스를 찾지 못햇습니다.");
            return;
        }

        foreach(var text in activeUIObjects)
        {
            Destroy(text);
        }
        activeUIObjects.Clear();
        Debug.Log($"현재 매니저 내 퀘스트 개수: {QuestManager.Instance.activeQuests.Count}");


        foreach(var quest in QuestManager.Instance.activeQuests)
        {
            GameObject go = Instantiate(questTextPrefab, contentParent);
            QuestUIText uiText = go.GetComponent<QuestUIText>();

            if(uiText != null)
            {
                uiText.Setup(quest);
                activeUIObjects.Add(go);
            }
        }
    }
}
