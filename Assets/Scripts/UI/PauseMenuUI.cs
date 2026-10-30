using Assets.Scripts.EventBuses;
using EventBuses;
using UnityEngine;

public class PauseMenuUI : Singleton<PauseMenuUI>
{
    [SerializeField] private EventBusSO EventBus;
    [SerializeField] private GameObject container;

    [Header("Configuración de la Espada")]
    [SerializeField] private RectTransform swordIndicator;
    [SerializeField] private PauseMenuOptionButtons defaultButton;
    [SerializeField] private float transitionSpeed = 15f;

    private PauseMenuOptionButtons currentButton;
    private bool isPaused;

    private void OnEnable()
    {
        EventBus.Subscribe<OnEscPressedEvent>(HandleMenu);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<OnEscPressedEvent>(HandleMenu);
    }

    private void Start()
    {
        container.SetActive(false); 
    }

    private void Update()
    {
        if (currentButton != null && swordIndicator != null && container.activeSelf)
        { 
            Vector3 targetPos = new Vector3(swordIndicator.position.x, currentButton.rectTransform.position.y, swordIndicator.position.z);
            swordIndicator.position = Vector3.Lerp(swordIndicator.position, targetPos, Time.unscaledDeltaTime * transitionSpeed);
        }
    }

    public void SetSwordTarget(PauseMenuOptionButtons newButton)
    {
        if (currentButton != null)
        {
            currentButton.SetTextColor(Color.white);
        }

        currentButton = newButton;

        if (currentButton != null)
        {
            currentButton.SetTextColor(Color.black);
        }
    }

    public void HandleMenu(OnEscPressedEvent ev)
    {
        if (!isPaused)
        {
            isPaused = true;
            PauseMenu();
        }
        else
        {
            isPaused = false;
        }
        

        container.SetActive(isPaused);
    }

    public void PauseMenu()
    {
        if (defaultButton != null)
        {
            SetSwordTarget(defaultButton);
        }
    }

    public void UnPauseMenu()
    {
        isPaused = false;
        container.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}