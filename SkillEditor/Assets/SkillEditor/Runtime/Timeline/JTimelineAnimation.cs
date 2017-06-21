using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace CySkillEditor
{
    public class JTimelineAnimation : JTimelineBase
    {

        private Dictionary<int, List<AnimatorClipInfo>> initialAnimationInfo = new Dictionary<int, List<AnimatorClipInfo>>();
        private Dictionary<int, AnimatorStateInfo> initialAnimatorStateInfo = new Dictionary<int, AnimatorStateInfo>();

        [SerializeField]
        private List<JAnimationTrack> animationsTracks = new List<JAnimationTrack>();
        public List<JAnimationTrack> AnimationTracks
        {
            get { return animationsTracks; }
            private set { animationsTracks = value; }
        }


        [SerializeField]
        private Animator animator;
        private Animator Animator
        {
            get
            {
                if (animator == null)
                {
                    animator = AffectedObject.GetComponent<Animator>();
                }

                return animator;
            }
        }

        [SerializeField]
        private Vector3 sourcePosition;

        [SerializeField]
        private Quaternion sourceOrientation;

        [SerializeField]
        private float sourceSpeed;

        private bool previousEnabled;

        [SerializeField]
        public float RunningTime = 0;
        [SerializeField]
        private float previousTime = 0.0f;
        [SerializeField]
        public float SequenceUpdateRate = 0.015f;

        [SerializeField]
        private List<JAnimationClipData> allClips = new List<JAnimationClipData>();

        private List<JAnimationClipData> cachedRunningClips = new List<JAnimationClipData>();


        public override TimeLineType LineType()
        {
            return TimeLineType.Animation;
        }

        public override void StartTimeline()
        {
            InitAnimationState();
            previousEnabled = Animator.enabled;
            Animator.enabled = false;
        }
        private void InitAnimationState()
        {
            sourcePosition = AffectedObject.transform.localPosition;
            sourceOrientation = AffectedObject.transform.localRotation;
            sourceSpeed = Animator.speed;

            for (int layer = 0; layer < Animator.layerCount; layer++)
            {
                initialAnimationInfo.Add(layer, new List<AnimatorClipInfo>());
                var values = Animator.GetCurrentAnimatorClipInfo(layer);
                foreach (var value in values)
                {
                    initialAnimationInfo[layer].Add(value);
                }
            }
            for (int layer = 0; layer < Animator.layerCount; layer++)
                initialAnimatorStateInfo.Add(layer, Animator.GetCurrentAnimatorStateInfo(layer));

        }

        public void ResetAnimation()
        {
            for (int layer = 0; layer < Animator.layerCount; layer++)
            {
                if (!initialAnimatorStateInfo.ContainsKey(layer))
                    continue;
                Animator.Play(initialAnimatorStateInfo[layer].fullPathHash, layer, 0);// initialAnimatorStateInfo[layer].normalizedTime);
                Animator.Update(0.0f);
            }

            if (RunningTime > 0.0f)
            {
                //AffectedObject.transform.localPosition = sourcePosition;
                //AffectedObject.transform.localRotation = sourceOrientation;
            }
        }

        public override void Process(float sequenceTime, float playbackRate)
        {
            allClips.Clear();
            for (int index = 0; index < AnimationTracks.Count; index++)
            {
                var track = AnimationTracks[index];
                if (track.Enable)
                {
                    for (int trackClipIndex = 0; trackClipIndex < track.TrackClips.Count; trackClipIndex++)
                    {
                        var trackClip = track.TrackClips[trackClipIndex];
                        allClips.Add(trackClip);
                        trackClip.RunningLayer = track.Layer;
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
                ResetAnimation();
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
                        if (!JAnimationClipData.IsClipRunning(runningTotalTime, clip) && !clip.Looping)
                            continue;

                        cachedRunningClips.Add(clip);
                    }

                    cachedRunningClips.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));

                    for (int runningClipIndex = 0; runningClipIndex < cachedRunningClips.Count; runningClipIndex++)
                    {
                        var clip = cachedRunningClips[runningClipIndex];
                        PlayClip(clip, clip.RunningLayer, runningTotalTime);
                    }

                    Animator.Update(runningTime);

                    absDeltaTime -= SequenceUpdateRate;
                    if (!Mathf.Approximately(absDeltaTime, Mathf.Epsilon) && absDeltaTime < SequenceUpdateRate)
                        runningTime = absDeltaTime;

                    runningTotalTime += runningTime;
                }
            }

            previousTime = sequenceTime;
        }

        private void PlayClip(JAnimationClipData clipToPlay, int layer, float sequenceTime)
        {
            float normalizedTime = (sequenceTime - clipToPlay.StartTime) / clipToPlay.PlaybackDuration;

            if (clipToPlay.Looping)
            {
                normalizedTime = ((sequenceTime - clipToPlay.StartTime) % clipToPlay.PlaybackDuration) / clipToPlay.PlaybackDuration;
            }
            if (clipToPlay.CrossFade)
            {
                // The calculation and clamp are here, to resolve issues with big timesteps.
                // crossFadeTime will not always be equal to clipToPlay.transitionDuration, for insance
                // if the timeStep allows for a step of 0.5, we'll be 0.5s into the crossfade.
                var crossFadeTime = clipToPlay.TransitionDuration - (sequenceTime - clipToPlay.StartTime);
                crossFadeTime = Mathf.Clamp(crossFadeTime, 0.0f, Mathf.Infinity);

                Animator.CrossFade(clipToPlay.StateName, crossFadeTime, layer, normalizedTime);
            }
            else
                Animator.Play(clipToPlay.StateName, layer, normalizedTime);
        }

        public override void StopTimeline()
        {
            Animator.Update(-RunningTime);
            Animator.StopPlayback();

            ResetAnimation();

            initialAnimationInfo.Clear();
            initialAnimatorStateInfo.Clear();

            previousTime = 0.0f;

            Animator.speed = sourceSpeed;

            Animator.enabled = previousEnabled;
        }

        public override void EndTimeline()
        {
            Animator.enabled = previousEnabled;
        }

        public override void PauseTimeline()
        {
            Animator.enabled = false;
        }

        public JAnimationTrack AddNewTrack()
        {
            var track = ScriptableObject.CreateInstance<JAnimationTrack>();
            AddTrack(track);
            return track;
        }

        public void AddTrack(JAnimationTrack animationTrack)
        {
            animationTrack.TimeLine = this;
            animationsTracks.Add(animationTrack);
        }

        public void RemoveTrack(JAnimationTrack animationTrack)
        {
            animationsTracks.Remove(animationTrack);
        }

        public void SetTracks(List<JAnimationTrack> animationTrack)
        {
            AnimationTracks = animationTrack;
        }
    }
}