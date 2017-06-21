using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace CySkillEditor
{

    [Serializable]
    public class JCameraTrack : ScriptableObject
    {

        [SerializeField]
        private List<JCameraClipData> trackClipList = new List<JCameraClipData>();


        public List<JCameraClipData> TrackClips
        {
            get { return trackClipList; }
            private set { trackClipList = value; }
        }
        [SerializeField]
        private JTimelineBase limeLine;
        public JTimelineBase TimeLine
        {
            get { return limeLine; }
            set { limeLine = value; }
        }
        [SerializeField]
        private bool enable = true;

        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }
        public void AddClipWithName(SkillCameraAction action)
        {
            var clipData = ScriptableObject.CreateInstance<JCameraClipData>();
            clipData.TargetObject = TimeLine.AffectedObject.gameObject;
            clipData.Action = action;
            clipData.StartTime = action.delay;
            clipData.ActionName = Enum.GetName(typeof(SkillCameraAction.CameraAction), action.action);
           clipData.PlaybackDuration = action.phaseTime;
            clipData.Track = this;
            AddClip(clipData);
        }

        public void AddClipWithName(string effectName, float startTime, float PlayBackduration)
        {
            var clipData = ScriptableObject.CreateInstance<JCameraClipData>();
            clipData.TargetObject = TimeLine.AffectedObject.gameObject;
            clipData.StartTime = startTime;
            clipData.ActionName = effectName;
            clipData.PlaybackDuration = PlayBackduration;
            clipData.Action = new SkillCameraAction();
            clipData.Track = this;
            AddClip(clipData);
        }
        public void AddClip(JCameraClipData clipData)
        {
            if (trackClipList.Contains(clipData))
                throw new Exception("Track already contains Clip");

            trackClipList.Add(clipData);
        }

        public void RemoveClip(JCameraClipData clipData)
        {
            if (!trackClipList.Contains(clipData))
                throw new Exception("Track doesn't contains Clip");

            trackClipList.Remove(clipData);
        }

        private void SortClips()
        {
            trackClipList = trackClipList.OrderBy(trackClip => trackClip.StartTime).ToList();
        }

        public void SetClipData(List<JCameraClipData> particleData)
        {
            trackClipList = particleData;
        }

    }
}
