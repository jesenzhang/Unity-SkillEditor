using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Collections.Generic;

namespace CySkillEditor
{
    [Serializable]
    public class JCameraClipData : ScriptableObject
    {
        [SerializeField]
        private Camera targetCamera;
      
        public Camera TargetCamera
        {
            get {
                if (targetCamera == null)
                    targetCamera = Camera.main;
                return targetCamera;
            }
            set {
                targetCamera = value;
            }
        }
        public bool active = false;
        private Vector3 originPos;
        private float beginShakeTime = 0;
        List<Component> components = new List<Component>();
        [SerializeField]
        private SkillCameraAction action;
        public SkillCameraAction Action
        {
            get
            {
                if (action == null)
                    action = new SkillCameraAction();
                return action;
            }
            set
            {
                action = value;
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
        private float playbackDuration;
        public float PlaybackDuration
        {
            get { return playbackDuration; }
            set
            {
                playbackDuration = value;
            }
        }
        
        [SerializeField]
        private bool looping;
        public bool Looping
        {
            get { return looping; }
            set { looping = value; }
        }
       
        [SerializeField]
        private JCameraTrack track;
        public JCameraTrack Track
        {
            get { return track; }
            set
            {
                track = value;
            }
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
        
        public float EndTime
        {
            get { return startTime + playbackDuration; }
            private set {; }
        }

        [SerializeField]
        private string actionName = "NotSet";
        public string ActionName
        {
            get {
                actionName = Enum.GetName(typeof(SkillCameraAction.CameraAction), Action.action);
                return actionName; }
            set
            {
                actionName = value;
                FriendlyName = actionName;
            }
        }
        public string FriendlyName
        {
            get { return (ActionName); }
            private set {; }
        }

   

        public void Init()
        {
            if (active)
                return;
            if (TargetCamera == null)
            {
                TargetCamera = Camera.main;
            }
            originPos = TargetCamera.transform.position;
            beginShakeTime = startTime;
            if(action.action == SkillCameraAction.CameraAction.CAMERAACTION_BLUR)
            {
                CameraFilterPack_Blur_Focus cc = TargetCamera.gameObject.AddComponent<CameraFilterPack_Blur_Focus>();
                components.Add(cc);
            }
    
            active = true;
        }
        public void OnUpdate(float time)
        {
            if (active)
            {
                if (action.action == SkillCameraAction.CameraAction.CAMERAACTION_SHAKE)
                {
                    if ((time - beginShakeTime)*1000f> Action.shakeInterval)
                    {
                        TargetCamera.transform.position = originPos + UnityEngine.Random.insideUnitSphere * Action.shakeRange;

                        beginShakeTime = time;
                    }
                }
                
            }
        }
        public void Reset()
        {
            TargetCamera.transform.position = originPos;
            beginShakeTime = 0;
            foreach(var cc in components)
            {
                DestroyImmediate(cc);
            }
            components.Clear();
            active = false;
        }

        public delegate bool StateCheck(float sequencerTime, JCameraClipData clipData);

        public static bool IsClipNotRunning(float sequencerTime, JCameraClipData clipData)
        {
            return sequencerTime < clipData.StartTime;
        }

        public static bool IsClipRunning(float sequencerTime, JCameraClipData clipData)
        {
            return sequencerTime > clipData.StartTime && sequencerTime < clipData.EndTime;
        }

        public static bool IsClipFinished(float sequencerTime, JCameraClipData clipData)
        {
            return sequencerTime >= clipData.EndTime;
        }
    }
}