using System;

using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerGamemanager : MonoBehaviour
{
    public static PlayerGamemanager Instance;

    
    public static event Action OnGameStatChangeTime;

    [Header("시간 설정")]
    private float timer = 0f;
    public float timeInterval = 3f;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= timeInterval)
        {
            OnGameStatChangeTime?.Invoke();
            timer = 0f;
        }
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene(2);
    }
}
