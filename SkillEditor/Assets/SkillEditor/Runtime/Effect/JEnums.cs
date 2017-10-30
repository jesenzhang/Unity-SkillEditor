using System;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace CySkillEditor
{
    public enum EffectType
    {
        None = 1,
        //粒子特效
        Particle = 2,
        //屏幕特效
        Screen = 3,
        //相机特效
        Camera = 4,
        //弹道
        Trajectory = 5,

        Animation = 6,
        Sound = 7
    }
}
