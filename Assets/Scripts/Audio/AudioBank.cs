using Audio.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class AudioBank : ScriptableObject
{
    [SerializeField] private List<AudioClip> audios;
    [SerializeField] private SoundOptions soundOptions;
    [SerializeField]private VarOptions varOptions;
    [SerializeField] private EnumOptions enumOptions;
    [SerializeField] private bool loop;
    

    public void Play()
    {
        switch(soundOptions)
        {

        }
    }
}
