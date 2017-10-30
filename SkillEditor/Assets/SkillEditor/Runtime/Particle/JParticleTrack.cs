using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace CySkillEditor
{

    [Serializable]
    public class JParticleTrack : ScriptableObject
    {

        [SerializeField]
        private List<JParticleClipData> trackClipList = new List<JParticleClipData>();


        public List<JParticleClipData> TrackClips
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
        public void AddClipWithName(string effectName, float startTime, float PlayBackduration,EffectConfigure config)
        {
            var clipData = ScriptableObject.CreateInstance<JParticleClipData>();
            clipData.TargetObject = TimeLine.AffectedObject.gameObject;
            clipData.EffectConfig = config;
            clipData.StartTime = startTime;
            clipData.ParticleName = effectName;
            clipData.EffectDuration = ((JTimelineParticle)TimeLine).GetEffectDuration(effectName);// ParticleSystemUtility.GetParticleDuration(effectName, TimeLine.AffectedObject.gameObject);
            clipData.PlaybackDuration = PlayBackduration;
            clipData.Track = this;
            AddClip(clipData);
        }
        public void AddClipWithName(string effectName, float startTime, float PlayBackduration,GameObject effect, EffectConfigure config)
        {
            var clipData = ScriptableObject.CreateInstance<JParticleClipData>();
            clipData.TargetObject = TimeLine.AffectedObject.gameObject;
            clipData.Effect = effect;
            clipData.EffectConfig = config;
            clipData.StartTime = startTime;
            clipData.ParticleName = effectName;
            clipData.EffectDuration = ((JTimelineParticle)TimeLine).GetEffectDuration(effectName);// ParticleSystemUtility.GetParticleDuration(effectName, TimeLine.AffectedObject.gameObject);
            clipData.PlaybackDuration = PlayBackduration;
            clipData.Track = this;
            AddClip(clipData);
        }
        public void AddClipWithName(string effectName,float startTime,float PlayBackduration)
        {
            var clipData = ScriptableObject.CreateInstance<JParticleClipData>();
            clipData.TargetObject = TimeLine.AffectedObject.gameObject;
            clipData.StartTime = startTime;
            clipData.ParticleName = effectName;
            clipData.EffectDuration = ((JTimelineParticle)TimeLine).GetEffectDuration(effectName);// ParticleSystemUtility.GetParticleDuration(effectName, TimeLine.AffectedObject.gameObject);
            clipData.PlaybackDuration = PlayBackduration;
            clipData.Track = this;
            AddClip(clipData);
        }
        public void AddClip(JParticleClipData clipData)
        {
            if (trackClipList.Contains(clipData))
                throw new Exception("Track already contains Clip");
            clipData.Track = this;
            trackClipList.Add(clipData);
        }

        public void RemoveClip(JParticleClipData clipData)
        {
            if (!trackClipList.Contains(clipData))
                throw new Exception("Track doesn't contains Clip");

            trackClipList.Remove(clipData);
        }

        private void SortClips()
        {
            trackClipList = trackClipList.OrderBy(trackClip => trackClip.StartTime).ToList();
        }

        public void SetClipData(List<JParticleClipData> particleData)
        {
            trackClipList = particleData;
        }

    }
}
