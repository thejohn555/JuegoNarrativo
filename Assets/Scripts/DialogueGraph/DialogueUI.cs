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

    [SerializeField] private float shortPauseDelay = 0.3f;
    [SerializeField] private float longPauseDelay = 0.5f;

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

        if (!isDialogueMode) return;

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
        typeWritingCoroutine = StartCoroutine(TypeWrittingText(ev.Node.DialogueText, ev.Node.TextSpeed));
    }

    private void UpdateActiveSpeaker(string speakerName)
    {
        if (string.IsNullOrEmpty(speakerName) || currentSpeakers == null)
        {
            activeSpeakerTransform = null;
            dialogueText.color = Color.white;
            return;
        }

        if (currentSpeakers.TryGetValue(speakerName, out SpeakerData speakerData))
        {
            activeSpeakerTransform = speakerData.transform;
            dialogueText.color = speakerData.textColor;
            return;
        }

        string trimmed = speakerName.Trim();
        foreach (var pair in currentSpeakers)
        {
            if (string.Equals(pair.Key.Trim(), trimmed, System.StringComparison.OrdinalIgnoreCase))
            {
                activeSpeakerTransform = pair.Value.transform;
                dialogueText.color = pair.Value.textColor;
                return;
            }
        }

        activeSpeakerTransform = null;
        dialogueText.color = Color.white;
    }

    private void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        if (activeSpeakerTransform == null && currentNode != null)
        {
            UpdateActiveSpeaker(currentNode.SpeakerName);
        }

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (dialoguePanel == null || !dialoguePanel.activeSelf) return;

        RectTransform rect = dialoguePanel.GetComponent<RectTransform>();

        if (activeSpeakerTransform != null)
        {
            Vector3 worldTargetPos = GetSpeakerTopPosition(activeSpeakerTransform);
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldTargetPos);
            RectTransform parentRect = rect.parent.GetComponent<RectTransform>();

            if (parentRect != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    screenPos,
                    null,
                    out Vector2 canvasLocalPos
                );

                rect.localPosition = canvasLocalPos;
            }
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

        while (!dialoguePanel.activeInHierarchy)
        {
            yield return null;
        }

        dialogueText.ForceMeshUpdate();

        int totalCharacters = dialogueText.textInfo.characterCount;
        float delay = 1f / lettersPerSecond;

        for (int i = 0; i <= totalCharacters; i++)
        {
            contador++;
            dialogueText.maxVisibleCharacters = i;
            if (contador % 3 == 0)
            {
                AudioManager.Instance.PlayDialogueSound();
                contador = 0;
            }

            float currentDelay = delay;

            if (i > 0 && i < totalCharacters)
            {
                char currentChar = dialogueText.textInfo.characterInfo[i - 1].character;
                char nextChar = dialogueText.textInfo.characterInfo[i].character;

                if (!IsPunctuationPause(nextChar))
                {
                    currentDelay += GetPunctuationPause(currentChar);
                }
            }

            yield return new WaitForSeconds(currentDelay);
        }

        isWritingText = false;
        GenerateChoices();

        if (!isDialogueMode && (currentNode.Choices == null || currentNode.Choices.Count == 0))
        {
            autoCloseCoroutine = StartCoroutine(AutoCloseMonologue());
        }
    }

    private bool IsPunctuationPause(char c)
    {
        return c == ',' || c == ';' || c == ':' || c == '.' || c == '!' || c == '?';
    }

    private float GetPunctuationPause(char c)
    {
        switch (c)
        {
            case ',':
            case ';':
            case ':':
                return shortPauseDelay;
            case '.':
            case '!':
            case '?':
                return longPauseDelay;
            default:
                return 0f;
        }
    }

    private IEnumerator AutoCloseMonologue()
    {
        yield return new WaitForSeconds(2.5f);
        DialogueManager.Instance.AdvanceToNextNode();
    }
}