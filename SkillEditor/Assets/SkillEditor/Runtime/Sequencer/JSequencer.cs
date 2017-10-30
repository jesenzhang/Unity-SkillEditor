using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// JSequencer 时间序列 更新处理全部时间线
/// </summary>

namespace CySkillEditor
{

    [ExecuteInEditMode]
    [Serializable]
    public class JSequencer : MonoBehaviour
    {

        #region Member Variables

        /// <summary>
        /// 时间序列上相关的全部对象
        /// </summary>
        [SerializeField]
        private List<Transform> observedObjects = new List<Transform>();

        /// <summary>
        /// 当前运行时间
        /// </summary>
        [SerializeField]
        private float runningTime = 0.0f;
        /// <summary>
        /// 播放速率
        /// </summary>
        [SerializeField]
        private float playbackRate = 1.0f;
        /// <summary>
        /// 时间序列总长度 周期
        /// </summary>
        [SerializeField]
        private float duration = 10.0f;

        /// <summary>
        /// 播放方式 循环还是往返
        /// </summary>
        [SerializeField]
        private bool isLoopingSequence = false;

        [SerializeField]
        private bool isPingPongingSequence = false;

        [SerializeField]
        private bool updateOnFixedUpdate = false;
        /// <summary>
        /// 自动播放
        /// </summary>
        [SerializeField]
        private bool autoplay = false;

        private bool playing = false;
        /// <summary>
        /// 首次运行
        /// </summary>
        private bool isFreshPlayback = true;

        private float previousTime = -1.0f;

        private float minPlaybackRate = -100.0f;

        private float maxPlaybackRate = 100.0f;

        private float setSkipTime = -1.0f;
        #endregion

