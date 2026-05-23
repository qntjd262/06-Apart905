using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EKeyAction
{
    Interact,
    Inventory,
    Pause,
    Flashlight,
}

public class InputManager : Singleton<InputManager>
{
    private Dictionary<EKeyAction, KeyCode> keyBindings = new Dictionary<EKeyAction, KeyCode>();

    protected override void Awake()
    {
        // 1. [싱글톤 방어] 이미 인스턴스가 존재하는데 내가 중복 생성된 껍데기라면 즉시 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        base.Awake();

        // 2. [씬 전환 방어] 씬이 바뀌어도 저장된 메모리가 파괴되지 않도록 루트로 승격
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        // 3. 데이터 로드
        LoadAllKeys();
    }

    public void LoadAllKeys()
    {
        if (keyBindings == null) keyBindings = new Dictionary<EKeyAction, KeyCode>();

        keyBindings[EKeyAction.Interact] = ParseKey("Key_Interact", KeyCode.E);
        keyBindings[EKeyAction.Inventory] = ParseKey("Key_Inventory", KeyCode.I);
        keyBindings[EKeyAction.Pause] = ParseKey("Key_Pause", KeyCode.Escape);
        keyBindings[EKeyAction.Flashlight] = ParseKey("Key_Flashlight", KeyCode.F);
        
        Debug.Log($"[InputManager] 모든 키 동기화 완료. 인벤토리 키: {keyBindings[EKeyAction.Inventory]}");
    }

    private KeyCode ParseKey(string prefsKey, KeyCode defaultKey)
    {
        string savedKey = PlayerPrefs.GetString(prefsKey, defaultKey.ToString());
        if (Enum.TryParse(savedKey, out KeyCode parsedKey))
        {
            return parsedKey;
        }
        return defaultKey;
    }

    public void UpdateKey(EKeyAction action, KeyCode newKey)
    {
        if (keyBindings == null) LoadAllKeys();

        if (keyBindings.ContainsKey(action))
        {
            keyBindings[action] = newKey;
            Debug.Log($"[InputManager] 메모리 실시간 갱신: {action} -> {newKey}");
        }
    }

    public bool GetKeyDown(EKeyAction action)
    {
        if (keyBindings == null || keyBindings.Count == 0)
        {
            LoadAllKeys();
        }

        if (keyBindings.TryGetValue(action, out KeyCode key))
        {
            return Input.GetKeyDown(key);
        }
        return false;
    }

    public KeyCode GetKeyForAction(EKeyAction action)
    {
        if (keyBindings == null || keyBindings.Count == 0)
        {
            LoadAllKeys();
        }

        if (keyBindings.TryGetValue(action, out KeyCode key))
        {
            return key;
        }
        return KeyCode.None;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 바뀔 때 하드디스크에 저장되어 있던 변경 데이터를 다시 정렬한다.
        LoadAllKeys();
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
    }
}