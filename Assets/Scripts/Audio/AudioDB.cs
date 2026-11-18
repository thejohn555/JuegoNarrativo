using Audio.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDB", menuName = "Scriptable Objects/Audio Scriptable Object/AudioDB")]
public class AudioDB : ScriptableObject
{
    [SerializeField] private Dictionary<SFXID, AudioClip> sfxList;
    [SerializeField] private Dictionary<MusicID, AudioClip> musicList;
    [Header("For now you will have to select the enum options by int")]
    [Header("Remember to start by 0")]
    [SerializeField] private Dictionary<AudioBankID,Dictionary<int, List<AudioClip>>> audioBanks;

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

    public Dictionary<AudioBankID, Dictionary<int, List<AudioClip>>> AudioBanks 
    {
        get
        {
            return audioBanks;
        }
    }
}