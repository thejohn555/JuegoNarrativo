using Assets.Scripts.EventBuses;
using System.Collections;
using EventBuses;
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
    [SerializeField] private GameObject choiceContainer;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Image choiceBackground;
    private RuntimeDialogueNode currentNode;
    private bool isWritingText;
    private string currentFullText;
    private Coroutine typeWritingCoroutine;
    private Vector3 defaultDialoguePanelPos;
    private Vector3 defaultAnchoredPosition;

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
        defaultAnchoredPosition = dialoguePanel.GetComponent<RectTransform>().anchoredPosition;
        //defaultDialoguePanelPos = dialoguePanel.transform.position;
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
        if (currentNode == null) return;

        if (isWritingText)
        {
            if (typeWritingCoroutine != null) StopCoroutine(typeWritingCoroutine);
            dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;
            isWritingText = false;

            // si el jugador skipea la animación, mostramos los botones de golpe
            GenerateChoices();
        }
        else
        {
            if (currentNode.Choices == null || currentNode.Choices.Count == 0)
            {
                DialogueManager.Instance.AdvanceToNextNode();
            }
        }
    }

    private void RecieveDialoguePositionData(OnDialogueCameraDataEvent ev)
    {
        RectTransform rect = dialoguePanel.GetComponent<RectTransform>();
        var data = ev.DialogueCameraData;

        if (data.textColor.a == 0)
            dialogueText.color = Color.white;
        else
            dialogueText.color = data.textColor;

        if (data.isDialogue)
        {
            Vector3 offset = new Vector3(
                data.dialogueTextOffset.x,
                data.dialogueTextOffset.y,
                0
            );
            rect.anchoredPosition = defaultAnchoredPosition + offset;
        }
        else
        {
            if (data.targetPosition != null)
            {
                Vector3 worldTargetPos = data.targetPosition.position + data.dialogueTextOffset;
                Vector2 screenPos = Camera.main.WorldToScreenPoint(worldTargetPos);
                RectTransform parentRect = rect.parent.GetComponent<RectTransform>();

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    screenPos,
                    null,
                    out Vector2 canvasLocalPos
                );

                rect.localPosition = canvasLocalPos;
            }
        }
    }

    private void OnNodeDisplayed(OnNodeDisplayedEvent ev)
    {
        if (typeWritingCoroutine != null) StopCoroutine(typeWritingCoroutine);
        currentNode = ev.Node;
        speakerText.text = ev.Node.SpeakerName;

        ClearChoices();

        typeWritingCoroutine = StartCoroutine(TypeWrittingText(ev.Node.DialogueText, ev.Node.TextSpeed));
    }

    private void ClearChoices()
    {
        if (choiceContainer != null) choiceContainer.SetActive(false);

        if (buttonContainer != null)
        {
            foreach (Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void GenerateChoices()
    {
        if (currentNode.Choices == null || currentNode.Choices.Count == 0) return;

        if (choiceContainer != null) choiceContainer.SetActive(true);

        foreach (var choice in currentNode.Choices)
        {
            Button btn = Instantiate(choiceButtonPrefab, buttonContainer);

            TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();

            if (btnText != null) btnText.SetText(choice.ChoiceText); 

            btn.onClick.AddListener(() =>
            {
                ClearChoices();
                DialogueManager.Instance.DisplayNode(choice.DestinationNodeID);
            });
        }
    }

    private IEnumerator TypeWrittingText(string fullText, float lettersPerSecond)
    {
        isWritingText = true;
        currentFullText = fullText;

        dialogueText.SetText(fullText);

        dialogueText.maxVisibleCharacters = 0;

        yield return null;

        int totalCharacters = dialogueText.textInfo.characterCount;
        float delay = 1f / lettersPerSecond;

        for (int i = 0; i <= totalCharacters; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(delay);
        }

        isWritingText = false;
        GenerateChoices();
    }
}
