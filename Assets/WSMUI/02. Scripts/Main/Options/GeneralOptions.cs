using UnityEngine;

public class GeneralOptions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize()
    {

    }
    public void ResetToDefault()
    {
        Debug.Log("[GeneralOptions] 일반 설정이 기본값으로 초기화 대기 중입니다.");
    }
    public void SaveOptions()
    {
        Debug.Log("[GeneralOptions] 일반 설정이 저장되었습니다.");
    }
    public void RevertOptions()
    {
        Debug.Log("[GeneralOptions] 일반 설정이 이전 저장 상태로 롤백되었습니다.");
    }
}
