using Assets.Scripts.EventBuses;
using Assets.Scripts.EventBuses.Events;
using EventBuses;
using System.Collections;
using UnityEngine;

public class BarkUI : FloatingTextUI
{
    [SerializeField] private EventBusSO EventBus;
    [SerializeField] private CanvasGroup barkCanvasGroup;
    [SerializeField] private float visibleTimeAfterTyping = 2.5f;

    private Coroutine autoAdvanceCoroutine;
    private RuntimeDialogueGraph currentBarkGraph;
    private int currentNodeIndex;

    private bool isMainDialogueActive;
    private Transform barkSpeakerTransform;
    protected override bool ShouldPlayTypingSound => !isMainDialogueActive;

    private void OnEnable()
    {
        EventBus.Subscribe<OnBarkEvent>(OnBarkTriggered);
        EventBus.Subscribe<OnDialogueDataEvent>(OnMainDialogueStart);
        EventBus.Subscribe<OnDialogueEndedEvent>(OnMainDialogueEnd);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<OnBarkEvent>(OnBarkTriggered);
        EventBus.UnSubscribe<OnDialogueDataEvent>(OnMainDialogueStart);
        EventBus.UnSubscribe<OnDialogueEndedEvent>(OnMainDialogueEnd);
    }

    private void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    private void OnBarkTriggered(OnBarkEvent ev)
    {
        if (ev.barkGraph == null || ev.barkGraph.AllNodes == null || ev.barkGraph.AllNodes.Count == 0) return;
        if (ev.speakerTransform == null) return;

        if (isMainDialogueActive && activeSpeakerTransform == ev.speakerTransform) return;

        CancelBark();

        currentBarkGraph = ev.barkGraph;
        currentNodeIndex = 0;
        barkSpeakerTransform = ev.speakerTransform;
        activeSpeakerTransform = ev.speakerTransform;

        bodyText.color = ev.textColor;
        if (speakerText != null) speakerText.text = ev.speakerName;

        panel.SetActive(true);
        SetBarkVisibility(!isMainDialogueActive);

        Canvas.ForceUpdateCanvases();
        UpdatePosition();

        PlayCurrentNode();
    }

    private void PlayCurrentNode()
    {
        if (currentBarkGraph == null || currentNodeIndex >= currentBarkGraph.AllNodes.Count)
        {
            CancelBark();
            return;
        }

        RuntimeDialogueNode node = currentBarkGraph.AllNodes[currentNodeIndex];
        typeWritingCoroutine = StartCoroutine(TypeWritingRoutine(node.DialogueText, node.TextSpeed));
    }

    protected override void OnTypingCompleted()
    {
        autoAdvanceCoroutine = StartCoroutine(AutoAdvanceRoutine());
    }

    private IEnumerator AutoAdvanceRoutine()
    {
        yield return new WaitForSeconds(visibleTimeAfterTyping);

        currentNodeIndex++;
        PlayCurrentNode();
    }

    private void SetBarkVisibility(bool visible)
    {
        if (barkCanvasGroup != null)
        {
            barkCanvasGroup.alpha = visible ? 1f : 0f;
        }
    }

    public void ResetBark(int startNodeIndex)
    {
        if (currentBarkGraph == null || currentBarkGraph.AllNodes == null || currentBarkGraph.AllNodes.Count == 0)
        {
            CancelBark();
            return;
        }

        if (startNodeIndex < 0 || startNodeIndex >= currentBarkGraph.AllNodes.Count)
        {
            CancelBark();
            return;
        }

        if (typeWritingCoroutine != null)
        {
            StopCoroutine(typeWritingCoroutine);
            typeWritingCoroutine = null;
        }

        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        currentNodeIndex = startNodeIndex;

        if (panel != null && !panel.activeSelf)
        {
            panel.SetActive(true);
        }

        SetBarkVisibility(!isMainDialogueActive);
        UpdatePosition();

        PlayCurrentNode();
    }

    public void CancelBark()
    {
        if (typeWritingCoroutine != null) StopCoroutine(typeWritingCoroutine);
        if (autoAdvanceCoroutine != null) StopCoroutine(autoAdvanceCoroutine);
        typeWritingCoroutine = null;
        autoAdvanceCoroutine = null;

        if (panel != null) panel.SetActive(false);
        SetBarkVisibility(false);

        activeSpeakerTransform = null;
        barkSpeakerTransform = null;
        currentBarkGraph = null;
        currentNodeIndex = 0;
    }

    private void OnMainDialogueStart(OnDialogueDataEvent ev)
    {
        if (!ev.DialogueData.isDialogue) return;

        isMainDialogueActive = true;

        if (barkSpeakerTransform != null && ev.DialogueData.speakers != null)
        {
            foreach (var speaker in ev.DialogueData.speakers.Values)
            {
                if (speaker.transform == barkSpeakerTransform)
                {
                    CancelBark();
                    return;
                }
            }
        }

        if (currentBarkGraph != null)
        {
            SetBarkVisibility(false);
        }
    }

    private void OnMainDialogueEnd(OnDialogueEndedEvent ev)
    {
        isMainDialogueActive = false;

        if (currentBarkGraph != null && panel.activeSelf)
        {
            SetBarkVisibility(true);
            ResetBark(currentNodeIndex);
            UpdatePosition();
        }
    }
}