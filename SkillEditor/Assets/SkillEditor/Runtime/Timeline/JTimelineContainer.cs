using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace CySkillEditor
{
    /// <summary>
    /// 时间线容器
    /// </summary>
    [ExecuteInEditMode]
    public class JTimelineContainer : MonoBehaviour
    {
        public static int Comparer(JTimelineContainer a, JTimelineContainer b)
        {
            return (a.Index.CompareTo(b.Index));
        }

        #region Member Variables
        [SerializeField]
        private Transform affectedObject = null;

        [SerializeField]
        private string affectedObjectPath;

        [SerializeField]
        private int index = -1;
        #endregion

        #region Properties
        /// <summary>
        /// 得到时间线容器作用对象
        /// </summary>
        public Transform AffectedObject
        {
            get
            {
                if (affectedObject == null && affectedObjectPath != string.Empty)
                {
                    var foundGameObject = GameObject.Find(affectedObjectPath);
                    if (foundGameObject)
                    {
                        affectedObject = foundGameObject.transform;
                        foreach (var timeline in Timelines)
                            timeline.LateBindAffectedObjectInScene(affectedObject);
                    }
                }

                return affectedObject;
            }
            set
            {
                affectedObject = value;

                if (affectedObject != null)
                    affectedObjectPath = affectedObject.transform.GetFullHierarchyPath();

                RenameTimelineContainer();
            }
        }

        /// <summary>
        /// 关联的时间序列
        /// </summary>
        private JSequencer sequence;
        public JSequencer Sequence
        {
            get
            {
                if (sequence)
                    return sequence;

                sequence = transform.parent.GetComponent<JSequencer>();
                return sequence;
            }
        }

        /// <summary>
        /// 包含的所有的时间线
        /// </summary>
        private JTimelineBase[] timelines;
        public JTimelineBase[] Timelines
        {
            get
            {
                if (timelines != null)
                    return timelines;

                timelines = GetComponentsInChildren<JTimelineBase>();
                return timelines;
            }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        public string AffectedObjectPath
        {
            get { return affectedObjectPath; }
            private set { affectedObjectPath = value; }
        }

        #endregion

        private void Start()
        {
            if (affectedObjectPath == null)
            {
                affectedObjectPath = string.Empty;
            }
            else
            {
                if (AffectedObject == null && affectedObjectPath.Length != 0)
                {
                    GameObject affectedGameObject = GameObject.Find(affectedObjectPath);
                    AffectedObject = affectedGameObject.transform;
                }
            }

        }

        #region ExtensionRegion
        public JTimelineBase AddNewTimeline(TimeLineType type)
        {
            JTimelineBase timeline = null;
            string name = Enum.GetName(typeof(TimeLineType), type);
            UnityEngine.Transform line = transform.Find(name + "Timeline for " + affectedObject.name);
            if (line == null)
            {
                GameObject newTimeline = new GameObject(name + "Timeline for " + affectedObject.name);
                newTimeline.transform.parent = transform;
                line = newTimeline.transform;
            }

            if (type == TimeLineType.Animation && (timeline = line.GetComponent<JTimelineAnimation>()) == null)
            {
                timeline = line.gameObject.AddComponent<JTimelineAnimation>();
            }
            if (type == TimeLineType.Effect && (timeline = line.GetComponent<JTimelineParticle>()) == null)
            {
                timeline = line.gameObject.AddComponent<JTimelineParticle>();
            }
            if (type == TimeLineType.Sound && (timeline = line.GetComponent<JTimelineSound>()) == null)
            {
                timeline = line.gameObject.AddComponent<JTimelineSound>();
            }
            if (type == TimeLineType.Transform && (timeline = line.GetComponent<JTimelineTransform>()) == null)
            {
                timeline = line.gameObject.AddComponent<JTimelineTransform>();
            }
            if (type == TimeLineType.Event && (timeline = line.GetComponent<JTimelineEvent>()) == null)
            {
                timeline = line.gameObject.AddComponent<JTimelineEvent>();
            }
            if (type == TimeLineType.Trajectory && (timeline = line.GetComponent<JTimelineTrajectory>()) == null)
            {
                timeline = line.gameObject.AddComponent<JTimelineTrajectory>();
            }
            if (type == TimeLineType.CameraAction && (timeline = line.GetComponent<JTimelineCamera>()) == null)
            {
                timeline = line.gameObject.AddComponent<JTimelineCamera>();
            }
            return timeline;
        }
        #endregion

        public void ProcessTimelines(float sequencerTime, float playbackRate)
        {
            if (!gameObject.activeInHierarchy)
                return;
            foreach (JTimelineBase timeline in Timelines)
                timeline.Process(sequencerTime, playbackRate);
        }

        public void SkipTimelineTo(float sequencerTime)
        {
            foreach (JTimelineBase timeline in Timelines)
                timeline.SkipTimelineTo(sequencerTime);
        }

        public void ManuallySetTime(float sequencerTime)
        {
            foreach (JTimelineBase timeline in Timelines)
                timeline.ManuallySetTime(sequencerTime);
        }


        public void RenameTimelineContainer()
        {
            if (affectedObject)
                name = "Timelines for " + affectedObject.name;
        }

        public void ResetCachedData()
        {
            sequence = null;
            timelines = null;
            foreach (var timeline in Timelines)
                timeline.ResetCachedData();
        }
    }
}
 
