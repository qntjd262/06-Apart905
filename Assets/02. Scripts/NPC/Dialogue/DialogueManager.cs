using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

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
    public bool IsDialogueActive {get; private set; }

    void Start()
    {
        if(dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public void StartDialogue(string[] lines, System.Action onComplete)
    {
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);
        StartCoroutine(PlayDialogue(lines, onComplete));
    }

    private IEnumerator PlayDialogue(string[] lines, System.Action onComplete)
    {
        foreach (string line in lines)
        {
            dialogueText.text = "";
            isTyping = true;
            
            foreach (char letter in line.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
            
            isTyping = false;

            yield return null;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
            yield return null;
        }

        //dialoguePanel.SetActive(false);
        //IsDialogueActive = false;
        onComplete?.Invoke(); // 대화가 끝나면 실행
    }

    public void EndDialogue()
    {
        if(dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        IsDialogueActive = false;
    }
}
