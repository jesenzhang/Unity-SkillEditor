
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace CySkillEditor
{
    public class JEffectAnimation : JEffectBase
    {
        private Dictionary<int, List<AnimatorClipInfo>> initialAnimationInfo = new Dictionary<int, List<AnimatorClipInfo>>();
        private Dictionary<int, AnimatorStateInfo> initialAnimatorStateInfo = new Dictionary<int, AnimatorStateInfo>();

        public GameObject TargetObject;
        public float PlaybackDuration = 0;
        public bool Looping = false;
        public int Layer = 0;
        public string StateName;
        public RuntimeAnimatorController controller;

        public float RunningTime = 0;
        
        private Vector3 sourcePosition;
        private Quaternion sourceOrientation;
        private float sourceSpeed;
        private bool previousEnabled;
 

        [SerializeField]
        private Animator animator;
        private Animator Animator
        {
            get
            {
                if (animator == null)
                {
                    animator = TargetObject.GetComponent<Animator>();
                    if (animator != null)
                    {
                        if (animator.runtimeAnimatorController == null || animator.runtimeAnimatorController != controller)
                        {
                            animator.runtimeAnimatorController = controller;
                        }
                    }
                }

                return animator;
            }
        }
        
        public bool active = false;

        public override void SetData(object[] data)
        {
            TargetObject = (GameObject)data[0];
            Layer = (int)data[1];
            StateName = (string)data[2];
            PlaybackDuration = (float)data[3];
        }

        public override void Init()
        {
            if (active)
                return;
            InitAnimationState();
            previousEnabled = Animator.enabled;
            Animator.enabled = false;

            active = true;
        }
        public override void OnUpdate(float time)
        {
            if (active)
            {
                PlayClip(Layer, time);
                Animator.Update(time);
            }
          
        }
        public override void Reset()
        {
            if (Animator == null)
                return;
            Animator.Update(-0.015f);
            Animator.StopPlayback();

            ResetAnimation();

            initialAnimationInfo.Clear();
            initialAnimatorStateInfo.Clear();
            RunningTime = 0;
            Animator.speed = sourceSpeed;
            Animator.enabled = previousEnabled;
            active = false;
        }
        private void InitAnimationState()
        {
            if (Animator == null)
                return;
            sourcePosition = TargetObject.transform.localPosition;
            sourceOrientation = TargetObject.transform.localRotation;
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
            if (Animator == null)
                return;
            for (int layer = 0; layer < Animator.layerCount; layer++)
            {
                if (!initialAnimatorStateInfo.ContainsKey(layer))
                    continue;
                Animator.Play(initialAnimatorStateInfo[layer].fullPathHash, layer, 0);// initialAnimatorStateInfo[layer].normalizedTime);
                Animator.Update(0.0f);
            }
            TargetObject.transform.localPosition = sourcePosition;
            TargetObject.transform.localRotation = sourceOrientation;
            Animator.speed = sourceSpeed;
        }
        private void PlayClip(int layer, float sequenceTime)
        {
            if (Animator == null)
                return;
            RunningTime += sequenceTime;
            float normalizedTime = RunningTime / PlaybackDuration;

            if (Looping)
            {
                normalizedTime = ((RunningTime) % PlaybackDuration) / PlaybackDuration;
            }
             Animator.Play(StateName, layer, normalizedTime);
            
        }


    }
}