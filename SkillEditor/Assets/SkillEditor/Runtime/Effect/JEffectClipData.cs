using UnityEngine;
using System;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace CySkillEditor
{
    [Serializable]
    public class JEffectClipData : ScriptableObject
    {
    
        [SerializeField]
        //特效类型
        public EffectType effectType = EffectType.None;
        [HideInInspector]
        [SerializeField]
        //数据保存字符串
        public string dataString = "";
        [SerializeField]
        //数据对象 使用时转换类型
        public object DataObj;

        [SerializeField]
        //是否激活
        public bool active = false;
        
        [SerializeField]
        public Transform Target;
        [SerializeField]
        public SkillEffectUnit effectunit;
        [SerializeField]
        public JSkillUnit skillunit;
        [SerializeField]
        public Camera TargetCamera;
        [SerializeField]
        public SkillCameraAction cameraAction;
        

        [SerializeField]
        public int layer;
        [SerializeField]
        public RuntimeAnimatorController animController;

        [SerializeField]
        public AudioClip sound;

        [HideInInspector]
        private JEffectBase effectAction;
        public JEffectBase EffectAction
        {
            get
            {
                if (effectAction == null)
                    effectAction = new JEffectBase();
                return effectAction;

            }
            set
            {
                effectAction = value;
            }
        }

        public void Init()
        {
            if (active)
                return;
            
            if (effectType == EffectType.Particle)
            {
                if (!(effectAction is JEffectParticle) || effectAction==null)
                {
                    effectAction = new JEffectParticle();
                }
                List<object> param = new List<object>();
                param.Add(effectunit);
                param.Add(TargetObject);
                ((JEffectParticle)EffectAction).SetData(param.ToArray());
                ((JEffectParticle)EffectAction).Init();
            }
            else
            if (effectType == EffectType.Trajectory)
            {
                if (!(effectAction is JEffectTrajectory) || effectAction == null)
                {
                    effectAction = new JEffectTrajectory();
                }
                    List<object> param = new List<object>();
                    param.Add(Target);
                    param.Add(TargetObject);
                    param.Add(skillunit);
                    param.Add(effectunit);
                    ((JEffectTrajectory)EffectAction).SetData(param.ToArray());
                    ((JEffectTrajectory)EffectAction).Init();
                if (PlaybackDuration != effectunit.artEffect.phaseTime / 1000f)
                {
                    PlaybackDuration = effectunit.artEffect.phaseTime / 1000f;
                }
            }
            else
            if (effectType == EffectType.Camera)
            {
                if (!(effectAction is JEffectCamera) || effectAction == null)
                {
                    effectAction = new JEffectCamera();
                }
                List<object> param = new List<object>();
                param.Add(TargetCamera);
                param.Add(cameraAction);
                ((JEffectCamera)EffectAction).SetData(param.ToArray());
                ((JEffectCamera)EffectAction).Init();
            }else
            if (effectType == EffectType.Animation)
            {
                if (!(effectAction is JEffectAnimation) || effectAction == null)
                {
                    effectAction = new JEffectAnimation();
                }
                List<object> param = new List<object>();
                param.Add(TargetObject);
                param.Add(layer);
                param.Add(stateName);
                param.Add(playbackDuration);
                ((JEffectAnimation)EffectAction).SetData(param.ToArray());
                ((JEffectAnimation)EffectAction).Init();
            }
            else
            if (effectType == EffectType.Sound)
            {
                if (!(effectAction is JEffectSound) || effectAction == null)
                {
                    effectAction = new JEffectSound();
                }
                List<object> param = new List<object>();
                param.Add(TargetObject);
                param.Add(sound);
                param.Add(playbackDuration);
                ((JEffectSound)EffectAction).SetData(param.ToArray());
                ((JEffectSound)EffectAction).Init();
            }
            else
            {
                if(effectAction==null)
                    effectAction = new JEffectBase();
            }
           
            active = true;
        }
        public void OnUpdate(float time)
        {
            if (active)
            {
                if (EffectAction != null)
                {
                    if (effectAction is JEffectTrajectory)
                    {
                        ((JEffectTrajectory)EffectAction).OnUpdate(time);
                    }
                    if (effectAction is JEffectParticle)
                    {
                        ((JEffectParticle)EffectAction).OnUpdate(time);
                    }
                    if (effectAction is JEffectCamera)
                    {
                        ((JEffectCamera)EffectAction).OnUpdate(time);
                    }
                    if (effectAction is JEffectAnimation)
                    {
                        ((JEffectAnimation)EffectAction).OnUpdate(time);
                    }
                    if (effectAction is JEffectSound)
                    {
                        ((JEffectSound)EffectAction).OnUpdate(time);
                    }
                }
            }
        }
        public void Reset()
        {
            if (EffectAction != null)
            {
                if (effectAction is JEffectTrajectory)
                {
                    ((JEffectTrajectory)EffectAction).Reset();
                }
                if (effectAction is JEffectParticle)
                {
                    ((JEffectParticle)EffectAction).Reset();
                }
                if (effectAction is JEffectCamera)
                {
                    ((JEffectCamera)EffectAction).Reset();
                }
                if (effectAction is JEffectAnimation)
                {
                    ((JEffectAnimation)EffectAction).Reset();
                }
                if (effectAction is JEffectSound)
                {
                    ((JEffectSound)EffectAction).Reset();
                }

            }
            active = false;
        }
        public T GetDataObj<T>()
        {
            T obj = JsonUtility.FromJson<T>(dataString);
            return obj;
        }
        public void SaveDataObj<T>(object obj)
        {
            dataString = JsonUtility.ToJson((T)obj);
        }
        [SerializeField]
        private float startTime;
        public float StartTime
        {
            get { return startTime; }
            set
            {
                startTime = value;
                if (effectType == EffectType.Particle || effectType == EffectType.Trajectory)
                {
                    effectunit.artEffect.beginTime = (int)(startTime * 1000f);
                }
                else if (effectType == EffectType.Camera)
                {
                    cameraAction.delay = startTime;
                }
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
                if (effectType == EffectType.Particle || effectType == EffectType.Trajectory)
                {
                    effectunit.artEffect.phaseTime = (int)(playbackDuration * 1000f);
                }
                else if (effectType == EffectType.Camera)
                {
                    cameraAction.phaseTime = playbackDuration;
                }
            }
        }

        [SerializeField]
        private float effectDuration;
        public float EffectDuration
        {
            get { return effectDuration; }
            set
            {
                effectDuration = value;
            }
        }
        [SerializeField]
        private bool looping;
        public bool Looping
        {
            get { return looping; }
            set { looping = value; }
        }
        [HideInInspector]
        [SerializeField]
        private GameObject targetObject;
        public GameObject TargetObject
        {
            get {
                if (targetObject == null)
                    if(Track!=null)
                        targetObject = Track.TimeLine.AffectedObject.gameObject;
                return targetObject; }
            set
            {
                targetObject = value;
            }
        }
        [SerializeField]
        private JEffectTrack track;
        public JEffectTrack Track
        {
            get { return track; }
            set
            {
                track = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        private string stateName = "NotSet";
        public string StateName
        {
            get
            {
                if ((effectType == EffectType.Particle || effectType == EffectType.Trajectory) && (effectunit != null && effectunit.artEffect != null && effectunit.artEffect.effectObj != null))
                    stateName = effectunit.artEffect.effectObj.name;
                else if (effectType == EffectType.Camera && cameraAction!=null)
                {
                    stateName = Enum.GetName(typeof(SkillCameraAction.CameraAction), cameraAction.action);
                }
                if (effectType == EffectType.Sound && sound != null)
                {
                    stateName = sound.name;
                }
                return stateName;
            }
            set
            {
                stateName = value;
                FriendlyName = MakeFriendlyStateName(StateName);
            }
        }
        public string FriendlyName
        {
            get { return MakeFriendlyStateName(StateName); }
            private set {; }
        }
        public static string MakeFriendlyStateName(string stateName)
        {
            return stateName;
        }
        public float EndTime
        {
            get { return startTime + playbackDuration; }
            private set {; }
        }
        public delegate bool StateCheck(float sequencerTime, JEffectClipData clipData);

        public static bool IsClipNotRunning(float sequencerTime, JEffectClipData clipData)
        {
            return sequencerTime < clipData.StartTime;
        }

        public static bool IsClipRunning(float sequencerTime, JEffectClipData clipData)
        {
            return sequencerTime > clipData.StartTime && sequencerTime < clipData.EndTime;
        }

        public static bool IsClipFinished(float sequencerTime, JEffectClipData clipData)
        {
            return sequencerTime >= clipData.EndTime;
        }
    }
}