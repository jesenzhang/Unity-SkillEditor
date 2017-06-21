using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
namespace CySkillEditor
{
   
    [Serializable]
    public class EffectConfigure
    {
        public enum LifeTime
        {
            AUTO = 0,
            GLOBAL = 1
        }

        public enum PosType
        {
            FEET = 0,
            BODY = 1,
            HEAD = 2,
            BONE = 3,
            RELATIVE = 4,
            WORLD = 5
        }

        public float bodyHeight;
        public float headHeight;
        public float feetWidth;
        public string effectName = "";
        //技能位置类型
        public PosType posType = PosType.FEET;
        //位置
        public Vector3 position = Vector3.zero;
        //旋转
        public Vector3 rotation = Vector3.zero;
        //位置类型为骨骼的名称
        public string boneName = "";
        //生命周期类型
        public LifeTime lifeTime = LifeTime.AUTO;

        public EffectConfigure Copy()
        {
            EffectConfigure b = new EffectConfigure();
            b.bodyHeight = bodyHeight;
            b.headHeight = headHeight;
            b.feetWidth = feetWidth;
            b.effectName = effectName;
            b.posType = posType;
            b.position = position;
            b.rotation = rotation;
            b.boneName = boneName;
            b.lifeTime = lifeTime;
            return b;
        }
    }
}