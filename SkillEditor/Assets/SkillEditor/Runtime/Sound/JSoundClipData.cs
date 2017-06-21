using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

namespace CySkillEditor
{
    [Serializable]
    public class JSoundClipData : ScriptableObject
    {
        [SerializeField]
        private AudioClip clip;
        public AudioClip Clip
        {
            get { return clip; }
            set
            {
                clip = value;
            }
        }

        [SerializeField]
        private float startTime;
        public float StartTime
        {
            get { return startTime; }
            set
            {
                startTime = value;
            }
        }

        [SerializeField]
        private float playbackDuration;
        public float PlaybackDuration
        {
            get { return playbackDuration; }
            set
            {
                playbackDuration = value;
            }
        }

        [SerializeField]
        private float soundDuration;
        public float SoundDuration
        {
            get { return soundDuration; }
            set
            {
                soundDuration = value;
            }
        }
        [SerializeField]
        private bool crossFade;
        public bool CrossFade
        {
            get { return crossFade; }
            set { crossFade = value; }
        }

        [SerializeField]
        private float transitionDuration;
        public float TransitionDuration
        {
            get { return transitionDuration; }
            set { transitionDuration = value; }
        }
        [SerializeField]
        private JSoundTrack track;
        public JSoundTrack Track
        {
            get { return track; }
            set
            {
                track = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        private GameObject targetObject;
        public GameObject TargetObject
        {
            get { return targetObject; }
            set
            {
                targetObject = value;
            }
        }

        public int Index
        {
            get;
            set;
        }

        public float EndTime
        {
            get { return startTime + playbackDuration; }
            private set {; }
        }
        [SerializeField]
        private bool looping;
        public bool Looping
        {
            get { return looping; }
            set { looping = value; }
        }
        [HideInInspector]
        [SerializeField]
        private string soundName = "NotSet";
        public string SoundName
        {
            get
            {
                if (Clip != null)
                {
                    soundName = Clip.name;
                }
                return soundName;
            }

        }
        public string FriendlyName
        {
            get { return (SoundName); }
            private set {; }
        }

        public delegate bool StateCheck(float sequencerTime, JSoundClipData clipData);

        public static bool IsClipNotRunning(float sequencerTime, JSoundClipData clipData)
        {
            return sequencerTime < clipData.StartTime;
        }

        public static bool IsClipRunning(float sequencerTime, JSoundClipData clipData)
        {
            return sequencerTime > clipData.StartTime && sequencerTime < clipData.EndTime;
        }

        public static bool IsClipFinished(float sequencerTime, JSoundClipData clipData)
        {
            return sequencerTime >= clipData.EndTime;
        }
    }
}
