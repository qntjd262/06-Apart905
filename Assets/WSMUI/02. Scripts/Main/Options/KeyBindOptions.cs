using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class KeyBindOptions : MonoBehaviour
{
    [System.Serializable]
    public class KeyBindData
    {
        public string keyActionName; // 내부 키 식별자 (예: "Key_Interact")
        public Button bindButton;    // 맵핑된 에디터 버튼
        public TextMeshProUGUI bindText; // 버튼 내부 텍스트 컴포넌트
        public KeyCode defaultKey;   // 데이터가 없을 때 들어갈 기본 단축키
    }

    [Header("단축키 등록 리스트")]
    [SerializeField] private KeyBindData[] keyBindings;

    private bool isWaitingForKey = false;

    public void Initialize()
    {
        for (int i = 0; i < keyBindings.Length; i++)
        {
            KeyBindData data = keyBindings[i];
            
            // 기존에 저장한 키 코드를 문자열로 로드
            string savedKey = PlayerPrefs.GetString(data.keyActionName, data.defaultKey.ToString());
            data.bindText.text = savedKey;

            // 버튼마다 고유 코루틴 연결
            data.bindButton.onClick.RemoveAllListeners();
            data.bindButton.onClick.AddListener(() => StartRebind(data));
        }
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
        data.bindText.color = Color.red; // 입력 대기 상태 가시성 연출

        while (isWaitingForKey)
        {
            if (Input.anyKeyDown)
            {
                // 시스템 내부 KeyCode 전체 검색 루프 실행
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        // ESC 입력 시 바인딩 취소 처리
                        if (keyCode == KeyCode.Escape)
                        {
                            string currentKey = PlayerPrefs.GetString(data.keyActionName, data.defaultKey.ToString());
                            data.bindText.text = currentKey;
                            isWaitingForKey = false;
                            break;
                        }

                        // 마우스 기본 입력 제어권 보호를 위한 방어 코드
                        if (keyCode != KeyCode.Mouse0 && keyCode != KeyCode.Mouse1)
                        {
                            PlayerPrefs.SetString(data.keyActionName, keyCode.ToString());
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
}