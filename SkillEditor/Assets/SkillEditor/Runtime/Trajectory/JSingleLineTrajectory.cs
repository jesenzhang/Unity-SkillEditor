using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
namespace CySkillEditor
{
    public class JSingleLineTrajectory : JAbstractTrajectory
    {

        public override JSkillUnit.LaunchType GetLaunchType()
        {
            return JSkillUnit.LaunchType.SINGLELINE;
        }
        public override bool CheckMoveOver(float tempTime)
        {
            if ( skillunit.launchType == JSkillUnit.LaunchType.SINGLELINE)
            {
                return tempTime > ((SkillLine) skillunit.skillObj).moveTime / 1000f;
            }
            if ( skillunit.launchType == JSkillUnit.LaunchType.MULLINE)
            {
                return tempTime > ((SkillMultiLine) skillunit.skillObj).moveTime / 1000f;
            }
            return false;
        }
        public override void Begin()
        {
            base.Begin();
            if (effecrObj.Count >= 1)
            {
                foreach (var e in effecrObj)
                    GameObject.DestroyImmediate(e);
            }
            effecrObj.Clear();
            effecrObj.Add(GameObject.Instantiate( effectunit.artEffect.effectObj));
            particleSys.Clear();
            ParticleSystem[] particleSys0 = effecrObj[0].GetComponentsInChildren<ParticleSystem>();
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
            effecrObj[0].SetActive(false);
            SetState(TRAJSTATE.STATE_BEGIN);
            float offsetY = 0;
            effecrObj[0].transform.SetParent(TargetObject.transform);
            if ( effectunit.configure.posType == CySkillEditor.EffectConfigure.PosType.FEET)
            {
                offsetY = 0;
            }
            else if ( effectunit.configure.posType == CySkillEditor.EffectConfigure.PosType.BODY)
            {
                offsetY =  effectunit.configure.bodyHeight;
            }
            else if ( effectunit.configure.posType == CySkillEditor.EffectConfigure.PosType.HEAD)
            {
                offsetY =  effectunit.configure.headHeight;
            }
            else if ( effectunit.configure.posType == CySkillEditor.EffectConfigure.PosType.BONE)
            {
                Transform bone =  TargetObject.transform.Find( effectunit.configure.boneName);
                if (bone)
                    effecrObj[0].transform.SetParent(bone);
            }

            effecrObj[0].transform.localPosition = new Vector3(0, offsetY, 0);
            Vector3 offset =  effectunit.configure.position;
            effecrObj[0].transform.localPosition += offset;
            _originPos = effecrObj[0].transform.position;
            _originPos += _originPosOffset;
            if ( effectunit.configure.posType == CySkillEditor.EffectConfigure.PosType.WORLD)
            {
                effecrObj[0].transform.position = offset;
            }
            if ( Target != null)
            {
                _originDir = ( Target.position -  _originPos).normalized;
            }
            else
            {
                //_originDir =  TargetObject.transform.forward;
            }
            _originDir += _originDirOffset;
        }
        public override void OnMoveStart()
        {
            effecrObj[0].transform.position = _originPos;
            effecrObj[0].SetActive(true);
            foreach (var p in particleSys)
            {
                p.Stop();
                p.Simulate(0, true);
                p.Clear();
            }
        }
        public override void OnMoveUpdate(float time)
        {
            if (effecrObj != null)
            {
                foreach (var p in particleSys)
                {
                    p.Simulate(time, true);
                }
                Vector3 dir = _originDir;

                if ( skillunit.launchType == JSkillUnit.LaunchType.SINGLELINE)
                {
                    SkillLine line = (SkillLine) skillunit.skillObj;
                    float fen = time / (line.moveTime / 1000f);
                    Vector3 final = _originPos + dir * line.speed * line.moveTime / 1000f;
                    effecrObj[0].transform.position = Vector3.Lerp(_originPos, final, fen);
                }
                if ( skillunit.launchType == JSkillUnit.LaunchType.MULLINE)
                {
                    SkillMultiLine line = (SkillMultiLine) skillunit.skillObj;
                    float fen = time / (line.moveTime / 1000f);
                    Vector3 final = _originPos + dir * line.speed * line.moveTime / 1000f;
                    effecrObj[0].transform.position = Vector3.Lerp(_originPos, final, fen);
                    effecrObj[0].transform.forward = _originDir;
                }

                
            }
        }

        public override void OnMoveOver()
        {
            if (effecrObj.Count >= 1)
            {
                foreach (var e in effecrObj)
                    GameObject.DestroyImmediate(e);
            }
            effecrObj.Clear();
            particleSys.Clear();
        }
    }
}