using Assets.Scripts.Camera;
using Assets.Scripts.EventBuses;
using EventBuses;
using System.Collections;
using System.Collections.Generic;
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
    private Coroutine autoCloseCoroutine;
    private Vector3 defaultAnchoredPosition;

    private bool isDialogueMode;
    private Dictionary<string, SpeakerData> currentSpeakers;
    private Transform activeSpeakerTransform;

    private void OnEnable()
    {
        EventBus.Subscribe<OnNodeDisplayedEvent>(OnNodeDisplayed);
        EventBus.Subscribe<OnDialogueStartedEvent>(ShowPanel);
        EventBus.Subscribe<OnDialogueEndedEvent>(HidePanel);
        EventBus.Subscribe<OnSkipDialogueEvent>(NextNode);
        EventBus.Subscribe<OnDialogueDataEvent>(RecieveDialoguePositionData);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<OnNodeDisplayedEvent>(OnNodeDisplayed);
        EventBus.UnSubscribe<OnDialogueStartedEvent>(ShowPanel);
        EventBus.UnSubscribe<OnDialogueEndedEvent>(HidePanel);
        EventBus.UnSubscribe<OnSkipDialogueEvent>(NextNode);
        EventBus.UnSubscribe<OnDialogueDataEvent>(RecieveDialoguePositionData);
    }

    private void Awake()
    {
        defaultAnchoredPosition = dialoguePanel.GetComponent<RectTransform>().anchoredPosition;
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

    private void RecieveDialoguePositionData(OnDialogueDataEvent ev)
    {
        var data = ev.DialogueCameraData;

        isDialogueMode = data.isDialogue;
        currentSpeakers = data.speakers;
    }

    private void OnNodeDisplayed(OnNodeDisplayedEvent ev)
    {
        if (typeWritingCoroutine != null) StopCoroutine(typeWritingCoroutine);
        if (autoCloseCoroutine != null) StopCoroutine(autoCloseCoroutine);

        currentNode = ev.Node;
        speakerText.text = ev.Node.SpeakerName;

        if (currentSpeakers != null && currentSpeakers.TryGetValue(ev.Node.SpeakerName, out SpeakerData speakerData))
        {
            activeSpeakerTransform = speakerData.transform;
            dialogueText.color = speakerData.textColor;
        }
        else
        {
            activeSpeakerTransform = null;
            dialogueText.color = Color.white;
        }

        ClearChoices();
        typeWritingCoroutine = StartCoroutine(TypeWrittingText(ev.Node.DialogueText, ev.Node.TextSpeed));
    }

    private void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        RectTransform rect = dialoguePanel.GetComponent<RectTransform>();

        if (activeSpeakerTransform != null)
        {
            Vector3 worldTargetPos = GetSpeakerTopPosition(activeSpeakerTransform);
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
        else
        {
            rect.anchoredPosition = defaultAnchoredPosition;
        }
    }

    private Vector3 GetSpeakerTopPosition(Transform speakerTransform)
    {
        SpriteRenderer sprite = speakerTransform.GetComponentInChildren<SpriteRenderer>();

        if (sprite != null)
        {
            return new Vector3(speakerTransform.position.x, sprite.bounds.max.y + 0.8f, speakerTransform.position.z);
        }

        return speakerTransform.position + Vector3.up * 2f;
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
        int contador = 0;
        isWritingText = true;
        currentFullText = fullText;

        dialogueText.SetText(fullText);
        dialogueText.maxVisibleCharacters = 0;

        yield return null;

        int totalCharacters = dialogueText.textInfo.characterCount;
        float delay = 1f / lettersPerSecond;

        for (int i = 0; i <= totalCharacters; i++)
        {
            contador++;
            dialogueText.maxVisibleCharacters = i;
            if (contador % 3 == 0)
            {
                contador = 0;
            }
            yield return new WaitForSeconds(delay);
        }

        isWritingText = false;
        GenerateChoices();

        if (!isDialogueMode && (currentNode.Choices == null || currentNode.Choices.Count == 0))
        {
            autoCloseCoroutine = StartCoroutine(AutoCloseMonologue());
        }
    }

    private IEnumerator AutoCloseMonologue()
    {
        yield return new WaitForSeconds(2.5f);
        DialogueManager.Instance.AdvanceToNextNode();
    }
}