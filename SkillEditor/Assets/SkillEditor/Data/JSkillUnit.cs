using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
namespace CySkillEditor
{
    public enum ModelTargetType
    {
        Player = 0,
        NPC = 1
    }

    [Serializable]
    public class JSkillUnit
    {
        public enum LaunchType
        {
            //	此条件无需判断
            LAUNCH_NONE = 0,
            //	单线
            SINGLELINE = 1,
            //	多线
            MULLINE = 2,
            //	近战攻击
            MELEE = 3,
            //	旋转
            ROTATION = 4,
            //	区域
            AREA = 5,
            //	跳跃
            JUMP = 6,
            //	曲线
            HELIX = 7,
            //	范围随机
            AREA_RANDSKILL = 8,
            //	跟随
            FOLLOW = 9,
            //背刺
            BACK_STAB = 10
        }
        public enum TargetType
        {
            //	0.自己
            MYSELF = 0,
            //	友方
            FRIEND = 1,
            //	敌方
            ENEMY = 2
        }
        public enum ReferPoint
        {
            SELF = 0,
            POSITION = 1,
            TARGET = 2
        }
        public enum BasePoint
        {
            CENTER = 0,
            EDGE = 1
        }
        //	静态ID, 从1开始 [1 - ~]
        public int id ;
            //	子表引用Id
        public int referId;
        //	发射类型
        public LaunchType launchType;
        //	技能美术表现ID
        public int artId;
        //	技能CD MS
        public int cd;
        // 	技能时间 fireActionTime
        public int skillTime;
        //	施法判定距离													
        public float distance;
        //	目标类型
        public TargetType targetType;
        //弹道类型
        private SkillObj _skillObj;
        public SkillObj skillObj
        {
            get
            {
                if (_skillObj is SkillLine)
                {
                    return (SkillLine)_skillObj;
                }
                if (_skillObj is SkillMultiLine)
                {
                    return (SkillMultiLine)_skillObj;
                }
                if (_skillObj is SkillJump)
                {
                   return (SkillJump)_skillObj;
                }
                if (_skillObj is SkillHelix)
                {
                   return (SkillHelix)_skillObj;
                }
                if (_skillObj is SkillFollow)
                {
                    return (SkillFollow)_skillObj;
                }
                if (_skillObj is SkillBackStab)
                {
                    return (SkillBackStab)_skillObj;
                }
                if (_skillObj is SkillArea)
                {
                   return (SkillArea)_skillObj;
                }
                if (_skillObj is SkillAreaRand)
                {
                   return (SkillAreaRand)_skillObj;
                }
                coverToObj();
                return _skillObj;
            }
            set {
                _skillObj = value;
                coverToString();

            }
        }
        // 	技能引导策略
        public SkillGuidePolicy guidePolicy;

        public string SkillData = "";
        private void OnEnable()
        {
            guidePolicy = new SkillGuidePolicy();
        }

        private void coverToString()
        {
            if (_skillObj is SkillLine)
            {
                SkillData = JsonUtility.ToJson((SkillLine)_skillObj);
            }
            if (_skillObj is SkillMultiLine)
            {
                SkillData = JsonUtility.ToJson((SkillMultiLine)_skillObj);
            }
            if (_skillObj is SkillJump)
            {
                SkillData = JsonUtility.ToJson((SkillJump)_skillObj);
            }
            if (_skillObj is SkillHelix)
            {
                SkillData = JsonUtility.ToJson((SkillHelix)_skillObj);
            }
            if (_skillObj is SkillFollow)
            {
                SkillData = JsonUtility.ToJson((SkillFollow)_skillObj);
            }
            if (_skillObj is SkillBackStab)
            {
                SkillData = JsonUtility.ToJson((SkillBackStab)_skillObj);
            }
            if (_skillObj is SkillArea)
            {
                SkillData = JsonUtility.ToJson((SkillArea)_skillObj);
            }
            if (_skillObj is SkillAreaRand)
            {
                SkillData = JsonUtility.ToJson((SkillAreaRand)_skillObj);
            }

        }
        private void coverToObj()
        {
            if (launchType == LaunchType.SINGLELINE)
            {
                _skillObj = JsonUtility.FromJson<SkillLine>(SkillData);
            }
            if (launchType == LaunchType.MULLINE)
            {
                _skillObj = JsonUtility.FromJson<SkillMultiLine>(SkillData);
            }
            if (launchType == LaunchType.JUMP)
            {
                _skillObj = JsonUtility.FromJson<SkillJump>(SkillData);
            }
            if (launchType == LaunchType.HELIX)
            {
                _skillObj = JsonUtility.FromJson<SkillHelix>(SkillData);
            }
            if (launchType == LaunchType.FOLLOW)
            {
                _skillObj = JsonUtility.FromJson<SkillFollow>(SkillData);
            }
            if (launchType == LaunchType.BACK_STAB)
            {
                _skillObj = JsonUtility.FromJson<SkillBackStab>(SkillData);
            }
            if (launchType == LaunchType.AREA)
            {
                _skillObj = JsonUtility.FromJson<SkillArea>(SkillData);
            }
            if (launchType == LaunchType.AREA_RANDSKILL)
            {
                _skillObj = JsonUtility.FromJson<SkillAreaRand>(SkillData);
            }

        }
        public JSkillUnit Copy()
        {
            JSkillUnit b = new JSkillUnit();
            b.id = id;
            b.referId = referId;
            b.launchType = launchType;
            b.artId = artId;
            b.cd = cd;
            b.skillTime = skillTime;
            b.distance = distance;
            b.targetType = targetType;
            b.SkillData = SkillData;
            if (skillObj is SkillLine)
            {
                b.skillObj = ((SkillLine)skillObj).Copy();
            }
            if (skillObj is SkillMultiLine)
            {
                b.skillObj = ((SkillMultiLine)skillObj).Copy();
            }
            if (skillObj is SkillJump)
            {
                b.skillObj = ((SkillJump)skillObj).Copy();
            }
            if (skillObj is SkillHelix)
            {
                b.skillObj = ((SkillHelix)skillObj).Copy();
            }
            if (skillObj is SkillFollow)
            {
                b.skillObj = ((SkillFollow)skillObj).Copy();
            }
            if (skillObj is SkillBackStab)
            {
                b.skillObj = ((SkillBackStab)skillObj).Copy();
            }
            if (skillObj is SkillArea)
            {
                b.skillObj = ((SkillArea)skillObj).Copy();
            }
            if (skillObj is SkillAreaRand)
            {
                b.skillObj = ((SkillAreaRand)skillObj).Copy();
            }

            if (guidePolicy!=null)
                b.guidePolicy = guidePolicy.Copy();

            return b;
        }
    }

}