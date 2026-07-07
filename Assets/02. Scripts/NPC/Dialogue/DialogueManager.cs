using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }
    protected override void OnSceneUnloaded(Scene scene) { }

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.05f;

    private bool isTyping = false;
    private bool skipRequested = false; // 타이핑 스킵용
    public bool IsDialogueActive { get; private set; }

    public TextMeshProUGUI nameText;       // 대사마다 변경될 이름 표시용
    public Image npcPortrait;              // 대사마다 변경될 이미지 표시용

    [Header("대화 종료 후 딜레이")]
    [SerializeField] private float blockDuration = 0.3f;

    void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public void StartDialogue(NPCdata npcInfo, DialogueLine[] lines, System.Action onComplete)
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.Log($"{npcInfo?.NpcName ?? "NPC"}: 출력할 대사가 없으므로 스킵하고 퀘스트 창으로 이동합니다.");
            onComplete?.Invoke();
            return;
        }

        if (IsDialogueActive) return; 

        IsDialogueActive = true;
        dialoguePanel.SetActive(true);

        if (npcInfo != null)
        {
            if (nameText != null) nameText.text = npcInfo.NpcName;
            
            if (npcPortrait != null)
            {
                if (npcInfo.NpcImage != null)
                {
                    npcPortrait.gameObject.SetActive(true);
                    npcPortrait.sprite = npcInfo.NpcImage;
                }
                else
                {
                    npcPortrait.gameObject.SetActive(false);
                }
            }
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCursorState();
        }

        // 대사 시퀀스 실행
        StartCoroutine(PlayDialogue(lines, onComplete));
    }

    private IEnumerator PlayDialogue(DialogueLine[] lines, System.Action onComplete)
    {
        foreach (DialogueLine line in lines)
        {
            if (line.speakerData != null)
            {
                if (nameText != null) 
                    nameText.text = line.speakerData.NpcName;

                if (npcPortrait != null)
                {
                    if (line.speakerData.NpcImage != null)
                    {
                        npcPortrait.gameObject.SetActive(true);
                        npcPortrait.sprite = line.speakerData.NpcImage;
                    }
                    else
                    {
                        npcPortrait.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (nameText != null) nameText.text = "";
                if (npcPortrait != null) npcPortrait.gameObject.SetActive(false);
            }

            dialogueText.text = "";
            isTyping = true;
            skipRequested = false;

            foreach (char letter in line.dialogueText.ToCharArray())
            {
                if (skipRequested)
                {
                    dialogueText.text = line.dialogueText;
                    break;
                }
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;

            yield return null;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
        }

        StartCoroutine(EndDialogueRoutine(onComplete));
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
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        IsDialogueActive = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCursorState();
        }

        onComplete?.Invoke();
    }

    private IEnumerator EndDialogueRoutine(System.Action onComplete)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCursorState();
        }

        onComplete?.Invoke();

        yield return new WaitForSeconds(blockDuration);

        IsDialogueActive = false; // 0.3초가 지나서야 플레이어 행동 가능
    }

    public void EndDialogue()
    {
        StartCoroutine(EndDialogueRoutine(null));
    }
}