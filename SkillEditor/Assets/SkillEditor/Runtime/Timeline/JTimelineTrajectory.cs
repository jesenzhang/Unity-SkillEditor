using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace CySkillEditor
{
    public class JTimelineTrajectory : JTimelineBase
    {
        private Dictionary<int, List<AnimatorClipInfo>> initialAnimationInfo = new Dictionary<int, List<AnimatorClipInfo>>();
        private Dictionary<int, AnimatorStateInfo> initialAnimatorStateInfo = new Dictionary<int, AnimatorStateInfo>();

        [SerializeField]
        private List<JTrajectoryTrack> trajectoryTracks = new List<JTrajectoryTrack>();
        public List<JTrajectoryTrack> TrajectoryTracks
        {
            get { return trajectoryTracks; }
            private set { trajectoryTracks = value; }
        }

        [SerializeField]
        public float RunningTime = 0;
        [SerializeField]
        private float previousTime = 0.0f;
        [SerializeField]
        public float SequenceUpdateRate = 0.015f;
        [SerializeField]
        private List<JTrajectoryClipData> allClips = new List<JTrajectoryClipData>();
        private List<JTrajectoryClipData> cachedRunningClips = new List<JTrajectoryClipData>();
        bool Restart = false;
        public override TimeLineType LineType()
        {
            return TimeLineType.Trajectory;
        }

        public override void StartTimeline()
        {
            Restart = true;
        }
        public override void Process(float sequenceTime, float playbackRate)
        {
            allClips.Clear();
            for (int index = 0; index < TrajectoryTracks.Count; index++)
            {
                var track = TrajectoryTracks[index];
                if (track.Enable)
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
            var runningTime = SequenceUpdateRate;
            var runningTotalTime = previousTime + runningTime;

            if (timelinePlayingInReverse)
            { 
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
                        if (JTrajectoryClipData.IsClipFinished(runningTotalTime, clip))
                        {
                            for (int i = 0; i < clip.TrajectoryList.Length; i++)
                            {
                                if (clip.TrajectoryList[i]._active)
                                {
                                    clip.TrajectoryList[i].Reset();
                                }
                                
                            }
                         //   if (clip.Trajectory._active)
                         //   {
                         //       clip.Trajectory.Reset();
                        //    }
                        }
                        if (!JTrajectoryClipData.IsClipRunning(runningTotalTime, clip) && !clip.Looping)
                            continue;

                        cachedRunningClips.Add(clip);
                    }

                    cachedRunningClips.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));

                    for (int runningClipIndex = 0; runningClipIndex < cachedRunningClips.Count; runningClipIndex++)
                    {
                        var clip = cachedRunningClips[runningClipIndex];
                        if (Restart)
                        {
                            if(clip.skillunit.launchType == JSkillUnit.LaunchType.SINGLELINE)
                                    clip.TrajectoryList = SingleLineFacyory(clip);
                            if (clip.skillunit.launchType == JSkillUnit.LaunchType.MULLINE)
                                clip.TrajectoryList = MultiLineFacyory(clip);
                            Restart = false;
                        }
                        if (clip.TrajectoryList != null)
                        {
                            for (int i = 0; i < clip.TrajectoryList.Length; i++)
                            {
                                if (!clip.TrajectoryList[i]._active)
                                {
                                    clip.TrajectoryList[i].Begin();
                                }
                                clip.TrajectoryList[i].DoUpdate(Time.realtimeSinceStartup);
                            }
                            
                        }
                       
                    }
                     
                    absDeltaTime -= SequenceUpdateRate;
                    if (!Mathf.Approximately(absDeltaTime, Mathf.Epsilon) && absDeltaTime < SequenceUpdateRate)
                        runningTime = absDeltaTime;

                    runningTotalTime += runningTime;
                }
            }

            previousTime = sequenceTime;
        }

       

        public override void StopTimeline()
        {

            for (int index = 0; index < TrajectoryTracks.Count; index++)
            {
                var track = TrajectoryTracks[index];
               
                for (int trackClipIndex = 0; trackClipIndex < track.TrackClips.Count; trackClipIndex++)
                {
                    var trackClip = track.TrackClips[trackClipIndex];
                    for (int i = 0; i < trackClip.TrajectoryList.Length; i++)
                    { 
                        trackClip.TrajectoryList[i].Reset(); 
                    }
                }
            }
            Restart = true;
        }

        public override void EndTimeline()
        {
            for (int index = 0; index < TrajectoryTracks.Count; index++)
            {
                var track = TrajectoryTracks[index];

                for (int trackClipIndex = 0; trackClipIndex < track.TrackClips.Count; trackClipIndex++)
                {
                    var trackClip = track.TrackClips[trackClipIndex];
                    for (int i = 0; i < trackClip.TrajectoryList.Length; i++)
                    {
                        trackClip.TrajectoryList[i].Reset();
                    }
                }

            }
            Restart = true;
        }

        public override void PauseTimeline()
        {
            
        }

        public JTrajectoryTrack AddNewTrack()
        {
            var track = ScriptableObject.CreateInstance<JTrajectoryTrack>();
            AddTrack(track);
            return track;
        }

        public void AddTrack(JTrajectoryTrack track)
        {
            track.TimeLine = this;
            trajectoryTracks.Add(track);
        }

        public void RemoveTrack(JTrajectoryTrack animationTrack)
        {
            trajectoryTracks.Remove(animationTrack);
        }

        public void SetTracks(List<JTrajectoryTrack> animationTrack)
        {
            trajectoryTracks = animationTrack;
        }

        private JSingleLineTrajectory[] SingleLineFacyory(JTrajectoryClipData clip)
        {
            if (clip.skillunit.launchType != JSkillUnit.LaunchType.SINGLELINE)
                return null;
            List<JSingleLineTrajectory> list = new List<JSingleLineTrajectory>();
            SkillLine line = (SkillLine)clip.skillunit.skillObj;
            
            if (line.waves == 0)
                line.waves = 1;
            for (int i=0;i< line.waves;i++)
            {
                JSingleLineTrajectory sg = new JSingleLineTrajectory();
                sg.parent = clip;
                sg._originDir = clip.TargetObject.transform.forward;
                sg._delayBegin = i * line.waveDelay;
                list.Add(sg);
            }
            clip.PlaybackDuration = (line.waveDelay * line.waves + line.moveTime+ clip.skillunit.guidePolicy.guideTime+ clip.skillunit.guidePolicy.guidingTime) /1000f;
            return list.ToArray();
        }
        public  Vector3 RotateRound(Vector3 dir, Vector3 axis, float angle)
        {
            Vector3 point = Quaternion.AngleAxis(angle, axis) * dir;
            return point.normalized;
        }
        private JSingleLineTrajectory[] MultiLineFacyory(JTrajectoryClipData clip)
        {
            if (clip.skillunit.launchType != JSkillUnit.LaunchType.MULLINE)
                return null;
            List<JSingleLineTrajectory> list = new List<JSingleLineTrajectory>();
            SkillMultiLine line = (SkillMultiLine)clip.skillunit.skillObj;
            if (line.shape.area == SkillShape.Area.CIRCLE)
            {
                float angle = 360f / line.unitCount;
                Vector3 dir = clip.TargetObject.transform.forward;
                for (int i = 0; i < line.unitCount; i++)
                {
                    Vector3 ndir = RotateRound(dir, new Vector3(0, 1, 0), i * angle);
                    if (line.waves == 0)
                        line.waves = 1;
                    for (int j = 0; j < line.waves; j++)
                    {
                        JSingleLineTrajectory sg = new JSingleLineTrajectory();
                        sg._originDir = dir;
                        sg.parent = clip;
                        sg._delayBegin = j * line.waveDelay;
                        list.Add(sg);
                    }
                }
            }
            if (line.shape.area == SkillShape.Area.QUADRATE)
            {
                float angle = 360f / line.unitCount;
                Vector3 dir = clip.TargetObject.transform.forward;
                Vector3 ndir = RotateRound(dir, new Vector3(0, 1, 0),90);
                if (line.unitCount > 1)
                {
                    ndir = ndir * line.shape.param2 / (line.unitCount - 1);
                }
                Vector3 beginPos =  - ndir * (line.unitCount - 1)/2f;
                for (int i = 0; i < line.unitCount; i++)
                {
                    if (line.waves == 0)
                        line.waves = 1;
                    for (int j = 0; j < line.waves; j++)
                    {
                        JSingleLineTrajectory sg = new JSingleLineTrajectory();
                        sg._originDir = dir;
                        sg._originPosOffset = beginPos + ndir * i;
                        sg.parent = clip;
                        sg._delayBegin =  j*line.waveDelay;
                        list.Add(sg);
                    }
                }
            }
            if (line.shape.area == SkillShape.Area.SECTOR)
            {
                float angle = line.shape.param3 / (line.unitCount-1);
                float beginAngle = -angle * (line.unitCount - 1) / 2;
                Vector3 dir = clip.TargetObject.transform.forward;
                for (int i = 0; i < line.unitCount; i++)
                {
                    Vector3 ndir = RotateRound(dir, new Vector3(0, 1, 0), beginAngle+i*angle);
                    if (line.waves == 0)
                        line.waves = 1;
                    for (int j = 0; j < line.waves; j++)
                    {
                        JSingleLineTrajectory sg = new JSingleLineTrajectory();
                        sg._originDir = ndir;
                        sg.parent = clip;
                        sg._delayBegin = j * line.waveDelay;
                        list.Add(sg);
                    }
                }
            }
            if (line.shape.area == SkillShape.Area.TRIANGLE)
            {
                Vector3 dir = clip.TargetObject.transform.forward;
                Vector3 ndir = RotateRound(dir, new Vector3(0, 1, 0), 90);
                if (line.unitCount > 1)
                {
                    ndir = ndir * line.shape.param2 / (line.unitCount - 1)/2;
                }
                Vector3 beginPos = Vector3.zero;
                if(line.shape.param3==1)
                    beginPos  =- ndir * (line.unitCount - 1) / 2f;
                if (line.shape.param3 == 2)
                    beginPos = dir * line.shape.param1 - ndir * (line.unitCount - 1) / 2f;
                Vector3 launchPos = Vector3.zero;
                Vector3 pdir = Vector3.zero;
                for (int i = 0; i < line.unitCount; i++)
                {
                    launchPos = beginPos + i * ndir;
                    if (line.shape.param3 == 1)
                    {
                        Vector3 tempPos = dir * line.shape.param1;
                        pdir = tempPos - launchPos;
                    }
                    else
                    if (line.shape.param3 == 2)
                    {
                        pdir =  launchPos;
                    }
                   
                    pdir = Vector3.Normalize(pdir);
                    if (line.waves == 0)
                        line.waves = 1;
                    for (int j = 0; j < line.waves; j++)
                    {
                        JSingleLineTrajectory sg = new JSingleLineTrajectory();
                        sg._originPosOffset = launchPos;
                        sg._originDir = pdir;
                        sg.parent = clip;
                        sg._delayBegin = j * line.waveDelay;
                        list.Add(sg);
                    }
                }
            }
            clip.PlaybackDuration = 10;// (line.waveDelay * line.waves + line.moveTime + clip.skillunit.guidePolicy.guideTime + clip.skillunit.guidePolicy.guidingTime) / 1000f;
            return list.ToArray();
        }

    }
}