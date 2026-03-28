using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    public string gameSceneName = "PlayerScene";

    public void OnClickCharacterButton(string characterName)
    {
        if(CharacterDataManager.Instance != null && CharacterDataManager.Instance.characterDB.ContainsKey(characterName))
        {
            CharacterDataManager.Instance.selectedCharacterSO = CharacterDataManager.Instance.characterDB[characterName];

            Debug.Log($"{characterName} 선택 완료");

            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.Log("아직 구글 드라이브 데이터 불러오는 중? , 이름이 잘못되었을수도");
        }
    }
}
