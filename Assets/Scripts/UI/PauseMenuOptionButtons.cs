using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuOptionButtons : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private TMP_Text text;
    private PauseMenuUI pauseMenu;
    public RectTransform rectTransform { get; private set; }

    private void Awake()
    {
        pauseMenu = GetComponentInParent<PauseMenuUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetSwordTarget(this);
        }
    }

    public void SetTextColor(Color color)
    {
        if (text != null)
        {
            text.color = color;
        }
    }
}