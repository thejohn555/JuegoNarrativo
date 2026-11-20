using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EventBuses;
using EventBuses;
using Interactives;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneGameManager:Singleton<SceneGameManager>
    {
        [SerializeField] EventBusSO eventBus;
        [SerializeField] NPC Kidnapper;
        void SwapScene(int index)
        {
            SceneManager.LoadScene(index);
        }

        private void OnEnable()
        {
            Debug.Log("Suscrito al acabar dialogo");
            eventBus.Subscribe<OnDialogueEndedEvent>(OnDialogueEnded);
        }

        private void OnDialogueEnded(OnDialogueEndedEvent obj)
        {
            Debug.Log("OnDialgue end");
            Debug.Log($"evento id: {obj.NpcID }, Kinapper NPC: {Kidnapper.npcID}");
            if ( obj.NpcID != Kidnapper.npcID) return;
            Debug.Log("Lanzamos corrutinas");
            StartCoroutine(CheckStateNextFrame());
        }

        private IEnumerator CheckStateNextFrame()
        {
            Debug.Log("Corrutina");
            yield return null;

            if (Kidnapper != null && Kidnapper.currentStateId == "KidnapperGiveAle")
            {
                Debug.Log("Cambio de escena");
                SwapScene(1);
            }
                
        }
    }
}