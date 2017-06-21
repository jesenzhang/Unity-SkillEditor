using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
namespace CySkillEditor
{
    [Serializable]
    public class SkillObj
    {
    }
    [Serializable]
    public class SkillShape
    {
        public enum Area
        {
            //	长方形
            QUADRATE = 0,
            //	圆
            CIRCLE = 1,
            //	三角
            TRIANGLE = 2,
            //	扇形
            SECTOR = 3
        }
        public Area area;
        //	长方形的长 圆的半径 三角形的高 扇形的半径
        public float param1;
        //	长方形的宽 三角形的底
        public float param2;
        //	扇形的角度，是否倒三角
        public float param3;

        public SkillShape Copy()
        {
            SkillShape b = new SkillShape();
            b.area = area;
            b.param1 = param1;
            b.param2 = param2;
            b.param3 = param3;
            return b;
        }
    }
    [Serializable]
    public class SkillLine : SkillObj
    {
        //	静态ID, 从1开始 [1 - ~]
        public int id;
        //	运动阶段的持续时间						
        public int moveTime;
        //	运动阶段的速率
        public float speed;
        //	粒子波次
        public int waves;
        //  波次间隔时间
        public int waveDelay;
        //	技能最大作用数量
        public int maxInfluence;
        //	技能是否可穿透
        public bool canPierce;
        // 碰撞体
        public SkillShape hitArea  =new SkillShape();
        // 粒子偏移量
        public Vector3 offset;

        public SkillLine Copy()
        {
            SkillLine b = new SkillLine();
            b.id = id;
            b.moveTime = moveTime;
            b.speed = speed;
            b.waves = waves;
            b.waveDelay = waveDelay;
            b.maxInfluence = maxInfluence;
            b.canPierce = canPierce;
            b.hitArea = hitArea.Copy();
            b.offset = offset;
            return b;
        }
    }
    [Serializable]
    public class SkillMultiLine : SkillObj
    {
        //	静态ID, 从1开始 [1 - ~]
        public int id = 1;
        //	运动阶段的持续时间						
        public int moveTime = 2;
        //	运动阶段的速率
        public float speed = 3;
        //	技能粒子数, 线数
        public int unitCount = 4;
        //  粒子波次, 每条线中的粒子数
        public int waves = 5;
        //  波次间隔时间
        public int waveDelay = 6;
        //	技能最大作用数量
        public int maxInfluence = 7;
        //	技能是否可穿透
        public bool canPierce;
        // 碰撞体
        public SkillShape hitArea = new SkillShape();
        // 多线形状区域
        public SkillShape shape = new SkillShape();
        // 粒子偏移量
        public Vector3 offset;

        public SkillMultiLine Copy()
        {
            SkillMultiLine b = new SkillMultiLine();
            b.id = id;
            b.moveTime = moveTime;
            b.speed = speed;
            b.unitCount = unitCount;
            b.waves = waves;
            b.waveDelay = waveDelay;
            b.maxInfluence = maxInfluence;
            b.canPierce = canPierce;
            b.hitArea = hitArea.Copy();
            b.shape = shape.Copy();
            b.offset = offset;
            return b;
        }
    }
    [Serializable]
    public class SkillArea : SkillObj
    {
        //	静态ID, 从1开始 [1 - ~]
        public int id = 1;
        public JSkillUnit.ReferPoint referPoint;
        // 粒子伤害计算基准点，粒子的表现位置有特效处理，此数值只对碰撞计算有影响
        public JSkillUnit.BasePoint basePoint;
        //  buff生效延迟
        public int moveDelay;
        //	技能粒子波次
        public int waves;
        //	技能伤害间隔
        public int waveDelay;
        // 	碰撞体
        public SkillShape hitArea = new SkillShape();
        //	技能最大作用数量
        public int maxInfluence;
        public SkillArea Copy()
        {
            SkillArea b = new SkillArea();
            b.id = id;
            b.moveDelay = moveDelay;
            b.referPoint = referPoint;
            b.basePoint = basePoint;
            b.waves = waves;
            b.waveDelay = waveDelay;
            b.maxInfluence = maxInfluence;
            b.hitArea = hitArea.Copy();
            return b;
        }
    }
    [Serializable]
    public class SkillHelix : SkillObj
    {
        //	静态ID, 从1开始 [1 - ~]
        public int id;
        //	运动阶段的持续时间						
        public int moveTime;
        //	曲线最大半径
        public float maxRadius;
        //	技能最大作用数量
        public int maxInfluence;
        // 碰撞体
        public SkillShape hitArea = new SkillShape();
        //	技能是否可穿透
        public bool canPierce;
        // 粒子偏移量
        public Vector3 offset;
        public SkillHelix Copy()
        {
            SkillHelix b = new SkillHelix();
            b.id = id;
            b.moveTime = moveTime;
            b.maxRadius = maxRadius;
            b.maxInfluence = maxInfluence;
            b.canPierce = canPierce;
            b.hitArea = hitArea.Copy();
            b.offset = offset;
            return b;
        }
    }
    [Serializable]
    public class SkillAreaRand : SkillObj
    {
        //	静态ID, 从1开始 [1 - ~]
        public int id = 1;
        public JSkillUnit.ReferPoint referPoint;
        public JSkillUnit.BasePoint basePoint;
        //  子技能id
        public int unitID;
        //	技能粒子数
        public int unitCount;
        // 产生粒子区域
        public SkillShape area = new SkillShape();

