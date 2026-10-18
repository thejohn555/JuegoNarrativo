using Assets.Scripts.EventBuses;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    private bool isWritingText;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<OnNodeDisplayedEvent>(OnNodeDisplayed);
        EventBus.Instance.Subscribe<OnDialogueStartedEvent>(ShowPanel);
        EventBus.Instance.Subscribe<OnDialogueEndedEvent>(HidePanel);
        EventBus.Instance.Subscribe<OnSkipDialogueEvent>(NextNode);
    }

    private void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnNodeDisplayedEvent>(OnNodeDisplayed);
        EventBus.Instance.UnSubscribe<OnDialogueStartedEvent>(ShowPanel);
        EventBus.Instance.UnSubscribe<OnDialogueEndedEvent>(HidePanel);
        EventBus.Instance.UnSubscribe<OnSkipDialogueEvent>(NextNode);
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
        }
        else
        {
            //mostrar siguiente nodo
            DialogueManager.Instance.AdvanceToNextNode();
        }
    }

    private void OnNodeDisplayed(OnNodeDisplayedEvent ev)
    {
        speakerText.text = ev.Node.SpeakerName;
        dialogueText.text = ev.Node.DialogueText;
    }
}
