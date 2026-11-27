using Assets.Scripts.EventBuses;
using Assets.Scripts.EventBuses.Events;
using EventBuses;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BarkSpawner : MonoBehaviour
{
    [SerializeField] private EventBusSO EventBus;
    [SerializeField] private BarkUI barkPrefab;
    [SerializeField] private int defaultCapacity = 4;
    [SerializeField] private int maxPoolSize = 15;

    private IObjectPool<BarkUI> barkPool;
    private readonly Dictionary<Transform, BarkUI> activeBarks = new Dictionary<Transform, BarkUI>();
    private bool isMainDialogueActive;

    private void Awake()
    {
        barkPool = new ObjectPool<BarkUI>(
            createFunc: CreateBarkInstance,
            actionOnGet: OnGetBarkFromPool,
            actionOnRelease: OnReleaseBarkToPool,
            actionOnDestroy: OnDestroyBarkInstance,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxPoolSize
        );

        EventBus.Subscribe<OnBarkEvent>(OnBarkTriggered);
        EventBus.Subscribe<OnDialogueDataEvent>(OnMainDialogueStart);
        EventBus.Subscribe<OnDialogueEndedEvent>(OnMainDialogueEnd);
    }

    private void OnDestroy()
    {
        EventBus.UnSubscribe<OnBarkEvent>(OnBarkTriggered);
        EventBus.UnSubscribe<OnDialogueDataEvent>(OnMainDialogueStart);
        EventBus.UnSubscribe<OnDialogueEndedEvent>(OnMainDialogueEnd);

        barkPool?.Clear();
    }

    private BarkUI CreateBarkInstance()
    {
        return Instantiate(barkPrefab, transform);
    }

    private void OnGetBarkFromPool(BarkUI bark)
    {
        bark.gameObject.SetActive(true);
    }

    private void OnReleaseBarkToPool(BarkUI bark)
    {
        bark.gameObject.SetActive(false);
    }

    private void OnDestroyBarkInstance(BarkUI bark)
    {
        if (bark != null)
        {
            Destroy(bark.gameObject);
        }
    }

    private void OnBarkTriggered(OnBarkEvent ev)
    {
        if (ev.barkGraph == null || ev.barkGraph.AllNodes == null || ev.barkGraph.AllNodes.Count == 0) return;
        if (ev.speakerTransform == null) return;

        if (activeBarks.TryGetValue(ev.speakerTransform, out BarkUI existingBark))
        {
            existingBark.Play(ev, isMainDialogueActive, HandleBarkFinished);
            return;
        }

        BarkUI newBark = barkPool.Get();
        activeBarks[ev.speakerTransform] = newBark;
        newBark.Play(ev, isMainDialogueActive, HandleBarkFinished);
    }

    private void HandleBarkFinished(BarkUI bark, Transform speaker)
    {
        if (speaker != null && activeBarks.TryGetValue(speaker, out BarkUI current) && current == bark)
        {
            activeBarks.Remove(speaker);
        }

        barkPool.Release(bark);
    }

    private void OnMainDialogueStart(OnDialogueDataEvent ev)
    {
        if (!ev.DialogueData.isDialogue) return;
        isMainDialogueActive = true;

        var speakers = new List<Transform>(activeBarks.Keys);
        foreach (Transform speaker in speakers)
        {
            if (!activeBarks.TryGetValue(speaker, out BarkUI bark)) continue;

            bool isTargetSpeaker = false;
            if (ev.DialogueData.speakers != null)
            {
                foreach (var data in ev.DialogueData.speakers.Values)
                {
                    if (data.transform == speaker)
                    {
                        isTargetSpeaker = true;
                        break;
                    }
                }
            }

            if (isTargetSpeaker)
                bark.CancelBark();
            else
                bark.SetVisibility(false);
        }
    }

    private void OnMainDialogueEnd(OnDialogueEndedEvent ev)
    {
        isMainDialogueActive = false;

        foreach (BarkUI bark in activeBarks.Values)
        {
            bark.SetVisibility(true);
        }
    }
}