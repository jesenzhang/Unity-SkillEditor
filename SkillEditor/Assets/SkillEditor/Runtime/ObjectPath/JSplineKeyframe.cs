using UnityEngine;
using System;
using System.Collections;
namespace CySkillEditor
{
    [Serializable]
    public class JSplineKeyframe : ScriptableObject
    {
        [SerializeField]
        private JTransformTrack track;
        public JTransformTrack Track
        {
            get { return track; }
            set
            {
                track = value;
            }
        }

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

        [SerializeField]
        private Vector3 normal = Vector3.zero;
        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; }
        }


        [SerializeField]
        private Vector3 tangent = Vector3.zero;
        public Vector3 Tangent
        {
            get { return tangent; }
            set { tangent = value; }
        }

        [SerializeField]
        private Vector3 position = Vector3.zero;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        [SerializeField]
        private Vector3 rotation = Vector3.zero;
        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        [SerializeField]
        private Vector3 scale = Vector3.zero;
        public Vector3 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public static int Comparer(JSplineKeyframe a, JSplineKeyframe b)
        {
            return (a.StartTime.CompareTo(b.StartTime));
        }
    }
}
