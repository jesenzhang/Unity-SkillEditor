using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace CySkillEditor
{
    public class JTimelineSound : JTimelineBase
    {

        [SerializeField]
        private List<JSoundTrack> soundTracks = new List<JSoundTrack>();
        public List<JSoundTrack> SoundTracks
        {
            get { return soundTracks; }
            private set { soundTracks = value; }
        }

        [SerializeField]
        private List<JSoundClipData> allClips = new List<JSoundClipData>();

        private List<JSoundClipData> cachedRunningClips = new List<JSoundClipData>();

        [SerializeField]
        private Vector3 sourcePosition;

        [SerializeField]
        private Quaternion sourceOrientation;

        [SerializeField]
        public float RunningTime = 0;
        [SerializeField]
        private float previousTime = 0.0f;
        [SerializeField]
        public float SequenceUpdateRate = 0.015f;

        private bool scrubbing = false;

        [SerializeField]
        private AudioClip orientationClip;
        public AudioClip OrientationClip
        {
            get
            {
                if (sound == null)
                {
                    sound = AffectedObject.GetComponent<AudioSource>();
                }
                if (orientationClip == null)
                {
                    orientationClip = sound.clip;
                }
                return orientationClip;
            }
        }

        [SerializeField]
        private AudioSource sound;
        public AudioSource Sound
        {
            get
            {
                if (sound == null)
                {
                    sound = AffectedObject.GetComponent<AudioSource>();
                    orientationClip = sound.clip;
                }

                return sound;
            }
        }


        public override TimeLineType LineType()
        {
            return TimeLineType.Sound;
        }

        public override void StartTimeline()
        {
            sourcePosition = AffectedObject.transform.localPosition;
            sourceOrientation = AffectedObject.transform.localRotation;
            Sound.playOnAwake = false;
            Sound.mute = false;
        }
        public void ResetSound()
        {
            if (sound)
            {
                Sound.Stop();
            }

            if (RunningTime > 0.0f)
            {
                AffectedObject.transform.localPosition = sourcePosition;
                AffectedObject.transform.localRotation = sourceOrientation;
            }
        }

        public override void Process(float sequenceTime, float playbackRate)
        {
            allClips.Clear();
            for (int index = 0; index < SoundTracks.Count; index++)
            {
                var track = SoundTracks[index];
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
            scrubbing = !(absDeltaTime == runningTime);
            if (timelinePlayingInReverse)
            {
                ResetSound();
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
                        if (!JSoundClipData.IsClipRunning(runningTotalTime, clip) && !clip.Looping)
                            continue;
                        cachedRunningClips.Add(clip);
                    }

                    cachedRunningClips.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));

                    for (int runningClipIndex = 0; runningClipIndex < cachedRunningClips.Count; runningClipIndex++)
                    {
                        var clip = cachedRunningClips[runningClipIndex];
                        PlayClip(clip, runningTotalTime);
                    }

                    //  Sound.PlayScheduled(runningTime);

                    absDeltaTime -= SequenceUpdateRate;
                    if (!Mathf.Approximately(absDeltaTime, Mathf.Epsilon) && absDeltaTime < SequenceUpdateRate)
                        runningTime = absDeltaTime;

                    runningTotalTime += runningTime;
                }
            }

            previousTime = sequenceTime;
        }

        private void PlayClip(JSoundClipData clipToPlay, float sequenceTime)
        {
            if (Sound != null)
            {
                if (Sound.clip != clipToPlay.Clip)
                {
                    Sound.clip = clipToPlay.Clip;
                }

                float normalizedTime = (sequenceTime - clipToPlay.StartTime) / clipToPlay.PlaybackDuration;
                Sound.pitch = Sound.clip.length/clipToPlay.PlaybackDuration;
                if (clipToPlay.Looping)
                {
                    normalizedTime = ((sequenceTime - clipToPlay.StartTime) % clipToPlay.PlaybackDuration) / clipToPlay.PlaybackDuration;
                }
                normalizedTime = Mathf.Clamp(normalizedTime * Sound.clip.length, 0, Sound.clip.length);
                
                if ((Sound.clip.length - normalizedTime) > 0.0001f)
                {
                    if (Sequence.IsPlaying)
                    {
                        if (!Sound.isPlaying)
                        {
                            Sound.time = normalizedTime;
                            Sound.Play();
                        }
                        //Sound.time = normalizedTime;
                    }
                    else
                    {
                        Sound.time = normalizedTime;
                    }
                }

            }
        }

        public override void StopTimeline()
        {
            if (Sound)
            {
                ResetSound();
            }
            previousTime = 0.0f;

        }

        public override void EndTimeline()
        {
            base.EndTimeline();
            Sound.Stop();
        }

        public override void PauseTimeline()
        {
            if (Sound != null)
            {
                Sound.Pause();
            }
        }
        public override void ResumeTimeline()
        {
            Sound.Play();
        }
        public void AddTrack(JSoundTrack soundTrack)
        {
            soundTrack.TimeLine = this;
            SoundTracks.Add(soundTrack);
        }

        public void RemoveTrack(JSoundTrack soundTrack)
        {
            SoundTracks.Remove(soundTrack);
        }

        public void SetTracks(List<JSoundTrack> soundTrack)
        {
            SoundTracks = soundTrack;
        }

    }
}