using System;
using Blueprints_arquitectura_Dani_.Runtime;
using EventBuses;
using UnityEngine;

public class BlueprintsEventsOAD : MonoBehaviour
{
    public static  BlueprintsEventsOAD Instance { get;private set; }
    [SerializeField] EventBusSO eventBus;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        BlueprintsEventManager.Instance.ChangeNpcState += NpcChangeState;
    }

    private void OnDisable()
    {
        BlueprintsEventManager.Instance.ChangeNpcState -= NpcChangeState;
    }

    public void NpcChangeState(string npc_id, string state)
    {
        Debug.Log("NpcChangeState");
        eventBus.Publish(new OnNPCStateChangedEvent(npc_id, state));
    }
}
