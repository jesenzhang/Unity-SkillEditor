using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Collections.Generic;

namespace CySkillEditor
{
    [Serializable]
    public class JParticleClipData : ScriptableObject
    {
        public GameObject Effect;
        [HideInInspector]
        public GameObject EffectObj;
        public List<ParticleSystem> particleSys = new List<ParticleSystem>();
        public bool active = false;
        [HideInInspector]
        [SerializeField]
        private EffectConfigure effectConfig;
        public EffectConfigure EffectConfig
        {
            get {
                if(effectConfig==null)
                    effectConfig = new EffectConfigure();
                return effectConfig; }
            set
            {
                effectConfig = value;
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
        private bool crossFade;
        public bool CrossFade
        {
            get { return crossFade; }
            set { crossFade = value; }
        }
        [SerializeField]
        private bool looping;
        public bool Looping
        {
            get { return looping; }
            set { looping = value; }
        }
        [SerializeField]
        private float transitionDuration;
        public float TransitionDuration
        {
            get { return transitionDuration; }
            set { transitionDuration = value; }
        }
        [SerializeField]
        private JParticleTrack track;
        public JParticleTrack Track
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


        public int Index
        {
            get;
            set;
        }

      
        public float EndTime
        {
            get { return startTime + playbackDuration; }
            private set {; }
        }

        [SerializeField]
        private string particleName = "NotSet";
        public string ParticleName
        {
            get { return particleName; }
            set
            {
                particleName = value;
                FriendlyName = particleName;
            }
        }
        public string FriendlyName
        {
            get { return (particleName); }
            private set {; }
        }
        public void Init()
        {
            if (active)
                return;
            if (Effect == null)
                Effect = TargetObject.transform.Find(ParticleName).gameObject;
            if (Effect == null)
                return;
            if (particleName != Effect.name)
                return;
            if (EffectObj)
            {
                GameObject.DestroyImmediate(EffectObj);
            }
            particleSys.Clear();
            EffectObj = GameObject.Instantiate(Effect);
            ParticleSystem[] particleSys0 = EffectObj.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particleSys0.Length; i++)
            {
                particleSys0[i].Stop();
                particleSys0[i].Simulate(0);
                particleSys0[i].Clear();
                if (ParticleSystemUtility.IsRoot(particleSys0[i]))
                {
                    particleSys.Add(particleSys0[i]);
                }
            }
            float offsetY = 0;
            Vector3 offset = EffectConfig.position;
            //世界坐标
            if (EffectConfig.posType == CySkillEditor.EffectConfigure.PosType.WORLD)
            {
                EffectObj.transform.position = offset;
            }
            else //世界坐标 相对于主角
            if (EffectConfig.posType == CySkillEditor.EffectConfigure.PosType.RELATIVE)
            {
                EffectObj.transform.position = TargetObject.transform.position+ offset;
            }
            else
            {
                //附着于主角身上
                EffectObj.transform.SetParent(Track.TimeLine.AffectedObject.transform);
                if (EffectConfig.posType == CySkillEditor.EffectConfigure.PosType.FEET)
                {
                    offsetY = 0;
                }
                else if (EffectConfig.posType == CySkillEditor.EffectConfigure.PosType.BODY)
                {
                    offsetY = EffectConfig.bodyHeight;
                }
                else if (EffectConfig.posType == CySkillEditor.EffectConfigure.PosType.HEAD)
                {
                    offsetY = EffectConfig.headHeight;
                }
                else if (EffectConfig.posType == CySkillEditor.EffectConfigure.PosType.BONE)
                {
                    Transform bone = TargetObject.transform.Find(EffectConfig.boneName);
                    if (bone)
                        EffectObj.transform.SetParent(bone);
                }
                EffectObj.transform.localPosition = new Vector3(0, offsetY, 0);
                EffectObj.transform.localPosition += offset;
            }
            EffectObj.transform.rotation = Quaternion.Euler(EffectConfig.rotation);
            active = true;
        }
        public  void OnUpdate(float time)
        { 
            if (EffectObj != null && active)
            {
                foreach (var p in particleSys)
                {
                    p.Simulate(time, true,false);
                }
            }
        }
        public void Reset()
        {
            active = false;
            if(EffectObj)
            {
                GameObject.DestroyImmediate(EffectObj);
            }
            particleSys.Clear();
    }

        public delegate bool StateCheck(float sequencerTime, JParticleClipData clipData);

        public static bool IsClipNotRunning(float sequencerTime, JParticleClipData clipData)
        {
            return sequencerTime < clipData.StartTime;
        }

        public static bool IsClipRunning(float sequencerTime, JParticleClipData clipData)
        {
            return sequencerTime > clipData.StartTime && sequencerTime < clipData.EndTime;
        }

        public static bool IsClipFinished(float sequencerTime, JParticleClipData clipData)
        {
            return sequencerTime >= clipData.EndTime;
        }
    }
}