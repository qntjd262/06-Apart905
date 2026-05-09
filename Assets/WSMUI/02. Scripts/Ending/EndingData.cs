using UnityEngine;

[CreateAssetMenu(fileName = "EndingData", menuName = "Scriptable Objects/EndingData")]
public class EndingData : ScriptableObject
{
    public string endingName;
    [TextArea] public string endingDescription; // 엔딩 씬에서 보여줄 줄거리
    public Sprite endingIllustration;          // 엔딩 결과 화면 이미지
}
