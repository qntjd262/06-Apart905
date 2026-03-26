// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class SoundManager : Singleton<SoundManager>
// {
//     [Header("Audio Sources")]
//     [SerializeField] private AudioSource bgmSource;
//     [SerializeField] private AudioSource sfxSource; // 혹은 여러 개의 소스를 리스트로 관리

//     [Header("Audio Clips")]
//     [SerializeField] private AudioClip[] sfxClips; // 이름이나 인덱스로 찾을 클립들

//     protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//     {
//         // 씬이 바뀌었을 때 특정 배경음을 틀거나 끄는 로직
//     }

//     protected override void OnSceneUnloaded(Scene scene) { }

//     public void PlaySFX(string clipName)
//     {
//         // 이름으로 클립을 찾아 sfxSource.PlayOneShot() 실행
//     }

//     public void SetVolume(string groupName, float value)
//     {
//         // AudioMixer 파라미터 조절 로직
//     }

// }
