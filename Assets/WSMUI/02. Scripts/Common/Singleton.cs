using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    // 핵심: 앱 종료 상태를 추적하는 플래그
    private static bool _applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            // 게임이 종료 중일 때는 억지로 새로 만들지 않고 null을 반환한다.
            if (_applicationIsQuitting) 
            {
                return null;
            }

            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    protected abstract void OnSceneLoaded(Scene scene, LoadSceneMode mode);
    protected abstract void OnSceneUnloaded(Scene scene);

    protected virtual void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;

        // 파괴될 때 자기 자신이 인스턴스였다면 null로 비워준다.
        if (_instance == this)
        {
            _instance = null;
        }
    }

    // 유니티 생명주기: 앱이 강제 종료되거나 에디터 플레이가 꺼질 때 호출됨
    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }
}