        //开放属性
        #region Properties
        public List<Transform> ObservedObjects
        {
            get
            {
                return observedObjects;
            }
        }
        public float Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
                if (duration <= 0.0f)
                    duration = 0.1f;
            }
        }
        
        /// <summary>
        /// 是否在播放
        /// </summary>
        public bool IsPlaying
        {
            get { return playing; }
        }
        /// <summary>
        /// 循环播放
        /// </summary>
        public bool IsLopping
        {
            get { return isLoopingSequence; }
            set { isLoopingSequence = value; }
        }

        /// <summary>
        /// 往返播放
        /// </summary>
        public bool IsPingPonging
        {
            get { return isPingPongingSequence; }
            set { isPingPongingSequence = value; }
        }
        /// <summary>
        /// 播放是否结束
        /// </summary>
        public bool IsComplete
        {
            get { return (!IsPlaying && RunningTime >= Duration); }
            set {; }
        }
        public float MinPlaybackRate
        {
            get { return minPlaybackRate; }
        }

        public float MaxPlaybackRate
        {
            get { return maxPlaybackRate; }
        }

        public float PlaybackRate
        {
            get { return playbackRate; }
            set { playbackRate = Mathf.Clamp(value, MinPlaybackRate, MaxPlaybackRate); }
        }
        public bool HasSequenceBeenStarted
        {
            get { return !isFreshPlayback; }
        }

        public float RunningTime
        {
            get { return runningTime; }
            set
            {
                runningTime = value;
                if (runningTime <= 0.0f)
                    runningTime = 0.0f;

                if (runningTime > duration)
                    runningTime = duration;

                if (isFreshPlayback)
                {
                    foreach (JTimelineContainer timelineContainer in TimelineContainers)
                    {
                        foreach (JTimelineBase timeline in timelineContainer.Timelines)
                            timeline.StartTimeline();
                    }
                    isFreshPlayback = false;
                }

                foreach (JTimelineContainer timelineContainer in TimelineContainers)
                {
                    timelineContainer.ManuallySetTime(RunningTime);
                    timelineContainer.ProcessTimelines(RunningTime, PlaybackRate);
                }

                OnRunningTimeSet(this);
            }
        }


        private JTimelineContainer[] timelineContainers;
        public JTimelineContainer[] TimelineContainers
        {
            get
            {
                if (timelineContainers == null)
                    timelineContainers = GetComponentsInChildren<JTimelineContainer>();

                return timelineContainers;
            }
        }

        public JTimelineContainer[] SortedTimelineContainers
        {
            get
            {
                var timelineContainers = TimelineContainers;
                Array.Sort(timelineContainers, JTimelineContainer.Comparer);

                return timelineContainers;
            }
        }

        public JTimelineBase[] SortedTimelines
        {
            get
            {
                List<JTimelineBase> list = new List<JTimelineBase>();
                foreach (JTimelineContainer contain in TimelineContainers)
                {
                    list.AddRange(contain.Timelines);
                }
                list.Sort(JTimelineBase.Comparer);

                return list.ToArray();
            }
        }
        public List<List<JTimelineBase>> SortedTimelinesLists
        {
            get
            {
                List<List<JTimelineBase>> TypeList = new List<List<JTimelineBase>>();

                foreach (TimeLineType type in Enum.GetValues(typeof(TimeLineType)))
                {
                    // TODO: 遍历操作  
                    TypeList.Add(new List<JTimelineBase>());
                }

                foreach (JTimelineContainer contain in TimelineContainers)
                {
                    foreach (JTimelineBase line in contain.Timelines)
                    {
                        int index = (int)line.LineType();
                        TypeList[index].Add(line);
                    }
                }
                return TypeList;
            }
        }
        public int TimelineContainerCount
        {
            get
            {
                return TimelineContainers.Length;
            }
        }

        public int ObservedObjectCount
        {
            get
            {
                return ObservedObjects.Count;
            }
        }

        public bool UpdateOnFixedUpdate
        {
            get { return updateOnFixedUpdate; }
            set { updateOnFixedUpdate = value; }
        }

        public static float SequenceUpdateRate
        {
            get { return 0.005f * Time.timeScale; }
        }
        #endregion

        #region Delegates
        public delegate void PlaybackDelegate(JSequencer sequencer);
        public delegate void UpdateDelegate(JSequencer sequencer, float newRunningTime);
        /// <summary>
        /// This Delegate will be called when Playback has Started, add delegates with +=
        /// </summary>
        public PlaybackDelegate PlaybackStarted = delegate { };
        /// <summary>
        /// This Delegate will be called when Playback has Stopped, add delegates with +=
        /// </summary>
        public PlaybackDelegate PlaybackStopped = delegate { };
        /// <summary>
        /// This Delegate will be called when Playback has Paused, add delegates with +=
        /// </summary>
        public PlaybackDelegate PlaybackPaused = delegate { };
        /// <summary>
        /// This Delegate will be called when Playback has Finished, add delegates with +=
        /// </summary>
        public PlaybackDelegate PlaybackFinished = delegate { };
        /// <summary>
        /// This Delegate will be called before an update with the new runningTime, and before timelines have been processed add delegates with +=
        /// </summary>
        public UpdateDelegate BeforeUpdate = delegate { };
        /// <summary>
        /// This Delegate will be called after an update with the new runningTime, and after timelines have been processed add delegates with +=
        /// </summary>
        public UpdateDelegate AfterUpdate = delegate { };
        /// <summary>
        /// This Delegate will be called whenever the RunningTime is set add delegates with +=
        /// </summary>
        public PlaybackDelegate OnRunningTimeSet = delegate { };
        #endregion
        private void OnDestroy()
        {
            StopCoroutine("UpdateSequencerCoroutine");
        }

        private void Start()
        {
            // Attempt to auto fix our Event Objects
            foreach (JTimelineContainer timelineContainer in TimelineContainers)
            {
                if (!timelineContainer)
                    continue;

                ///处理时间先开始时的一些事件
                ///待续
            }

            if (autoplay && Application.isPlaying)
                Play();
        }

        public void TogglePlayback()
        {
            if (playing)
                Pause();
            else
                Play();
        }

        public void Play()
        {
            if (PlaybackStarted != null)
                PlaybackStarted(this);

            // Playback runs on a coroutine.
            StartCoroutine("UpdateSequencerCoroutine");

            // Start or resume our playback.
            if (isFreshPlayback)
            {
                foreach (JTimelineContainer timelineContainer in TimelineContainers)
                {
                    foreach (JTimelineBase timeline in timelineContainer.Timelines)
                    {
                        timeline.StartTimeline();
                    }
                }
                isFreshPlayback = false;
            }
            else
            {
                foreach (JTimelineContainer timelineContainer in TimelineContainers)
                {
                    foreach (JTimelineBase timeline in timelineContainer.Timelines)
                    {
                        timeline.ResumeTimeline();
                    }
                }
            }

            playing = true;
            previousTime = Time.time;
        }

        public void Pause()
        {
            if (PlaybackPaused != null)
                PlaybackPaused(this);

            playing = false;

            foreach (JTimelineContainer timelineContainer in TimelineContainers)
            {
                foreach (JTimelineBase timeline in timelineContainer.Timelines)
                {
                    timeline.PauseTimeline();
                }
            }
        }

        public void Stop()
        {
            if (PlaybackStopped != null)
                PlaybackStopped(this);

            // Playback runs on a coroutine.
            StopCoroutine("UpdateSequencerCoroutine");

            foreach (JTimelineContainer timelineContainer in TimelineContainers)
            {
                foreach (JTimelineBase timeline in timelineContainer.Timelines)
                {
                    if (timeline.AffectedObject != null)
                        timeline.StopTimeline();
                }
            }

            isFreshPlayback = true;
            playing = false;
            runningTime = 0.0f;
        }

        /// <summary>
        ///  This method will be called when the scrub head has reached the end of playback, for a 10s long sequence, this will be 10 seconds in.
        /// </summary>
        private void End()
        {
            if (PlaybackFinished != null)
                PlaybackFinished(this);

            if (isLoopingSequence || isPingPongingSequence)
                return;

            foreach (JTimelineContainer timelineContainer in TimelineContainers)
            {
                foreach (JTimelineBase timeline in timelineContainer.Timelines)
                {
                    if (timeline.AffectedObject != null)
                        timeline.EndTimeline();
                }
            }
        }

        /// <summary>
        /// 创建时间线容器
        /// </summary>
        public JTimelineContainer CreateNewTimelineContainer(Transform affectedObject)
        {
            GameObject newTimelineContainerGO = new GameObject("TimelineContainer for " + affectedObject.name);
            newTimelineContainerGO.transform.parent = transform;

            JTimelineContainer timelineContainer = newTimelineContainerGO.AddComponent<JTimelineContainer>();
            timelineContainer.AffectedObject = affectedObject;

            int highestIndex = 0;
            foreach (JTimelineContainer ourTimelineContainer in TimelineContainers)
            {
                if (ourTimelineContainer.Index > highestIndex)
                    highestIndex = ourTimelineContainer.Index;
            }

            timelineContainer.Index = highestIndex + 1;
            return timelineContainer;
        }

        public JTimelineAnimation CreateAnimationTimeline(JTimelineContainer container)
        {
            GameObject newTimelineContainerGO = new GameObject("AnimationTimeline for " + container.AffectedObject.name);
            newTimelineContainerGO.transform.parent = container.transform;

            JTimelineAnimation timelineContainer = newTimelineContainerGO.AddComponent<JTimelineAnimation>();

            return timelineContainer;
        }

        /// <summary>
        /// 是否含有作用于affectedObject的时间线容器
        /// </summary>
        /// <param name="affectedObject"></param>
        /// <returns></returns>
        public bool HasTimelineContainerFor(Transform affectedObject)
        {
            foreach (var timelineContainer in TimelineContainers)
            {
                if (timelineContainer.AffectedObject == affectedObject)
                    return true;
            }
            return false;
        }

        /// <summary>
        ///得到affectedObject的时间线容器
        /// </summary>
        /// <returns>The timeline container for.</returns>
        /// <param name="affectedObject">Affected object.</param>
        public JTimelineContainer GetTimelineContainerFor(Transform affectedObject)
        {
            foreach (var timelineContainer in TimelineContainers)
            {
                if (timelineContainer.AffectedObject == affectedObject)
                    return timelineContainer;
            }
            return null;
        }

        public void DeleteTimelineContainer(JTimelineContainer timelineContainer)
        {
            GameObject.DestroyImmediate(timelineContainer.gameObject);
        }

        public void RemoveObservedObject(Transform observedObject)
        {
            if (!observedObjects.Contains(observedObject))
                return;

            observedObjects.Remove(observedObject);
        }

        /// <summary>
        /// Sets the time of this sequence to the passed time. This function will only fire events that are flagged as
        /// Fire On Skip and are Fire And Forget Events (A.K.A have a duration of < 0), all other events will be ignored. 
        /// If you want to set the time and fire all events, simply set the RunningTime.
        /// 
        /// ObserverTimelines and PropertyTimelines will work as before.
        /// </summary>
        public void SkipTimelineTo(float time)
        {
            if (RunningTime <= 0.0f && !IsPlaying)
                Play();

            setSkipTime = time;
        }

        public void SetPlaybackRate(float rate)
        {
            PlaybackRate = rate;
        }

        public void SetPlaybackTime(float time)
        {
            RunningTime = time;
        }

        public void UpdateSequencer(float deltaTime)
        {
            // 处理播放速度
            deltaTime *= playbackRate;

            // 更新时间线
            if (playing)
            {
                runningTime += deltaTime;
                float sampleTime = runningTime;

                if (sampleTime <= 0.0f)
                    sampleTime = 0.0f;
                if (sampleTime > Duration)
                    sampleTime = Duration;

                BeforeUpdate(this, runningTime);
                foreach (JTimelineContainer timelineContainer in TimelineContainers)
                    timelineContainer.ProcessTimelines(sampleTime, PlaybackRate);

                AfterUpdate(this, runningTime);

                bool hasReachedEnd = false;
                if (playbackRate > 0.0f && RunningTime >= duration)
                    hasReachedEnd = true;
                if (playbackRate < 0.0f && RunningTime <= 0.0f)
                    hasReachedEnd = true;

                if (hasReachedEnd)
                {
                    // 处理循环播放
                    if (isLoopingSequence)
                    {
                        var newRunningTime = 0.0f;
                        if (playbackRate > 0.0f && RunningTime >= Duration)
                            newRunningTime = RunningTime - Duration;
                        if (playbackRate < 0.0f && RunningTime <= 0.0f)
                            newRunningTime = Duration + RunningTime;

                        Stop();

                        runningTime = newRunningTime;
                        previousTime = -1.0f;

                        Play();

                        UpdateSequencer(0.0f);

                        return;
                    }
                    //处理兵乓播放
                    if (isPingPongingSequence)
                    {
                        if (playbackRate > 0.0f && RunningTime >= Duration)
                            runningTime = Duration + (Duration - RunningTime);
                        if (playbackRate < 0.0f && RunningTime <= 0.0f)
                            runningTime = -1.0f * RunningTime;

                        playbackRate *= -1.0f;

                        return;
                    }

                    playing = false;

                    // 启动协程 重放
                    StopCoroutine("UpdateSequencerCoroutine");

                    End();
                }
            }

            // 处理协程避免冲突
            if (setSkipTime > 0.0f)
            {
                foreach (JTimelineContainer timelineContainer in TimelineContainers)
                    timelineContainer.SkipTimelineTo(setSkipTime);

                runningTime = setSkipTime;

                previousTime = Time.time;

                setSkipTime = -1.0f;
            }
        }

        private IEnumerator UpdateSequencerCoroutine()
        {
            var wait = new WaitForSeconds(SequenceUpdateRate);
            while (true)
            {
                if (UpdateOnFixedUpdate)
                    yield break;

                float currentTime = Time.time;
                UpdateSequencer(currentTime - previousTime);
                previousTime = currentTime;

                yield return wait;
            }
        }

        private void FixedUpdate()
        {
            if (!UpdateOnFixedUpdate)
                return;

            float currentTime = Time.time;
            UpdateSequencer(currentTime - previousTime);
            previousTime = currentTime;
        }

        public void ResetCachedData()
        {
            timelineContainers = null;
            foreach (var timelineContainer in TimelineContainers)
                timelineContainer.ResetCachedData();
        }


        #region ExtensionRegion
        public List<JTimelineBase> CreateContainers(Transform affectedObject)
        {
            JTimelineContainer container;
            List<JTimelineBase> linelist = new List<JTimelineBase>();
            if (HasTimelineContainerFor(affectedObject))
            {
                container = GetTimelineContainerFor(affectedObject);
            }
            else
            {
                container = CreateNewTimelineContainer(affectedObject);
            }

            return linelist;
        }

        #endregion
    }
}