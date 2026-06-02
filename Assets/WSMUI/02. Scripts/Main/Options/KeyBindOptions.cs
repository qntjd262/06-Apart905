using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class KeyBindOptions : MonoBehaviour
{
    [System.Serializable]
    public class KeyBindData
    {
        public EKeyAction targetAction;  
        public string keyActionName;     
        public Button bindButton;        
        public TextMeshProUGUI bindText; 
        public KeyCode defaultKey;       
        
        [HideInInspector] public KeyCode tempKey; 
    }

    [Header("단축키 등록 리스트")]
    [SerializeField] private KeyBindData[] keyBindings;

    private bool isWaitingForKey = false;

    public void Initialize()
    {
        StopAllCoroutines();
        isWaitingForKey = false;

        for (int i = 0; i < keyBindings.Length; i++)
        {
            KeyBindData data = keyBindings[i];

            string savedKeyStr = PlayerPrefs.GetString(data.keyActionName, data.defaultKey.ToString());
            
            if (System.Enum.TryParse(savedKeyStr, out KeyCode savedKey))
            {
                data.tempKey = savedKey;
            }
            else
            {
                data.tempKey = data.defaultKey;
            }

            data.bindText.text = data.tempKey.ToString();
            data.bindText.color = Color.white;

            data.bindButton.onClick.RemoveAllListeners();
            data.bindButton.onClick.AddListener(() => StartRebind(data));
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isWaitingForKey = false;
    }

    private void StartRebind(KeyBindData data)
    {
        if (isWaitingForKey) return;
        StartCoroutine(WaitForKeyPressCoroutine(data));
    }

    private IEnumerator WaitForKeyPressCoroutine(KeyBindData data)
    {
        isWaitingForKey = true;
        data.bindText.text = "<입력 대기>";
        data.bindText.color = Color.red; 

        while (isWaitingForKey)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        if (keyCode == KeyCode.Escape)
                        {
                            // ESC를 누르면 변경을 취소하고 현재 들고 있던 tempKey 값으로 UI 복구
                            data.bindText.text = data.tempKey.ToString();
                            isWaitingForKey = false;
                            break;
                        }

                        if (keyCode != KeyCode.Mouse0 && keyCode != KeyCode.Mouse1)
                        {
                            // [핵심 수정] 즉시 PlayerPrefs나 InputManager를 건들지 않는다.
                            // 오직 임시 변수(tempKey)와 화면 글씨만 실시간으로 바꿔서 유저에게 보여준다.
                            data.tempKey = keyCode;
                            data.bindText.text = keyCode.ToString();

                            isWaitingForKey = false;
                            break;
                        }
                    }
                }
            }
            yield return null;
        }

        data.bindText.color = Color.white;
    }

    public void SaveOptions()
    {
        for (int i = 0; i < keyBindings.Length; i++)
        {
            KeyBindData data = keyBindings[i];
            
            PlayerPrefs.SetString(data.keyActionName, data.tempKey.ToString());
            
            if (InputManager.Instance != null)
            {
                InputManager.Instance.UpdateKey(data.targetAction, data.tempKey);
            }
        }
    }

    public void RevertOptions()
    {
        Initialize();
    }
}