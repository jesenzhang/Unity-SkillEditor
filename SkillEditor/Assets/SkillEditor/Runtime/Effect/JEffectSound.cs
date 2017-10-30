using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CySkillEditor
{
    public class JEffectSound : JEffectBase
    {
        public AudioClip audioClip;
        public GameObject TargetObject;
        private AudioSource AudioSourceObj;
        public float PlaybackDuration;
        private bool active = false;
        private float RunningTime = 0;

        public override void SetData(object[] data)
        {
            TargetObject = (GameObject)data[0];
            audioClip = (AudioClip)data[1];
            PlaybackDuration = (float)data[2];
        }

        public override void Init()
        {
            if (active)
                return;
            if (AudioSourceObj)
            {
                GameObject.DestroyImmediate(AudioSourceObj.gameObject);
            }
            if(AudioSourceObj==null)
                AudioSourceObj = new GameObject("AudioSourceObj").AddComponent<AudioSource>();
            Debug.Log(AudioSourceObj +"  "+ TargetObject);
            AudioSourceObj.gameObject.transform.SetParent(TargetObject.transform);
            AudioSourceObj.gameObject.transform.position = Vector3.zero;
            AudioSourceObj.clip = audioClip;
            RunningTime = 0;
            active = true;
        }
        public override void OnUpdate(float time)
        {
            if (AudioSourceObj != null && audioClip!=null && active)
            {
                PlayClip(audioClip, time);
            }
        }
        public override void Reset()
        {
            active = false;
            if (AudioSourceObj!=null)
            {
                GameObject.DestroyImmediate(AudioSourceObj.gameObject);
            }
            RunningTime = 0;
        }
        private void PlayClip(AudioClip clipToPlay, float sequenceTime)
        {
            if (AudioSourceObj == null)
                return;
            RunningTime += sequenceTime;
            if (AudioSourceObj.clip != clipToPlay)
            {
                AudioSourceObj.clip = clipToPlay;
            }

            float normalizedTime = RunningTime / PlaybackDuration;
            AudioSourceObj.pitch = clipToPlay.length / PlaybackDuration;
            normalizedTime = Mathf.Clamp(normalizedTime * clipToPlay.length, 0, clipToPlay.length);

            if ((clipToPlay.length - normalizedTime) > 0.0001f)
            {
                //if (AudioSourceObj.isPlaying)
                {
                    if (!AudioSourceObj.isPlaying)
                    {
                        AudioSourceObj.time = normalizedTime;
                        AudioSourceObj.Play();
                    }
                }
               // else
                {
                //    AudioSourceObj.time = normalizedTime;
                }
            }
        }

    }
}