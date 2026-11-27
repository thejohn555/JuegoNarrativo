using Assets.Scripts.EventBuses.Events;
using EventBuses;
using System;
using System.Collections;
using UnityEngine;

public class BarkUI : FloatingTextUI
{
    [SerializeField] private CanvasGroup barkCanvasGroup;
    [SerializeField] private float visibleTimeAfterTyping = 2.5f;

    private Coroutine autoAdvanceCoroutine;
    private RuntimeDialogueGraph currentBarkGraph;
    private int currentNodeIndex;

    private bool isHiddenByDialogue;
    private Transform barkSpeakerTransform;
    private Action<BarkUI, Transform> onFinishedCallback;

    public Transform SpeakerTransform => barkSpeakerTransform;
    //protected override bool ShouldPlayTypingSound => !isHiddenByDialogue;
    protected override bool ShouldPlayTypingSound => false;

    public void Play(OnBarkEvent ev, bool startHidden, Action<BarkUI, Transform> onFinished)
    {
        StopRunningCoroutines();

        onFinishedCallback = onFinished;
        currentBarkGraph = ev.barkGraph;
        currentNodeIndex = 0;
        barkSpeakerTransform = ev.speakerTransform;
        activeSpeakerTransform = ev.speakerTransform;

        bodyText.color = ev.textColor;
        if (speakerText != null) speakerText.text = ev.speakerName;

        gameObject.SetActive(true);
        if (panel != null) panel.SetActive(true);
        SetVisibility(!startHidden);

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

    public void SetVisibility(bool visible)
    {
        isHiddenByDialogue = !visible;
        if (barkCanvasGroup != null)
        {
            barkCanvasGroup.alpha = visible ? 1f : 0f;
        }
        if (visible) UpdatePosition();
    }

    public void CancelBark()
    {
        StopRunningCoroutines();

        Transform finishedSpeaker = barkSpeakerTransform;
        activeSpeakerTransform = null;
        barkSpeakerTransform = null;
        currentBarkGraph = null;
        currentNodeIndex = 0;

        if (panel != null) panel.SetActive(false);

        var callback = onFinishedCallback;
        onFinishedCallback = null;
        callback?.Invoke(this, finishedSpeaker);
    }

    private void StopRunningCoroutines()
    {
        if (typeWritingCoroutine != null) StopCoroutine(typeWritingCoroutine);
        if (autoAdvanceCoroutine != null) StopCoroutine(autoAdvanceCoroutine);
        typeWritingCoroutine = null;
        autoAdvanceCoroutine = null;
    }
}