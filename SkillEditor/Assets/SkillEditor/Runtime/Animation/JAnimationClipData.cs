using UnityEngine;
using System;
using System.Collections;
using UnityEditor;
namespace CySkillEditor
{
    [Serializable]
    public class JAnimationClipData : ScriptableObject
    {
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
        private float startTime;
        public float StartTime
        {
            get { return startTime; }
            set
            {
                startTime = value;
                dirty = true;
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
                dirty = true;
            }
        }

        [SerializeField]
        private float stateDuration;
        public float StateDuration
        {
            get { return stateDuration; }
            set
            {
                stateDuration = value;
                dirty = true;
            }
        }

        [HideInInspector]
        [SerializeField]
        private string stateName = "NotSet";
        public string StateName
        {
            get { return stateName; }
            set
            {
                stateName = value;
                FriendlyName = MakeFriendlyStateName(StateName);
                dirty = true;
            }
        }

        [SerializeField]
        private JAnimationTrack track;
        public JAnimationTrack Track
        {
            get { return track; }
            set
            {
                track = value;
                dirty = true;
            }
        }

        public string FriendlyName
        {
            get { return MakeFriendlyStateName(StateName); }
            private set {; }
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
                dirty = true;
            }
        }

        [HideInInspector]
        private bool dirty;
        public bool Dirty
        {
            get { return dirty; }
            set { dirty = value; }
        }
        [SerializeField]
        private bool looping;
        public bool Looping
        {
            get { return looping; }
            set
            {
                looping = value;
            }
        }

        public float EndTime
        {
            get { return startTime + playbackDuration; }
            private set {; }
        }

        public int RunningLayer
        {
            get;
            set;
        }

        public delegate bool StateCheck(float sequencerTime, JAnimationClipData clipData);

        public static bool IsClipNotRunning(float sequencerTime, JAnimationClipData clipData)
        {
            return sequencerTime < clipData.StartTime;
        }

        public static bool IsClipRunning(float sequencerTime, JAnimationClipData clipData)
        {
            return sequencerTime > clipData.StartTime && sequencerTime < clipData.EndTime;
        }

        public static bool IsClipFinished(float sequencerTime, JAnimationClipData clipData)
        {
            return sequencerTime >= clipData.EndTime;
        }

        public static string MakeFriendlyStateName(string stateName)
        {
            var firstIndex = stateName.IndexOf("Layer.");

            if (firstIndex == -1)
                return stateName;

            var endIndex = firstIndex + "Layer.".Length;
            return stateName.Remove(0, endIndex);
        }
    }
}