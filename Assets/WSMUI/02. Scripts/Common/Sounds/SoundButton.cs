using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class SoundButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private string sfxKey = "button_click";
    [SerializeField] private string hoverSfxKey = "button_hover";

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
            SoundManager.Instance.PlaySFX(sfxKey));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(hoverSfxKey);
    }
}