using Audio.Enums;
using EventBuses;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioEvent", menuName = "Scriptable Objects/Audio Scriptable Object/AudioEvent")]
public class AudioEvent : ScriptableObject
{ 
    [SerializeField] private List<SFXID> sfx;
    [SerializeField] private SoundOptions soundOptions;
    [SerializeField] private VarOptions varOptions;
    [SerializeField] private bool loop;
    [SerializeField] private bool usingAudioBank;
    [SerializeField] private AudioBankID bankType;

    private int varValue = 0;
    private Dictionary<int, List<AudioClip>> bankValue = new Dictionary<int, List<AudioClip>>();
    private bool firstTime = true;

    public void Initialize()
    {
        
        //eventBus.Subscribe<OnAudioVarChange<AudioEnums>>();
    }

    public void Play(AudioSource audioSource, AudioDB audioDB, Func<IEnumerator, Coroutine> coroutineMethod)
    {
        
            switch (usingAudioBank)
            {
                case true:
                    bankValue = audioDB.AudioBanks[bankType];
                    switch (soundOptions)
                    {
                        case SoundOptions.Concurrent:

                            foreach (AudioClip audioClip in bankValue[varValue])
                                audioSource.PlayOneShot(audioClip);
                            break;

                        case SoundOptions.Sequentially:

                            coroutineMethod.Invoke(SequentialAudio( audioSource, audioDB));
                            break;

                        case SoundOptions.Random:

                            int index = UnityEngine.Random.Range(0, bankValue[varValue].Count);
                            audioSource.clip = bankValue[varValue][index];
                            audioSource.Play();

                            break;

                        case SoundOptions.Single:

                            audioSource.clip = bankValue[varValue][0];
                            audioSource.Play();
                            break;
                    }

                    break;

                case false:
                    switch (soundOptions)
                    {
                        case SoundOptions.Concurrent:

                            foreach (SFXID id in sfx)
                            {
                                audioSource.PlayOneShot(audioDB.SfxList[id]);
                            }
                            break;

                        case SoundOptions.Sequentially:

                            coroutineMethod.Invoke(SequentialAudio(audioSource, audioDB));
                            break;

                        case SoundOptions.Random:

                            int index = UnityEngine.Random.Range(0, sfx.Count);

                            audioSource.clip = audioDB.SfxList[sfx[index]];
                            audioSource.Play();
                            

                            break;

                        case SoundOptions.Single:
                            audioSource.clip = audioDB.SfxList[sfx[0]];
                            audioSource.Play();
                            break;
                    }
                    break;
            }
        
        
        if(loop && firstTime)
        {
            coroutineMethod.Invoke((Loop(audioSource, audioDB,coroutineMethod)));
        }

        firstTime = false;

    }

    private IEnumerator SequentialAudio( AudioSource audioSource, AudioDB audioDB)
    {
        if(usingAudioBank == false)
        {
            for (int i = 0; i < sfx.Count; i++)
            {
                audioSource.clip = audioDB.SfxList[sfx[i]];
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);
            }
        }
        else
        {
            for (int i = 0; i < bankValue[varValue].Count; i++)
            {
                audioSource.clip = bankValue[varValue][i];
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);
            }
        }
        
    }

    private IEnumerator Loop(AudioSource audioSource, AudioDB audioDB, Func<IEnumerator, Coroutine> coroutineMethod)
    {
        while(true)
        {
            if(soundOptions == SoundOptions.Concurrent && audioSource.isPlaying == false)
            {
                yield return coroutineMethod.Invoke(SequentialAudio(audioSource, audioDB));
            }
            else if(soundOptions != SoundOptions.Concurrent)
            {
                if (audioSource.isPlaying == false)
                {
                    Play(audioSource, audioDB, coroutineMethod);
                }
                yield return new WaitForSeconds(0.2f);
            }
            
            
        }
    }

}
