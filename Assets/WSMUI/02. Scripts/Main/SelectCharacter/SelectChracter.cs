using UnityEngine;
using DG.Tweening;

public class SelectChracter : MonoBehaviour
{
    [SerializeField] private RectTransform container; 
    [SerializeField] private float spacing = 500f;    
    [SerializeField] private float duration = 0.5f;   
    [SerializeField] private Ease easeType = Ease.OutBack; 

    private int _currentIndex = 0;
    private int _totalCharacters;

    // 다른 클래스에서 현재 인덱스를 읽기만 할 수 있도록 프로퍼티 제공 (선택 사항)
    public int CurrentIndex => _currentIndex;

    void Start()
    {
        if (container == null) return;

        _totalCharacters = container.childCount;
        UpdatePosition(true);
    }

    // 버튼 클릭 이벤트는 public으로 유지 (UI Button에서 호출해야 함)
    public void OnNextButton()
    {
        if (_currentIndex < _totalCharacters - 1)
        {
            _currentIndex++;
            UpdatePosition();
        }
    }

    public void OnPrevButton()
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
            UpdatePosition();
        }
    }

    private void UpdatePosition(bool immediate = false)
    {
        float targetX = -_currentIndex * spacing;
        
        // 기존 트위닝이 실행 중이라면 정지시켜 꼬임을 방지
        container.DOKill(); 

        if (immediate)
        {
            container.anchoredPosition = new Vector2(targetX, 0);
        }
        else
        {
            container.DOAnchorPos(new Vector2(targetX, 0), duration)
                .SetEase(easeType);
        }
        
        UpdateScale();
    }

    private void UpdateScale()
    {
        for (int i = 0; i < _totalCharacters; i++)
        {
            Transform child = container.GetChild(i);
            child.DOKill(); // 개별 자식의 트위닝도 초기화

            float targetScale = (i == _currentIndex) ? 1.2f : 0.8f;
            child.DOScale(targetScale, duration).SetEase(easeType);
        }
    }
    public void OnClickSelectButton()
    {
        //SoundManager.Instance.PlaySFX("Button_Click");
        Debug.Log("Select button clicked");
        UIManager.Instance.LoadScene(Constants.ESceneType.Game);
    }
}
