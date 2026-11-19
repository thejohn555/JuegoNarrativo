using Assets.Scripts.Camera;
using Assets.Scripts.EventBuses;
using EventBuses;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : FloatingTextUI
{
    [SerializeField] private EventBusSO EventBus;
    [SerializeField] private Button choiceButtonPrefab;
    [SerializeField] private GameObject choiceContainer;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Image choiceBackground;

    private RuntimeDialogueNode currentNode;
    private string currentFullText;
    private Coroutine autoCloseCoroutine;
    private Vector3 defaultAnchoredPosition;

    private bool isDialogueMode;
    private Dictionary<string, SpeakerData> currentSpeakers;

    private void OnEnable()
    {
        EventBus.Subscribe<OnDialogueDataEvent>(RecieveDialoguePositionData);
        EventBus.Subscribe<OnNodeDisplayedEvent>(OnNodeDisplayed);
        EventBus.Subscribe<OnDialogueStartedEvent>(ShowPanel);
        EventBus.Subscribe<OnDialogueEndedEvent>(HidePanel);
        EventBus.Subscribe<OnSkipDialogueEvent>(NextNode);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<OnDialogueDataEvent>(RecieveDialoguePositionData);
        EventBus.UnSubscribe<OnNodeDisplayedEvent>(OnNodeDisplayed);
        EventBus.UnSubscribe<OnDialogueStartedEvent>(ShowPanel);
        EventBus.UnSubscribe<OnDialogueEndedEvent>(HidePanel);
        EventBus.UnSubscribe<OnSkipDialogueEvent>(NextNode);
    }

    private void Awake()
    {
        defaultAnchoredPosition = panel.GetComponent<RectTransform>().anchoredPosition;
    }

    private void ShowPanel(OnDialogueStartedEvent ev)
    {
        panel.SetActive(true);
    }

    private void HidePanel(OnDialogueEndedEvent ev)
    {
        panel.SetActive(false);
    }

    private void NextNode(OnSkipDialogueEvent ev)
    {
        if (currentNode == null) return;

        if (!isDialogueMode) return;

        if (isWritingText)
        {
            if (typeWritingCoroutine != null) StopCoroutine(typeWritingCoroutine);
            bodyText.maxVisibleCharacters = bodyText.textInfo.characterCount;
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
        var data = ev.DialogueData;

        isDialogueMode = data.isDialogue;
        currentSpeakers = data.speakers;

        if (currentNode != null)
        {
            UpdateActiveSpeaker(currentNode.SpeakerName);
            UpdatePosition();
        }
    }

    private void OnNodeDisplayed(OnNodeDisplayedEvent ev)
    {
        if (typeWritingCoroutine != null) StopCoroutine(typeWritingCoroutine);
        if (autoCloseCoroutine != null) StopCoroutine(autoCloseCoroutine);

        currentNode = ev.Node;
        speakerText.text = ev.Node.SpeakerName;

        UpdateActiveSpeaker(ev.Node.SpeakerName);
        UpdatePosition();

        ClearChoices();
        typeWritingCoroutine = StartCoroutine(TypeWritingRoutine(ev.Node.DialogueText, ev.Node.TextSpeed));
    }

    private void UpdateActiveSpeaker(string speakerName)
    {
        if (string.IsNullOrEmpty(speakerName) || currentSpeakers == null)
        {
            activeSpeakerTransform = null;
            bodyText.color = Color.white;
            return;
        }

        if (currentSpeakers.TryGetValue(speakerName, out SpeakerData speakerData))
        {
            activeSpeakerTransform = speakerData.transform;
            bodyText.color = speakerData.textColor;
            return;
        }

        string trimmed = speakerName.Trim();
        foreach (var pair in currentSpeakers)
        {
            if (string.Equals(pair.Key.Trim(), trimmed, System.StringComparison.OrdinalIgnoreCase))
            {
                activeSpeakerTransform = pair.Value.transform;
                bodyText.color = pair.Value.textColor;
                return;
            }
        }

        activeSpeakerTransform = null;
        bodyText.color = Color.white;
    }

    protected override void Update()
    {
        if (!panel.activeSelf) return;

        if (activeSpeakerTransform == null && currentNode != null)
        {
            UpdateActiveSpeaker(currentNode.SpeakerName);
        }

        UpdatePosition();
    }

    protected override void UpdatePosition()
    {
        if (activeSpeakerTransform != null)
        {
            base.UpdatePosition();
        }
        else if (panel != null && panel.activeSelf)
        {
            panel.GetComponent<RectTransform>().anchoredPosition = defaultAnchoredPosition;
        }
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

    protected override void OnTypingCompleted()
    {
        GenerateChoices();
    }
}