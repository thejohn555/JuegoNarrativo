using Assets.Scripts.EventBuses;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private EventBusSO EventBus;
    [SerializeField] private Button choiceButtonPrefab;
    [SerializeField] private Transform choiceContainer;
    [SerializeField] private Image choiceBackground;
    private bool isWritingText;
    private string currentFullText;
    private Coroutine typeWritingCoroutine;
    private Vector3 defaultDialoguePanelPos;

    private void OnEnable()
    {
        EventBus.Subscribe<OnNodeDisplayedEvent>(OnNodeDisplayed);
        EventBus.Subscribe<OnDialogueStartedEvent>(ShowPanel);
        EventBus.Subscribe<OnDialogueEndedEvent>(HidePanel);
        EventBus.Subscribe<OnSkipDialogueEvent>(NextNode);
        EventBus.Subscribe<OnDialogueCameraDataEvent>(RecieveDialoguePositionData);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<OnNodeDisplayedEvent>(OnNodeDisplayed);
        EventBus.UnSubscribe<OnDialogueStartedEvent>(ShowPanel);
        EventBus.UnSubscribe<OnDialogueEndedEvent>(HidePanel);
        EventBus.UnSubscribe<OnSkipDialogueEvent>(NextNode);
        EventBus.UnSubscribe<OnDialogueCameraDataEvent>(RecieveDialoguePositionData);
    }

    private void Awake()
    {
        defaultDialoguePanelPos = dialoguePanel.transform.position;
    }

    private void ShowPanel(OnDialogueStartedEvent ev)
    {
        dialoguePanel.SetActive(true);
    }

    private void HidePanel(OnDialogueEndedEvent ev)
    {
        dialoguePanel.SetActive(false);
    }

    private void NextNode(OnSkipDialogueEvent ev)
    {
        if (isWritingText)
        { 
            //completar animacion del tiron
            if (typeWritingCoroutine != null) StopCoroutine(typeWritingCoroutine);
            dialogueText.SetText(currentFullText);
            isWritingText = false;
        }
        else
        {
            //mostrar siguiente nodo
            DialogueManager.Instance.AdvanceToNextNode();
        }
    }

    private void RecieveDialoguePositionData(OnDialogueCameraDataEvent ev)
    {
        dialoguePanel.transform.position = defaultDialoguePanelPos;
        dialoguePanel.transform.position += ev.DialogueCameraData.dialogueTextOffset;
    }

    private void OnNodeDisplayed(OnNodeDisplayedEvent ev)
    {
        if (typeWritingCoroutine != null) StopCoroutine(typeWritingCoroutine);
        speakerText.text = ev.Node.SpeakerName;
        typeWritingCoroutine = StartCoroutine(TypeWrittingText(ev.Node.DialogueText, ev.Node.TextSpeed));
    }

    private IEnumerator TypeWrittingText(string fullText, float lettersPerSecond)
    {
        // iterar cuantas letras por segundo hay
        isWritingText = true;
        currentFullText = fullText;
        dialogueText.SetText("");

        float delay = 1 / lettersPerSecond;

        foreach (char letter in fullText)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(delay);
        }

        isWritingText = false;
    }
}
