using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCard : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterNameText;

    // 외부에서 데이터를 주입할 수 있는 유일한 public 메서드
    public void SetCard(CharacterStatSO data) //CharacterData data -> characterstatso data
    {
        //추가 사항 
        if (characterImage != null) characterImage.sprite = data.characterSprite;
        if (characterNameText != null) characterNameText.text = data.Name; //data.characterName -> data.Name
    }
}