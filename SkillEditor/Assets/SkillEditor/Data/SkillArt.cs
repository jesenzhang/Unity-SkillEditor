using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Collections.Generic;

namespace CySkillEditor
{
    [Serializable]
    public class SkillArt
    { 
        
        [SerializeField]
        //人物模型
        public GameObject model = null;
        [SerializeField]
        //模型名称
        public string modelName = "";
        [SerializeField]
        //模型类型
        public ModelTargetType modelType = ModelTargetType.Player;
        [SerializeField]
        //人物模型动作
        public string animationController = "";

        //	静态ID, 从1开始
        public int id;
        public string idString = "";
        //	技能开始动作	
        public string guideAction = "";
        //	技能释放动作		
        public string guidingAction = "";
        //	技能收手动作	
        public string endAction = "";
        //	技能开始阶段效果
        public List<SkillEffectUnit> beginEffect;
        //	技能粒子特效	
        public List<SkillEffectUnit> unitEffect;
        //	技能结束阶段效果	
        public List<SkillEffectUnit> endEffect;
        //	命中特效	
        public List<SkillEffectUnit> hitEffect;
        //	预警特效	
        public List<SkillEffectUnit> tipEffect;
         //	开始阶段镜头动作类型
        public SkillCameraAction beginCameraAction;
        //	移动阶段镜头动作类型
        public SkillCameraAction moveCameraAction;
        //	命中阶段镜头动作类型
        public SkillCameraAction hitCameraAction; 
        // 	预警参考点
        // 技能开始动作与上一个动作的动画混合时间
        public float guideFadeTime = 0.3f;

        private void OnEnable()
        {
            beginEffect = new List<SkillEffectUnit>();
            unitEffect = new List<SkillEffectUnit>();
            endEffect = new List<SkillEffectUnit>();
            hitEffect = new List<SkillEffectUnit>();
            tipEffect = new List<SkillEffectUnit>();
            beginCameraAction = new SkillCameraAction();
            moveCameraAction = new SkillCameraAction();
            hitCameraAction = new SkillCameraAction();
        }
        public SkillArt Copy()
        {
            SkillArt b = new SkillArt();
            b.id = id;
            b.model = model;
            b.modelName = modelName;
            b.animationController = animationController;
            b.modelType = modelType;
            b.idString = idString;
            b.guideAction = guideAction;
            b.guidingAction = guidingAction; 

            if (beginCameraAction != null)
                b.beginCameraAction = beginCameraAction.Copy();
            else
                b.beginCameraAction = null;

            if (moveCameraAction != null)
                b.moveCameraAction = moveCameraAction.Copy();
            else
                b.moveCameraAction = null;

            if (hitCameraAction != null)
                b.hitCameraAction = hitCameraAction.Copy();
            else
                b.hitCameraAction = null;
            /*
            if (unitEffect != null)
                b.unitEffect = unitEffect.Copy();
            else
                b.unitEffect = null;
            if (endEffect != null)
                b.endEffect = endEffect.Copy();
            else
                b.endEffect = null;
            if (hitEffect != null)
                b.hitEffect = hitEffect.Copy();
            else
                b.hitEffect = null;
            if (tipEffect != null)
                b.tipEffect = tipEffect.Copy();
            else
                b.tipEffect = null;
            */
            if (beginEffect != null)
            {
                b.beginEffect = new List<SkillEffectUnit>();
                for (int i = 0; i < beginEffect.Count; i++)
                {
                    b.beginEffect.Add(beginEffect[i].Copy());
                }
            }
            if (unitEffect != null)
            {
                b.unitEffect = new List<SkillEffectUnit>();
                for (int i = 0; i < unitEffect.Count; i++)
                {
                    b.unitEffect.Add(unitEffect[i].Copy());
                }
            }
            if (endEffect != null)
            {
                b.endEffect = new List<SkillEffectUnit>();
                for (int i = 0; i < endEffect.Count; i++)
                {
                    b.endEffect.Add(endEffect[i].Copy());
                }
            }
            if (hitEffect != null)
            {
                b.hitEffect = new List<SkillEffectUnit>();
                for (int i = 0; i < hitEffect.Count; i++)
                {
                    b.hitEffect.Add(hitEffect[i].Copy());
                }
            }
            if (tipEffect != null)
            {
                b.tipEffect = new List<SkillEffectUnit>();
                for (int i = 0; i < tipEffect.Count; i++)
                {
                    b.tipEffect.Add(tipEffect[i].Copy());
                }
            }
            return b;
        }
    }
}