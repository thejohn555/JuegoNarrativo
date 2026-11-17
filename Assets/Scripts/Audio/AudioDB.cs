using Audio.Enums;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [CreateAssetMenu(fileName ="AudioDB", menuName = "Scriptable Objects/Audio Scriptable Object/AudioDB")]
    public class AudioDB : ScriptableObject
    {
        [SerializeField] private Dictionary<SFXID, AudioClip> sfxList;
        [SerializeField] private Dictionary<MusicID, AudioClip> musicList;

        public Dictionary<SFXID, AudioClip> SfxList
        {
            get
            {
                return sfxList;
            }
        }

        public Dictionary<MusicID, AudioClip> MusicList
        {
            get
            {
                return musicList;
            }
        }
    }
