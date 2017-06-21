using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CySkillEditor
{
    [Serializable]
    public class SkillCameraAction
    {
        public enum CameraAction
        {
            CAMERAACTION_NONE = 0,
            CAMERAACTION_Y = 1,
            CAMERAACTION_SHAKE = 2,
            CAMERAACTION_BLUR = 3
        }

        public CameraAction action = CameraAction.CAMERAACTION_Y;
        public float param;
        public float delay = 0;

        //新增参数
        //震动参数
        public float shakeRange = 1;
        public int shakeInterval = 100;
        public float phaseTime = 0.5f;
        public SkillCameraAction Copy()
        {
            SkillCameraAction b = new SkillCameraAction();
            b.action = action;
            b.param = param;
            b.shakeRange = shakeRange;
            b.shakeInterval = shakeInterval;
            b.delay = delay;
            b.phaseTime = phaseTime;
            return b;
        }
    }
}