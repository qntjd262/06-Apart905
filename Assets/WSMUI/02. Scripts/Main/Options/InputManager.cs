using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EKeyAction
{
    MoveUp, MoveDown, MoveLeft, MoveRight, Sprint, Crouch,
    Interact, Attack, Flashlight, Inventory,
    QuickSlot1, QuickSlot2, QuickSlot3, QuickSlot4, QuickSlot5
}

public class InputManager : Singleton<InputManager>
{
    private Dictionary<EKeyAction, KeyCode> keyBindings = new Dictionary<EKeyAction, KeyCode>();

    public float MouseSensH { get; set; }
    public float MouseSensV { get; set; }

    private const float DefaultSens = 2.0f;

    private readonly Dictionary<EKeyAction, KeyCode> defaultBindings = new Dictionary<EKeyAction, KeyCode>()
    {
        { EKeyAction.MoveUp, KeyCode.W },
        { EKeyAction.MoveDown, KeyCode.S },
        { EKeyAction.MoveLeft, KeyCode.A },
        { EKeyAction.MoveRight, KeyCode.D },
        { EKeyAction.Sprint, KeyCode.LeftShift },
        { EKeyAction.Crouch, KeyCode.LeftControl },
        { EKeyAction.Interact, KeyCode.E },
        { EKeyAction.Attack, KeyCode.Mouse0 },
        { EKeyAction.Flashlight, KeyCode.F },
        { EKeyAction.Inventory, KeyCode.I },
        { EKeyAction.QuickSlot1, KeyCode.Alpha1 },
        { EKeyAction.QuickSlot2, KeyCode.Alpha2 },
        { EKeyAction.QuickSlot3, KeyCode.Alpha3 },
        { EKeyAction.QuickSlot4, KeyCode.Alpha4 },
        { EKeyAction.QuickSlot5, KeyCode.Alpha5 }
    };

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        base.Awake();
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        LoadAllKeys();
    }

    public void LoadAllKeys()
    {
        if (keyBindings == null) keyBindings = new Dictionary<EKeyAction, KeyCode>();

        foreach (var kvp in defaultBindings)
        {
            string prefsKeyName = "Key_" + kvp.Key.ToString();
            keyBindings[kvp.Key] = ParseKey(prefsKeyName, kvp.Value);
        }

        MouseSensH = PlayerPrefs.GetFloat("MouseSensH", DefaultSens);
        MouseSensV = PlayerPrefs.GetFloat("MouseSensV", DefaultSens);
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

    public void SetSensitivityRealTime(float h, float v)
    {
        MouseSensH = h;
        MouseSensV = v;
    }

    public void SaveSensitivityToDisk()
    {
        PlayerPrefs.SetFloat("MouseSensH", MouseSensH);
        PlayerPrefs.SetFloat("MouseSensV", MouseSensV);
        PlayerPrefs.Save();
    }

    public void UpdateKey(EKeyAction action, KeyCode newKey)
    {
        if (keyBindings == null) LoadAllKeys();

        if (keyBindings.ContainsKey(action))
        {
            keyBindings[action] = newKey;
            // Debug.Log($"[InputManager] 메모리 실시간 갱신: {action} -> {newKey}");
        }
    }

    public KeyCode GetDefaultKey(EKeyAction action)
    {
        if (defaultBindings.TryGetValue(action, out KeyCode defaultKey)) return defaultKey;
        return KeyCode.None;
    }

    public void ResetAllToDefaults()
    {
        foreach (var kvp in defaultBindings)
        {
            string prefsKeyName = "Key_" + kvp.Key.ToString();
            PlayerPrefs.DeleteKey(prefsKeyName);
            keyBindings[kvp.Key] = kvp.Value;
        }
        Debug.Log("[InputManager] 모든 단축키가 기본값으로 초기화되었습니다.");
    }

    public bool GetKeyDown(EKeyAction action)
    {
        if (keyBindings == null || keyBindings.Count == 0) LoadAllKeys();

        if (keyBindings.TryGetValue(action, out KeyCode key)) return Input.GetKeyDown(key);
        return false;
    }

    public bool GetKey(EKeyAction action)
    {
        if (keyBindings == null || keyBindings.Count == 0) LoadAllKeys();

        if (keyBindings.TryGetValue(action, out KeyCode key)) return Input.GetKey(key);
        return false;
    }

    public bool GetKeyUp(EKeyAction action)
    {
        if (keyBindings == null || keyBindings.Count == 0) LoadAllKeys();

        if (keyBindings.TryGetValue(action, out KeyCode key)) return Input.GetKeyUp(key);
        return false;
    }

    public KeyCode GetKeyForAction(EKeyAction action)
    {
        if (keyBindings == null || keyBindings.Count == 0) LoadAllKeys();

        if (keyBindings.TryGetValue(action, out KeyCode key)) return key;
        return KeyCode.None;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadAllKeys();
    }

    protected override void OnSceneUnloaded(Scene scene) { }
}