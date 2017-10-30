using UnityEngine;
using System;
using System.Collections;
using UnityEditor;
namespace CySkillEditor
{
    
    [Serializable]
    public class JTrajectoryClipData : ScriptableObject
    {
        public Vector3 OriginalPos;
        [SerializeField]
        public Transform Target;
        [SerializeField]
        public JSkillUnit skillunit;
        [SerializeField]
        public SkillEffectUnit effectunit;

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
        [HideInInspector]
        private  JAbstractTrajectory[] trajectoryList;
        public JAbstractTrajectory[] TrajectoryList
        {
            get
            {
                if (trajectoryList == null)
                {
                    trajectoryList = new JAbstractTrajectory[0];
                }
                return trajectoryList;
            }
            set
            {
                trajectoryList = value;
            }
        }
        
 
        [HideInInspector]
        [SerializeField]
        private string stateName = "NotSet";
        public string StateName
        {
            get {
                if (effectunit.artEffect.effectObj != null)
                    stateName = effectunit.artEffect.effectObj.name;
                return stateName;
            }
            set
            {
                stateName = value;
                FriendlyName = MakeFriendlyStateName(StateName); 
            }
        }

        [SerializeField]
        private JTrajectoryTrack track;
        public JTrajectoryTrack Track
        {
            get { return track; }
            set
            {
                track = value;
            }
        }

        private void OnEnable()
        {
            if(skillunit==null)
                skillunit = new JSkillUnit();
            if (effectunit == null)
                effectunit = new SkillEffectUnit();
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
            }
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
        public float EndTime
        {
            get { return startTime + PlaybackDuration; }
            private set {; }
        }
   
        public static bool IsClipNotRunning(float sequencerTime, JTrajectoryClipData clipData)
        {
            return sequencerTime < clipData.StartTime;
        }

        public static bool IsClipRunning(float sequencerTime, JTrajectoryClipData clipData)
        {
            return sequencerTime > clipData.StartTime && sequencerTime < clipData.EndTime;
        }

        public static bool IsClipFinished(float sequencerTime, JTrajectoryClipData clipData)
        {
            return sequencerTime >= clipData.EndTime;
        }

        public static string MakeFriendlyStateName(string stateName)
        { 
            return stateName;
        }
    }
}