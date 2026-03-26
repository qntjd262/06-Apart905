using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject playerPrefab;
    
    private GameObject _player;
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // switch (scene.name)
        // {
        //     case "Main":
        //         if (_player)
        //         {
        //             Destroy(_player);
        //             _player = null;
        //         }
        //         break;
        //     case "Character":
        //     case "Map":
        //         var spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
                
        //         if (_player)
        //         {
        //             _player.transform.position = spawnPoint.position;
        //             _player.transform.rotation = spawnPoint.rotation;
        //             _player.SetActive(true);
        //         }
        //         else
        //         {
        //             _player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        //             DontDestroyOnLoad(_player);
        //         }
        //         break;
        // }
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
        if (_player) _player.SetActive(false);
    }
}
