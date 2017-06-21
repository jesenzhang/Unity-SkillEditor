using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
namespace CySkillEditor
{
    [Serializable]
    public class SkillEffectUnit
    {
        //特效
        public SkillArtEffect artEffect = new SkillArtEffect();
        //配置
        public EffectConfigure configure = new EffectConfigure();

        public SkillEffectUnit Copy()
        {
            SkillEffectUnit b = new SkillEffectUnit();
            b.configure = configure.Copy();
            b.artEffect = artEffect.Copy();
            return b;
        }
    }
}