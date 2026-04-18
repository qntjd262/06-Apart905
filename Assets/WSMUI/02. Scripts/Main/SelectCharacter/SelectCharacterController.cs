using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class SelectCharacterController : MonoBehaviour
{
    [Header("Data Source")]
    private List<CharacterStatSO> characterDatas = new List<CharacterStatSO>();

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
    private CharacterCard[] cardScripts; // 추가

    private void OnEnable()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.OpenPopupWithEffects("캐릭터 선택");
        currentIndex = 0;
        UpdateUI(true);
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
        /*
        if (container == null || characterDatas == null || characterDatas.Length == 0)
        {
            Debug.LogError("데이터나 컨테이너가 설정되지 않았습니다!");
            return;
        }
        */
        if (CharacterDataManager.Instance != null && CharacterDataManager.Instance.characterDB.Count > 0)
        {
            characterDatas = CharacterDataManager.Instance.characterDB.Values.ToList();
        }
        else
        {
            Debug.LogError("캐릭터 데이터가 아직 구글에서 로드 되지 않았음");
            return;
        }


        //아래는 기존 코드 동일 characterDatas.Length -> characterDatas.Count
        totalCount = characterDatas.Count;
        cards = new Transform[totalCount];
        cardScripts = new CharacterCard[totalCount]; // 이 줄


        for (int i = 0; i < totalCount; i++)
        {
            cards[i] = container.GetChild(i);
            cardScripts[i] = cards[i].GetComponent<CharacterCard>(); // 추가

            if (cardScripts[i] != null)
                cardScripts[i].SetCard(characterDatas[i]);
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
        if (cards == null || totalCount == 0) return; // 추가
        float targetX = -currentIndex * spacing;
        container.DOKill();

        if (isImmediate)
            container.anchoredPosition = new Vector2(targetX, fixedY);
        else
            container.DOAnchorPos(new Vector2(targetX, fixedY), duration).SetEase(Ease.OutCubic);

        for (int i = 0; i < totalCount; i++)
        {
            Transform card = cards[i];
            card.DOKill(true);

            float targetScale = i == currentIndex ? 1.0f : unselectedScale;
            float targetBrightness = Mathf.Clamp01(1f - Mathf.Abs(i - currentIndex) * 0.8f);

            if (isImmediate)
                card.localScale = Vector3.one * targetScale;
            else
                card.DOScale(targetScale, duration);

            cardScripts?[i]?.SetDim(targetBrightness, duration, isImmediate);
        }
        UpdateStats();
        UpdateButtonState();
    }

    private void UpdateStats()
    {
        //characterDatas.Length -> characterDatas.Count
        if (characterDatas == null || characterDatas.Count <= currentIndex) return;


        //CharacterData -> CharacterStatSO
        CharacterStatSO data = characterDatas[currentIndex];


        if (descriptionText != null) descriptionText.text = $"Description : {data.description}";


        //data.뒤에 변수명 수정
        if (hpText != null) hpText.text = $"HP : {data.Hp}";
        if (atkText != null) atkText.text = $"Attack : {data.AttackPower}";
        if (defText != null) defText.text = $"Def : {data.Def}";
        if (stamText != null) stamText.text = $"Stamina : {data.MaxStamina}";
        if (thirstText != null) thirstText.text = $"Thirst : {data.ThirstDecreaseRate}";
        if (hungerText != null) hungerText.text = $"Hunger : {data.HungerDecreaseRate}";
        if (sanityText != null) sanityText.text = $"Infection : {data.InfectionIncreaseRate}";

    }

    private void UpdateButtonState()
    {
        if (prevButton != null) prevButton.interactable = currentIndex > 0;
        if (nextButton != null) nextButton.interactable = currentIndex < totalCount - 1;
    }

    public void OnClickSelectButton()
    {
        if (CharacterDataManager.Instance != null)
        {
            CharacterDataManager.Instance.selectedCharacterSO = characterDatas[currentIndex];
            Debug.Log($"선택된 캐릭터는 {characterDatas[currentIndex].Name}입니다.");
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.CloseSelectCharacterPanel();
            UIManager.Instance.LoadScene(Constants.ESceneType.Game);
        }

    }
}