        public SkillAreaRand Copy()
        {
            SkillAreaRand b = new SkillAreaRand();
            b.id = id;
            b.referPoint = referPoint;
            b.basePoint = basePoint;
            b.unitID = unitID;
            b.unitCount = unitCount;
            b.area = area.Copy();
            return b;
        }
    }
    [Serializable]
    public class SkillFollow : SkillObj
    {
        //	静态ID, 从1开始 [1 - ~]
        public int id;
        //	最大跟踪时间						
        public int maxFollowTime;
        //	运动阶段的速率
        public float speed;
        //	粒子波次
        public int waves;
        //  波次间隔时间
        public int waveDelay;
        //  碰撞体
        public SkillShape hitArea = new SkillShape();
        // 粒子偏移量
        public Vector3 offset;
        public SkillFollow Copy()
        {
            SkillFollow b = new SkillFollow();
            b.id = id;
            b.maxFollowTime = maxFollowTime;
            b.speed = speed;
            b.waves = waves;
            b.waveDelay = waveDelay;
            b.hitArea = hitArea.Copy();
            b.offset = offset;
            return b;
        }
    }
    [Serializable]
    public class SkillJump : SkillObj
    {
        public enum JumpType
        {
            JUMP_NONE = 0,
            //	朝着目标跳
            EMENY = 1,
            //	按照自己的方向跳
            FRIEND = 2
        }
        //	静态ID, 从1开始 [1 - ~]
        public int id;
        //	运动阶段的持续时间						
        public int moveTime;
        //	运动阶段的速率
        public float speed;
        // 	跳跃高度
        public float height;
        // 	跳跃目标类型
        public JumpType jumpType;
        //	技能最大作用数量
        public int maxInfluence;
        //  移动过程中的碰撞体
        public SkillShape hitArea = new SkillShape();
        public SkillJump Copy()
        {
            SkillJump b = new SkillJump();
            b.id = id;
            b.moveTime = moveTime;
            b.speed = speed;
            b.height = height;
            b.jumpType = jumpType;
            b.hitArea = hitArea.Copy();
            b.maxInfluence = maxInfluence;
            return b;
        }
    }
    [Serializable]
    public class SkillBackStab : SkillObj
    {
        //	静态ID, 从1开始 [1 - ~]
        public int id = 1;
        public JSkillUnit.ReferPoint referPoint;
        public JSkillUnit.BasePoint basePoint;
        //  buff生效延迟
        public int moveDelay;
        //  碰撞体
        public SkillShape hitArea = new SkillShape();
        //	技能最大作用数量
        public int maxInfluence;
        public SkillBackStab Copy()
        {
            SkillBackStab b = new SkillBackStab();
            b.id = id;
            b.referPoint = referPoint;
            b.basePoint = basePoint;
            b.moveDelay = moveDelay;
            b.maxInfluence = maxInfluence;
            b.hitArea = hitArea.Copy();
            return b;
        }
    }
}