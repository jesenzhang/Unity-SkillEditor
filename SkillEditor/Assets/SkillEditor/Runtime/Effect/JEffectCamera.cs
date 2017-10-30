using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace CySkillEditor
{
    public class JEffectCamera : JEffectBase
    {
        public Camera TargetCamera;
        public SkillCameraAction action;

        public bool active = false;
        private Vector3 originPos;
        private float beginShakeTime = 0;
        List<Component> components = new List<Component>();
        [SerializeField]
       


        public override void SetData(object[] data)
        {
            TargetCamera = (Camera)data[0];
            action = (SkillCameraAction)data[1];
        }

        public override void Init()
        {
            if (active)
                return;
            if (TargetCamera == null)
            {
                TargetCamera = Camera.main;
            }
            originPos = TargetCamera.transform.position;
            beginShakeTime = 0;
            if (action.action == SkillCameraAction.CameraAction.CAMERAACTION_BLUR)
            {
                CameraFilterPack_Blur_Focus cc = TargetCamera.gameObject.AddComponent<CameraFilterPack_Blur_Focus>();
                components.Add(cc);
            }

            active = true;
        }
        public override void OnUpdate(float time)
        {
            if (active)
            {
                beginShakeTime += time;
                if (action.action == SkillCameraAction.CameraAction.CAMERAACTION_SHAKE)
                {
                    if ((beginShakeTime) * 1000f > action.shakeInterval)
                    {
                        TargetCamera.transform.position = originPos + UnityEngine.Random.insideUnitSphere * action.shakeRange;

                        beginShakeTime = 0;
                    }
                }

            }
        }
        public override void Reset()
        {
            TargetCamera.transform.position = originPos;
            beginShakeTime = 0;
            foreach (var cc in components)
            {
                GameObject.DestroyImmediate(cc);
            }
            components.Clear();
            active = false;
        }


    }
}