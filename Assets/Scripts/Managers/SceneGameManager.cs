using System;
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
            Debug.Log("Sale funcion");
            if (obj.NpcID != Kidnapper.npcID)
            {
                Debug.Log("No tiene mismo id");
                return;  
            }
               
            if (Kidnapper.currentStateId == "KidnapperGiveAle")
            {
                Debug.Log("Cambiando escena");
                SwapScene(1);
            }
            else
            {
                Debug.Log("No tiene mismo estado");
            }
        }
    }
}