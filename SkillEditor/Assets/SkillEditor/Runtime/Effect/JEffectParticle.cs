using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CySkillEditor
{
    public class JEffectParticle : JEffectBase
    {
        
        public SkillEffectUnit EffectUnit;
        public GameObject TargetObject;
        
        private GameObject EffectObj;
        private List<ParticleSystem> particleSys = new List<ParticleSystem>();
        private bool active = false;
      

        public override void SetData(object[] data)
        {
            EffectUnit = (SkillEffectUnit)data[0];
            TargetObject = (GameObject)data[1];
        }

        public override void Init()
        {
            if (active)
                return;
            if (EffectUnit == null)
                return;
            if (EffectUnit.artEffect.effectObj == null)
                return;
            if (EffectObj)
            {
                GameObject.DestroyImmediate(EffectObj);
            }
            particleSys.Clear();
            EffectObj = GameObject.Instantiate(EffectUnit.artEffect.effectObj);
            ParticleSystem[] particleSys0 = EffectObj.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particleSys0.Length; i++)
            {
                particleSys0[i].Stop();
                particleSys0[i].Simulate(0);
                particleSys0[i].Clear();
                if (ParticleSystemUtility.IsRoot(particleSys0[i]))
                {
                    particleSys.Add(particleSys0[i]);
                }
            }
            float offsetY = 0;
            Vector3 offset = EffectUnit.configure.position;
            //世界坐标
            if (EffectUnit.configure.posType == CySkillEditor.EffectConfigure.PosType.WORLD)
            {
                EffectObj.transform.position = offset;
            }
            else //世界坐标 相对于主角
            if (EffectUnit.configure.posType == CySkillEditor.EffectConfigure.PosType.RELATIVE)
            {
                EffectObj.transform.position = TargetObject.transform.position + offset;
            }
            else
            {
                //附着于主角身上
                EffectObj.transform.SetParent(TargetObject.transform);
                if (EffectUnit.configure.posType == CySkillEditor.EffectConfigure.PosType.FEET)
                {
                    offsetY = 0;
                }
                else if (EffectUnit.configure.posType == CySkillEditor.EffectConfigure.PosType.BODY)
                {
                    offsetY = EffectUnit.configure.bodyHeight;
                }
                else if (EffectUnit.configure.posType == CySkillEditor.EffectConfigure.PosType.HEAD)
                {
                    offsetY = EffectUnit.configure.headHeight;
                }
                else if (EffectUnit.configure.posType == CySkillEditor.EffectConfigure.PosType.BONE)
                {
                    Transform bone = TargetObject.transform.Find(EffectUnit.configure.boneName);
                    if (bone)
                        EffectObj.transform.SetParent(bone);
                }
                EffectObj.transform.localPosition = new Vector3(0, offsetY, 0);
                EffectObj.transform.localPosition += offset;
            }
            EffectObj.transform.rotation = Quaternion.Euler(EffectUnit.configure.rotation);
            
            active = true;
        }
        public override void OnUpdate(float time)
        {
            if (EffectObj != null && active)
            {
                foreach (var p in particleSys)
                {
                    p.Simulate(time, true, false);
                }
            }
        }
        public override void Reset()
        {
            active = false;
            if (EffectObj)
            {
                GameObject.DestroyImmediate(EffectObj);
            }
            particleSys.Clear();
        }

    }
}