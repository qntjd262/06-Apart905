using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject playerPrefab;

    public CharacterStatSO SelectedCharacterData { get; private set; }

    public void SetCharacter(CharacterStatSO data)
    {
        if (data != null) SelectedCharacterData = data;
    }
    
    private GameObject _player;

    // 게임 시작 시 HUD 등 UI를 초기화하라는 이벤트
    public System.Action OnGameStart;

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 게임 씬으로 진입했을 때만 실행
        if (scene.name == Constants.ESceneType.Game.ToString())
        {
            SpawnAndInitializePlayer();
        }
        else if (scene.name == Constants.ESceneType.Main.ToString())
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
        // var spawnPointObj = GameObject.FindGameObjectWithTag("SpawnPoint");
        // if (spawnPointObj == null) return;

        // Transform spawnPoint = spawnPointObj.transform;

        // // 1. 플레이어 생성 또는 위치 재조정
        // if (_player == null)
        // {
        //     _player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        // }
        // else
        // {
        //     _player.transform.position = spawnPoint.position;
        //     _player.transform.rotation = spawnPoint.rotation;
        //     _player.SetActive(true);
        // }

        // 2. 플레이어 데이터 주입 (플레이어 담당자 스크립트 연결)
        // 주석 해제해서 사용해라.
        /*
        var playerStatus = _player.GetComponent<PlayerStatus>();
        if (playerStatus != null && SelectedCharacterData != null)
        {
            // 캐릭터 기본 스탯 주입 (HP, Hunger, Thirst는 Max치, Sanity는 0으로)
            playerStatus.SetInitialStatus(SelectedCharacterData);
        }
        */

        // 3. UI 초기화 이벤트 발생
        // UIManager가 이 신호를 듣고 알아서 HUD를 켤 것이다. GameManager가 HUD를 직접 찾을 필요 없다.
        OnGameStart?.Invoke();
    }

    protected override void OnSceneUnloaded(Scene scene)
    {

    }
}