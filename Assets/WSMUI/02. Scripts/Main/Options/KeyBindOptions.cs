using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class KeyBindOptions : MonoBehaviour
{
    [System.Serializable]
    public class KeyBindData
    {
        // [수정 핵심] 문자열 대신 인스펙터에서 드롭다운으로 고를 수 있게 변수 교체
        public EKeyAction targetAction;  
        public string keyActionName;     // 저장 데이터 식별용 (예: "Key_Interact")
        public Button bindButton;        
        public TextMeshProUGUI bindText; 
        public KeyCode defaultKey;       
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

            string savedKey = PlayerPrefs.GetString(data.keyActionName, data.defaultKey.ToString());
            data.bindText.text = savedKey;
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
                            string currentKey = PlayerPrefs.GetString(data.keyActionName, data.defaultKey.ToString());
                            data.bindText.text = currentKey;
                            isWaitingForKey = false;
                            break;
                        }

                        if (keyCode != KeyCode.Mouse0 && keyCode != KeyCode.Mouse1)
                        {
                            PlayerPrefs.SetString(data.keyActionName, keyCode.ToString());
                            data.bindText.text = keyCode.ToString();

                            // [수정 핵심] 지저분한 문자열 파싱을 지우고, 지정된 액션 열거형을 직접 꽂아 넣는다.
                            InputManager.Instance.UpdateKey(data.targetAction, keyCode);

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
}