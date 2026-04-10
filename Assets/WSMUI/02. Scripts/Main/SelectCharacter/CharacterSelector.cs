using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CharacterSelector : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private CharacterData[] characterDatas;

    [Header("UI References - Info Box (Left)")]

    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI defText;
    [SerializeField] private TextMeshProUGUI stamText;
    [SerializeField] private TextMeshProUGUI thirstText;
    [SerializeField] private TextMeshProUGUI hungerText;
    [SerializeField] private TextMeshProUGUI sanityText;

    [Header("UI References - Card List")]
    [SerializeField] private RectTransform container;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [Header("Settings")]
    [SerializeField] private float spacing = 500f;
    [SerializeField] private float fixedY = 20f;
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private float unselectedScale = 0.7f;

    private int currentIndex = 0;
    private int totalCount;
    private Transform[] cards;

    private void OnEnable()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OpenPopupWithEffects("CHARACTER SELECT");
        }
    }

    private void OnDisable()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ClosePopupWithEffects();
        }
    }

    void Start()
    {
        if (container == null || characterDatas == null || characterDatas.Length == 0)
        {
            Debug.LogError("데이터나 컨테이너가 설정되지 않았습니다!");
            return;
        }

        totalCount = characterDatas.Length;
        cards = new Transform[totalCount];

        for (int i = 0; i < totalCount; i++)
        {
            cards[i] = container.GetChild(i);

            CharacterCard cardScript = cards[i].GetComponent<CharacterCard>();
            if (cardScript != null)
            {
                cardScript.SetCard(characterDatas[i]);
            }
        }

        UpdateUI(true);
    }

    public void OnNextButton()
    {
        if (currentIndex < totalCount - 1)
        {
            currentIndex++;
            UpdateUI(false);
        }
    }

    public void OnPrevButton()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateUI(false);
        }
    }

    private void UpdateUI(bool isImmediate)
    {
        float targetX = -currentIndex * spacing;
        container.DOKill();

        if (isImmediate)
            container.anchoredPosition = new Vector2(targetX, fixedY);
        else
            container.DOAnchorPos(new Vector2(targetX, fixedY), duration).SetEase(Ease.OutCubic);

        // 카드 스케일 조절
        for (int i = 0; i < totalCount; i++)
        {
            bool isSelected = i == currentIndex;
            Transform card = cards[i];
            card.DOKill(true);

            float targetScale = isSelected ? 1.0f : unselectedScale;

            if (isImmediate)
                card.localScale = Vector3.one * targetScale;
            else
                card.DOScale(targetScale, duration);
        }

        UpdateStats();
        UpdateButtonState();
    }

    private void UpdateStats()
    {
        if (characterDatas == null || characterDatas.Length <= currentIndex) return;

        CharacterData data = characterDatas[currentIndex];

        if (descriptionText != null) descriptionText.text = data.description;

        if (hpText != null) hpText.text = data.hp.ToString();
        if (atkText != null) atkText.text = data.atk.ToString();
        if (defText != null) defText.text = data.def.ToString();
        if (stamText != null) stamText.text = data.stam.ToString();
        if (thirstText != null) thirstText.text = data.thirst.ToString();
        if (hungerText != null) hungerText.text = data.hunger.ToString();
        if (sanityText != null) sanityText.text = data.sanity.ToString();
    }

    private void UpdateButtonState()
    {
        if (prevButton != null) prevButton.interactable = currentIndex > 0;
        if (nextButton != null) nextButton.interactable = currentIndex < totalCount - 1;
    }

    public void OnClickSelectButton()
    {
        Debug.Log("<color=yellow>선택 버튼 클릭됨!</color>");
        PlayerPrefs.SetInt("SelectedCharacter", currentIndex);
        PlayerPrefs.Save();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCharacter(characterDatas[currentIndex]);
        }

        gameObject.SetActive(false);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.LoadScene(Constants.ESceneType.Game);
        }
    }
}
