using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    protected override void OnSceneUnloaded(Scene scene)
    {

    }

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.05f;

    private bool isTyping = false;
    private bool skipRequested = false; // 타이핑 스킵용
    public bool IsDialogueActive { get; private set; }

    public TextMeshProUGUI nameText;       // NPC 이름 표시용
    public Image npcPortrait;              // NPC 이미지 표시용

    void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public void StartDialogue(string npcName, Sprite portrait, string[] lines, System.Action onComplete)
    {
        if (IsDialogueActive) return; // 이미 대화 중이면 중복 방지
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);

        nameText.text = npcName;
        if (portrait != null)
        {
            npcPortrait.sprite = portrait;
            npcPortrait.gameObject.SetActive(true);
        }
        else
        {
            npcPortrait.gameObject.SetActive(false); // 이미지가 없으면 숨김
        }

        UIManager.Instance.UpdateCursorState();
        StartCoroutine(PlayDialogue(lines, onComplete));
    }

    private IEnumerator PlayDialogue(string[] lines, System.Action onComplete)
    {
        foreach (string line in lines)
        {
            dialogueText.text = "";
            isTyping = true;
            skipRequested = false;

            foreach (char letter in line.ToCharArray())
            {
                if (skipRequested)
                {
                    dialogueText.text = line;
                    break;
                }
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;

            yield return null;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
        }


    private void Update()
    {
        // 타이핑 중에 클릭하면 스킵 플래그 활성화
        if (isTyping && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            skipRequested = true;
        }
    }

    private void EndDialogue(System.Action onComplete)
    {
        if(dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        IsDialogueActive = false;

        if(UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCursorState();
        }

        onComplete?.Invoke();
    }

    public void EndDialogue()
    {
        EndDialogue(null);
    }
}
