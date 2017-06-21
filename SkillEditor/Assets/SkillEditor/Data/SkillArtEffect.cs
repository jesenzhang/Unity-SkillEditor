using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
namespace CySkillEditor
{
    [Serializable]
    public class SkillArtEffect
    {
        public enum EffPos
        {
            FEET = 0,
            BODY = 1,
            HEIGHT = 2
        }
        //特效id
        public int effect;
        public GameObject effectObj;
        public int beginTime = 0;
        public int phaseTime = 1000;
        //	技能的动作和特效持续时间			
        public EffPos effPos;
        //	特效的绑定点	
        public float height = 0f;
        
        public SkillArtEffect Copy()
        {
            SkillArtEffect b = new SkillArtEffect();
            b.effectObj = effectObj;
            b.effect = effect;
            b.beginTime = beginTime;
            b.phaseTime = phaseTime;
            b.effPos = effPos;
            b.height = height;
            return b;
        }
    }
}