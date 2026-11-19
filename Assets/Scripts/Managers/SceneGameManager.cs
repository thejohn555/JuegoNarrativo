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
           
            if (obj.NpcID != Kidnapper.npcID)
            {
                
                return;  
            }
               
            if (Kidnapper.currentStateId == "KidnapperGiveAle")
            {
                
                SwapScene(1);
            }
            else
            {
                
            }
        }
    }
}