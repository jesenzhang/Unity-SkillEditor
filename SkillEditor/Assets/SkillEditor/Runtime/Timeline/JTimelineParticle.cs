using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CySkillEditor
{
    public class JTimelineParticle : JTimelineBase
    {
        [SerializeField]
        private List<JParticleTrack> particleTracks = new List<JParticleTrack>();
        public List<JParticleTrack> ParticleTracks
        {
            get { return particleTracks; }
            private set { particleTracks = value; }
        }

        [SerializeField]
        private List<JParticleClipData> allClips = new List<JParticleClipData>();

        private List<JParticleClipData> cachedRunningClips = new List<JParticleClipData>();
         
        float previousTime = 0.0f;

        [SerializeField]
        private List<ParticleSystem> particleList = new List<ParticleSystem>();
        
        private Dictionary<string, List<string>> particleDict = null;

        public Dictionary<string, List<string>> ParticleDict
        {
            get
            {
                if (particleDict == null)
                    CheckParticleList();
               
                return particleDict;
            }
        }


        [SerializeField]
        public List<ParticleSystem> rootParticleList = new List<ParticleSystem>();
        public List<ParticleSystem> RootParticleList
        {
            get
            {
                if (rootParticleList == null)
                    CheckParticleList();
                return rootParticleList;
            }
        }

        public ParticleSystem[] ParticleList
        {
            get
            {
                if (particleList == null)
                    CheckParticleList();
                return particleList.ToArray();
            }
        }

        public float GetEffectDuration(string name)
        {
            float duration = 0;
            if (ParticleDict.ContainsKey(name))
            {
                foreach (var pp in ParticleDict[name])
                {
                    foreach (var ppp in RootParticleList)
                    {
                        if (pp == ppp.name)
                        {
                            duration = duration < ppp.main.duration ? ppp.main.duration : duration;
                        }
                    }
                }
            }
            return duration;
        }

        private void CheckParticleList()
        {
            ParticleSystem[] pchild = AffectedObject.GetComponentsInChildren<ParticleSystem>(true);
            int newcount = pchild.Length;
            particleList = new List<ParticleSystem>();
            List<ParticleSystem> rootlist = new List<ParticleSystem>();

            if (pchild != null && pchild.Length > 0)
            {
                particleList.AddRange(pchild);
            }
            int len = particleList.Count;

            particleDict = new Dictionary<string, List<string>>();

            for (int i = 0; i < len; i++)
            {
                if (ParticleSystemUtility.IsRoot(particleList[i]))
                {
                    string key = particleList[i].transform.parent.name;
                    if (particleDict.ContainsKey(key))
                    {
                        particleDict[key].Add(particleList[i].name);
                    }
                    else
                    {
                        particleDict.Add(key, new List<string>() { particleList[i].name });
                    }
                    rootlist.Add(particleList[i]);
                }
            }
            rootParticleList = rootlist;

        }

        public override TimeLineType LineType()
        {
            return TimeLineType.Particle;
        }


        public ParticleSystem GetParticleClipWithName(string name)
        {
            for (int i = 0; i < ParticleList.Length; i++)
            {
                if (ParticleList[i].name == name)
                {
                    return ParticleList[i];
                }
            }
            return null;
        }

        public int GetIndexWithName(string name)
        {
            for (int i = 0; i < ParticleList.Length; i++)
            {
                if (ParticleList[i].name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public override void StartTimeline()
        { 
            ResetParticle();
            previousTime = 0.0f;
        }

        public override void StopTimeline()
        {
            ResetParticle();
            previousTime = 0.0f;

        }

        public override void EndTimeline()
        {
            base.EndTimeline();
        }


        public override void Process(float sequenceTime, float playbackRate)
        {
            allClips.Clear();
            for (int index = 0; index < ParticleTracks.Count; index++)
            {
                var track = ParticleTracks[index];
                if (track!=null && track.Enable)
                {
                    for (int trackClipIndex = 0; trackClipIndex < track.TrackClips.Count; trackClipIndex++)
                    {
                        var trackClip = track.TrackClips[trackClipIndex];
                        allClips.Add(trackClip);
                    }
                }
            }
            var totalDeltaTime = sequenceTime - previousTime;
            var absDeltaTime = Mathf.Abs(totalDeltaTime);
            var timelinePlayingInReverse = totalDeltaTime < 0.0f;
            var runningTime = JSequencer.SequenceUpdateRate;
            var runningTotalTime = previousTime + runningTime;
            if (timelinePlayingInReverse)
            {
                ResetParticle();
                previousTime = 0.0f;
                Process(sequenceTime, playbackRate);
            }
            else
            {
                while (absDeltaTime > 0.0f)
                {
                    cachedRunningClips.Clear();
                    for (int allClipIndex = 0; allClipIndex < allClips.Count; allClipIndex++)
                    {
                        var clip = allClips[allClipIndex];
                        if (!JParticleClipData.IsClipRunning(runningTotalTime, clip) && !clip.Looping)
                        {
                            if (clip.active)
                                clip.Reset();
                            continue;
                        }
                        cachedRunningClips.Add(clip);
                    }
                    cachedRunningClips.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));

                    for (int runningClipIndex = 0; runningClipIndex < cachedRunningClips.Count; runningClipIndex++)
                    {
                        var clip = cachedRunningClips[runningClipIndex];
                        clip.Init();
                        clip.OnUpdate(runningTime);
                       /* if (ParticleDict.ContainsKey(clip.ParticleName))
                        {
                            List<string> p = ParticleDict[clip.ParticleName];
                            foreach (var pp in RootParticleList)
                            {
                                foreach (var ppp in p)
                                {
                                    if (pp.name == ppp)
                                    {
                                        //float playtime = Mathf.Clamp(clip.EffectDuration * (runningTotalTime - clip.StartTime) / clip.PlaybackDuration, 0, clip.EffectDuration);
                                        //float speed = clip.EffectDuration / clip.PlaybackDuration;
                                        pp.Simulate(runningTime, true, false);
                                       
                                        if (JParticleClipData.IsClipFinished(runningTotalTime+runningTime, clip))
                                        {
                                            pp.Stop(true);
                                            pp.Simulate(0, true, true);
                                            pp.Clear(true);
                                        }
                                    }
                                }
                            }
                        }*/
                    }
                    absDeltaTime -= JSequencer.SequenceUpdateRate;
                    if (!Mathf.Approximately(absDeltaTime, Mathf.Epsilon) && absDeltaTime < JSequencer.SequenceUpdateRate)
                        runningTime = absDeltaTime;
                    runningTotalTime += runningTime;
                }
            }
            previousTime = sequenceTime;
        }

        public override void PauseTimeline()
        {
          //  for (int i = 0; i < RootParticleList.Count; i++)
            {
                //if (IsRoot(ParticleList[i]))
                {
                  //  RootParticleList[i].Pause(false);
                }
            }
        }

        public void ResetParticle()
        {
            for (int index = 0; index < ParticleTracks.Count; index++)
            {
                var track = ParticleTracks[index];
                if (track != null && track.Enable)
                {
                    for (int trackClipIndex = 0; trackClipIndex < track.TrackClips.Count; trackClipIndex++)
                    {
                        var trackClip = track.TrackClips[trackClipIndex];
                        trackClip.Reset();
                    }
                }
            }
            /*
            for (int i = 0; i < RootParticleList.Count; i++)
            {
                 if (RootParticleList[i]!=null)
                {
                    RootParticleList[i].Stop(true);
                    RootParticleList[i].Simulate(0, true, true);
                    RootParticleList[i].Clear(true);

                }
            }*/
        }
        public JParticleTrack AddNewTrack()
        {
            var track = ScriptableObject.CreateInstance<JParticleTrack>();
            AddTrack(track);
            return track;
        }
        public void AddTrack(JParticleTrack particleTrack)
        {
            particleTrack.TimeLine = this;
            ParticleTracks.Add(particleTrack);
        }

        public void RemoveTrack(JParticleTrack particleTrack)
        {
            ParticleTracks.Remove(particleTrack);
        }

        public void SetTracks(List<JParticleTrack> particleTrack)
        {
            ParticleTracks = particleTrack;
        }
    }
}