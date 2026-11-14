using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.Managers
{
    public class TimeLineManager : MonoBehaviour
    {
        public static TimeLineManager Instance { get; private set; }

        [SerializeField] private Dictionary<string,PlayableDirector> directors;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public PlayableDirector GetDirector(string directorName)
        {
            return directors[directorName];
        }
    }
}