using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlOptions : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject keyBindItemPrefab;

    [Header("마우스 감도 슬라이더")]
    [SerializeField] private Slider sliderH;
    [SerializeField] private Slider sliderV;
    [SerializeField] private TextMeshProUGUI textH;
    [SerializeField] private TextMeshProUGUI textV;

    private List<KeyBindList> instantiatedItems = new List<KeyBindList>();
    private bool isWaitingForKey = false;
    private KeyCode[] cachedKeyCodes;

    private const float DefaultSens = 1.0f;

    public void Initialize()
    {
        StopAllCoroutines();
        isWaitingForKey = false;

        if (cachedKeyCodes == null)
        {
            cachedKeyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
        }

        if (InputManager.Instance != null) InputManager.Instance.LoadAllKeys();

        if (InputManager.Instance != null)
        {
            sliderH.onValueChanged.RemoveAllListeners();
            sliderV.onValueChanged.RemoveAllListeners();

            sliderH.value = InputManager.Instance.MouseSensH;
            sliderV.value = InputManager.Instance.MouseSensV;

            if (textH != null) textH.text = sliderH.value.ToString("F1");
            if (textV != null) textV.text = sliderV.value.ToString("F1");

            sliderH.onValueChanged.AddListener(v =>
            {
                if (textH != null) textH.text = v.ToString("F1");
                InputManager.Instance.SetSensitivityRealTime(v, sliderV.value);
            });

            sliderV.onValueChanged.AddListener(v =>
            {
                if (textV != null) textV.text = v.ToString("F1");
                InputManager.Instance.SetSensitivityRealTime(sliderH.value, v);
            });
        }
        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(contentParent.GetChild(i).gameObject);
        }
        instantiatedItems.Clear();

        foreach (EKeyAction action in System.Enum.GetValues(typeof(EKeyAction)))
        {
            // 만약 유저가 임의로 변경하면 안 되는 키(예: 일시정지)가 있다면 
            // 아래처럼 예외 처리를 해서 리스트에 안 뜨게 막을 수 있습니다.
            // if (action == EKeyAction.Pause) continue; 

            GameObject go = Instantiate(keyBindItemPrefab, contentParent);
            KeyBindList itemUI = go.GetComponent<KeyBindList>();

            KeyCode currentKey = KeyCode.None;
            if (InputManager.Instance != null)
            {
                currentKey = InputManager.Instance.GetKeyForAction(action);
            }

            itemUI.Initialize(action, currentKey, StartRebind);
            instantiatedItems.Add(itemUI);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isWaitingForKey = false;
    }

    private void StartRebind(KeyBindList targetItem)
    {
        if (isWaitingForKey) return;
        StartCoroutine(WaitForKeyPressCoroutine(targetItem));
    }

    private bool IsPointerOverSelectableUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<UnityEngine.UI.Selectable>() != null)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator WaitForKeyPressCoroutine(KeyBindList targetItem)
    {
        isWaitingForKey = true;
        targetItem.SetWaitingState();

        // 마우스 클릭으로 인한 즉시 할당 방지
        yield return new WaitUntil(() => !Input.GetMouseButton(0));

        while (isWaitingForKey)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in cachedKeyCodes)
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        if (keyCode == KeyCode.Escape)
                        {
                            targetItem.UpdateUI(targetItem.TempKey);
                            isWaitingForKey = false;
                            break;
                        }

                        if (keyCode == KeyCode.Mouse0 || keyCode == KeyCode.Mouse1 || keyCode == KeyCode.Mouse2)
                        {
                            if (IsPointerOverSelectableUI())
                            {
                                continue;
                            }
                        }

                        KeyBindList duplicateItem = instantiatedItems.Find(x => x != targetItem && x.TempKey == keyCode);
                        if (duplicateItem != null)
                        {
                            duplicateItem.UpdateUI(KeyCode.None);
                        }

                        targetItem.UpdateUI(keyCode);
                        isWaitingForKey = false;
                        break;
                    }
                }
            }
            yield return null;
        }
    }

    public void ResetToDefault()
    {
        sliderH.value = 2.0f;
        sliderV.value = 2.0f;

        if (InputManager.Instance == null) return;

        foreach (KeyBindList item in instantiatedItems)
        {
            KeyCode defaultKey = InputManager.Instance.GetDefaultKey(item.TargetAction);
            item.UpdateUI(defaultKey);
        }
        Debug.Log("[KeyBindOptions] 화면의 단축키가 기본값으로 변경되었습니다. (확인 버튼을 눌러야 저장됩니다)");
    }

    public void SaveOptions()
    {
        InputManager.Instance.SaveSensitivityToDisk();

        foreach (KeyBindList item in instantiatedItems)
        {
            string prefsKeyName = "Key_" + item.TargetAction.ToString();
            PlayerPrefs.SetString(prefsKeyName, item.TempKey.ToString());

            if (InputManager.Instance != null)
            {
                InputManager.Instance.UpdateKey(item.TargetAction, item.TempKey);
            }
        }
    }

    public void RevertOptions()
    {
        Initialize();
    }
}