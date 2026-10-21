using Assets.Scripts.EventBuses;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    private bool isWritingText;
    private string currentFullText;
    private Coroutine typeWritingCoroutine;

    private void OnEnable()
    {
        //Felipe
        EventBus.Instance.Subscribe<OnNodeDisplayedEvent>(OnNodeDisplayed);
         EventBus.Instance.Subscribe<OnDialogueStartedEvent>(ShowPanel);
         EventBus.Instance.Subscribe<OnDialogueEndedEvent>(HidePanel);
         EventBus.Instance.Subscribe<OnSkipDialogueEvent>(NextNode);
    }

    private void OnDisable()
    {
        //Felipe
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
