using Audio.Enums;
using EventBuses;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.EventBuses.Events
{
    public struct OnMusicPlay : IEvent
    {
        public MusicID musicClip;
        public AudioEventsID audioEvent;

        public OnMusicPlay(MusicID audioClip)
        {
            this.musicClip = audioClip;
            audioEvent = AudioEventsID.NULL;
        }

        public OnMusicPlay(AudioEventsID audioEvent)
        {
            this.audioEvent = audioEvent;
            musicClip = MusicID.NULL;
        }
    }
}