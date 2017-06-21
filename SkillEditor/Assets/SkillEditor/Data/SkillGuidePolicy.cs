using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
namespace CySkillEditor
{
    [Serializable]
    public class SkillGuidePolicy
    {
        public enum GuideType
        {
            NORMAL = 1,//	guideTime + guidingTime 后进入move state
            SING = 2,//		guideTime + guidingTime 后进入move state, guideTime 结束之后读条，读条时间是 guidingTime,此时可以被打断
            GUIDE = 3//	guideTime 后进入move state，move state 和引导的 guidingTime 时间同步
        }
        public GuideType type;
        //	起手时间
        public int guideTime;
        //	引导持续时间
        public int guidingTime ;
        //	收手时间
        public int endTime;

        public SkillGuidePolicy Copy()
        {
            SkillGuidePolicy b = new SkillGuidePolicy();
            b.type = type;
            b.guideTime = guideTime;
            b.guidingTime = guidingTime;
            b.endTime = endTime;
            return b;
        }
    }
}
