using System.Collections;
using TMPro;
using UnityEngine;

public abstract class FloatingTextUI : MonoBehaviour
{
    [Header("Referencias Base")]
    [SerializeField] protected GameObject panel;
    [SerializeField] protected TMP_Text speakerText;
    [SerializeField] protected TMP_Text bodyText;

    [Header("Pausas de Puntuación")]
    [SerializeField] protected float shortPauseDuration = 0.15f;
    [SerializeField] protected float longPauseDuration = 0.35f;

    protected Transform activeSpeakerTransform;
    protected Coroutine typeWritingCoroutine;
    protected bool isWritingText;
    protected virtual bool ShouldPlayTypingSound => true;

    protected virtual void Update()
    {
        if (panel != null && panel.activeSelf)
        {
            UpdatePosition();
        }
    }

    protected virtual void UpdatePosition()
    {
        if (panel == null || !panel.activeSelf || activeSpeakerTransform == null) return;

        RectTransform rect = panel.GetComponent<RectTransform>();
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

    protected Vector3 GetSpeakerTopPosition(Transform speakerTransform)
    {
        SpriteRenderer sprite = speakerTransform.GetComponentInChildren<SpriteRenderer>();

        if (sprite != null)
        {
            return new Vector3(speakerTransform.position.x, sprite.bounds.max.y + 0.8f, speakerTransform.position.z);
        }

        return speakerTransform.position + Vector3.up * 2f;
    }

    protected IEnumerator TypeWritingRoutine(string fullText, float lettersPerSecond)
    {
        int contador = 0;
        isWritingText = true;

        bodyText.SetText(fullText);
        bodyText.maxVisibleCharacters = 0;

        yield return null;

        while (!panel.activeInHierarchy)
        {
            yield return null;
        }

        bodyText.ForceMeshUpdate();

        int totalCharacters = bodyText.textInfo.characterCount;
        float baseDelay = lettersPerSecond > 0f ? 1f / lettersPerSecond : 0.02f;

        for (int i = 0; i <= totalCharacters; i++)
        {
            contador++;
            bodyText.maxVisibleCharacters = i;

            if (contador % 3 == 0 && AudioManager.Instance != null)
            {
                //audio
                contador = 0;
            }

            float currentDelay = baseDelay;

            if (i > 0 && i < totalCharacters)
            {
                char currentChar = bodyText.textInfo.characterInfo[i - 1].character;
                char nextChar = bodyText.textInfo.characterInfo[i].character;

                if (!IsPunctuationPause(nextChar))
                {
                    currentDelay += GetPunctuationPause(currentChar);
                }
            }

            yield return new WaitForSeconds(currentDelay);
        }

        isWritingText = false;
        OnTypingCompleted();
    }

    protected virtual void OnTypingCompleted() { }

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
                return shortPauseDuration;
            case '.':
            case '!':
            case '?':
                return longPauseDuration;
            default:
                return 0f;
        }
    }
}