using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public CharacterStatSO SelectedCharacterData { get; private set; }

    public EndingData selectedEnding;

    public void SetCharacter(CharacterStatSO data)
    {
        if (data != null) SelectedCharacterData = data;
    }

    private GameObject _player;

    // 게임 시작 시 HUD 등 UI를 초기화하라는 이벤트
    public System.Action OnGameStart;

    public bool IsPaused { get; private set; }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        SoundManager.Instance?.PauseBGM();
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        SoundManager.Instance?.ResumeBGM();
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        // 게임 씬으로 진입했을 때만 실행
        if (scene.name == Constants.ESceneType.PrototypeGame.ToString())
        {
            SpawnAndInitializePlayer();
        }
        else if (scene.name == Constants.ESceneType.PrototypeMain.ToString())
        {
            // 메인으로 돌아왔을 때 플레이어 파괴 처리 등
            if (_player != null)
            {
                Destroy(_player);
                _player = null;
            }
        }
    }

    private void SpawnAndInitializePlayer()
    {
        // 3. UI 초기화 이벤트 발생
        // UIManager가 이 신호를 듣고 알아서 HUD를 켤 것이다. GameManager가 HUD를 직접 찾을 필요 없다.
        OnGameStart?.Invoke();
    }

    protected override void OnSceneUnloaded(Scene scene)
    {

    }
}