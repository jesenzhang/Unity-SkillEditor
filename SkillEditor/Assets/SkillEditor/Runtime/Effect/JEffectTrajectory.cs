using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CySkillEditor
{
    public class JEffectTrajectory : JEffectBase
    {

        [SerializeField]
        public Transform Target;
        [SerializeField]
        public GameObject TargetObject;
        [SerializeField]
        public JSkillUnit skillunit;
        [SerializeField]
        public SkillEffectUnit effectunit;

        float PlaybackDuration = 0;
        private bool active = false;
        [HideInInspector]
        private JAbstractTrajectory[] trajectoryList;
        public JAbstractTrajectory[] TrajectoryList
        {
            get
            {
                if (trajectoryList == null)
                {
                    trajectoryList = new JAbstractTrajectory[0];
                }
                return trajectoryList;
            }
            set
            {
                trajectoryList = value;
            }
        }



        public override void SetData(object[] data)
        {
            Target = (Transform)data[0];
            TargetObject = (GameObject)data[1];
            skillunit = (JSkillUnit)data[2];
            effectunit = (SkillEffectUnit)data[3];
        }

        public override void Init()
        {
            if (active)
                return;
            if (skillunit.launchType == JSkillUnit.LaunchType.SINGLELINE)
                    TrajectoryList = SingleLineFacyory();
            if (skillunit.launchType == JSkillUnit.LaunchType.MULLINE)
                    TrajectoryList = MultiLineFacyory();
            effectunit.artEffect.phaseTime = (int)(PlaybackDuration * 1000f);
            active = true;
        }
        public override void OnUpdate(float time)
        {
            if (active)
            {
                if (TrajectoryList != null)
                {
                    for (int i = 0; i <TrajectoryList.Length; i++)
                    {
                        if (!TrajectoryList[i]._active)
                        {
                            TrajectoryList[i].Begin();
                        }
                        TrajectoryList[i].DoUpdate(Time.realtimeSinceStartup);
                    }

                }
            }
        }
        public override void Reset()
        {
            for (int i = 0; i < TrajectoryList.Length; i++)
            {
                if (TrajectoryList[i]._active)
                {
                    TrajectoryList[i].Reset();
                }
            }
            active = false;
        }

        private JSingleLineTrajectory[] SingleLineFacyory()
        {
            if (skillunit.launchType != JSkillUnit.LaunchType.SINGLELINE)
                return null;
            List<JSingleLineTrajectory> list = new List<JSingleLineTrajectory>();
            SkillLine line = (SkillLine)skillunit.skillObj;

            if (line.waves == 0)
                line.waves = 1;
            for (int i = 0; i < line.waves; i++)
            {
                JSingleLineTrajectory sg = new JSingleLineTrajectory();
                sg.TargetObject = TargetObject;
                sg.Target = Target;
                sg.skillunit = skillunit;
                sg.effectunit = effectunit;
                sg._originDir = TargetObject.transform.forward;
                sg._delayBegin = i * line.waveDelay;
                list.Add(sg);
            }
            if (skillunit.guidePolicy != null)
                PlaybackDuration = (line.waveDelay * line.waves + line.moveTime + skillunit.guidePolicy.guideTime + skillunit.guidePolicy.guidingTime) / 1000f;
            else
                PlaybackDuration = (line.waveDelay * line.waves + line.moveTime) / 1000f;

            return list.ToArray();
        }
        public Vector3 RotateRound(Vector3 dir, Vector3 axis, float angle)
        {
            Vector3 point = Quaternion.AngleAxis(angle, axis) * dir;
            return point.normalized;
        }
        private JSingleLineTrajectory[] MultiLineFacyory()
        {
            if (skillunit.launchType != JSkillUnit.LaunchType.MULLINE)
                return null;
            List<JSingleLineTrajectory> list = new List<JSingleLineTrajectory>();
            SkillMultiLine line = (SkillMultiLine)skillunit.skillObj;
            if (line.shape.area == SkillShape.Area.CIRCLE)
            {
                float angle = 360f / line.unitCount;
                Vector3 dir = TargetObject.transform.forward;
                for (int i = 0; i < line.unitCount; i++)
                {
                    Vector3 ndir = RotateRound(dir, new Vector3(0, 1, 0), i * angle);
                    if (line.waves == 0)
                        line.waves = 1;
                    for (int j = 0; j < line.waves; j++)
                    {
                        JSingleLineTrajectory sg = new JSingleLineTrajectory();
                        sg.TargetObject = TargetObject;
                        sg.Target = null;
                        sg._originDir = ndir;
                        sg.skillunit = skillunit;
                        sg.effectunit = effectunit;
                        sg._delayBegin = j * line.waveDelay;
                        list.Add(sg);
                    }
                }
            }
            if (line.shape.area == SkillShape.Area.QUADRATE)
            {
                Vector3 dir = TargetObject.transform.forward;
                Vector3 ndir = RotateRound(dir, new Vector3(0, 1, 0), 90);
                if (line.unitCount > 1)
                {
                    ndir = ndir * line.shape.param2 / (line.unitCount - 1);
                }
                Vector3 beginPos = -ndir * (line.unitCount - 1) / 2f;
                for (int i = 0; i < line.unitCount; i++)
                {
                    if (line.waves == 0)
                        line.waves = 1;
                    for (int j = 0; j < line.waves; j++)
                    {
                        JSingleLineTrajectory sg = new JSingleLineTrajectory();
                        sg.TargetObject = TargetObject;
                        sg.Target = null;
                        sg._originDir = dir;
                        sg._originPosOffset = beginPos + ndir * i;
                        sg.skillunit = skillunit;
                        sg.effectunit = effectunit;
                        sg._delayBegin = j * line.waveDelay;
                        list.Add(sg);
                    }
                }
            }
            if (line.shape.area == SkillShape.Area.SECTOR)
            {
                float angle = line.shape.param3 / (line.unitCount - 1);
                float beginAngle = -angle * (line.unitCount - 1) / 2;
                Vector3 dir = TargetObject.transform.forward;
                for (int i = 0; i < line.unitCount; i++)
                {
                    Vector3 ndir = RotateRound(dir, new Vector3(0, 1, 0), beginAngle + i * angle);
                    if (line.waves == 0)
                        line.waves = 1;
                    for (int j = 0; j < line.waves; j++)
                    {
                        JSingleLineTrajectory sg = new JSingleLineTrajectory();
                        sg.TargetObject = TargetObject;
                        sg.Target = null;
                        sg._originDir = ndir;
                        sg._originPosOffset = ndir * line.shape.param1;
                        sg.skillunit = skillunit;
                        sg.effectunit = effectunit;
                        sg._delayBegin = j * line.waveDelay;
                        list.Add(sg);
                    }
                }
            }
            if (line.shape.area == SkillShape.Area.TRIANGLE)
            {
                Vector3 dir = TargetObject.transform.forward;
                Vector3 ndir = RotateRound(dir, new Vector3(0, 1, 0), 90);
                if (line.unitCount > 1)
                {
                    ndir = ndir * line.shape.param2 / (line.unitCount - 1) / 2;
                }
                Vector3 beginPos = Vector3.zero;
                if (line.shape.param3 == 1)
                    beginPos = -ndir * (line.unitCount - 1) / 2f;
                if (line.shape.param3 == 2)
                    beginPos = dir * line.shape.param1 - ndir * (line.unitCount - 1) / 2f;
                Vector3 launchPos = Vector3.zero;
                Vector3 pdir = Vector3.zero;
                for (int i = 0; i < line.unitCount; i++)
                {
                    if (line.shape.param3 == 1)
                    {
                        launchPos = beginPos + i * ndir;
                        Vector3 tempPos = dir * line.shape.param1;
                        pdir = (tempPos - launchPos).normalized;
                    }
                    else
                    if (line.shape.param3 == 2)
                    {
                        Vector3 tempPos = beginPos + i * ndir;
                        pdir = (tempPos - launchPos).normalized;
                    }

                    if (line.waves == 0)
                        line.waves = 1;
                    for (int j = 0; j < line.waves; j++)
                    {
                        JSingleLineTrajectory sg = new JSingleLineTrajectory();
                        sg.TargetObject = TargetObject;
                        sg.Target = null;
                        sg._originPosOffset = launchPos;
                        sg._originDir = pdir;
                        sg.skillunit = skillunit;
                        sg.effectunit = effectunit;
                        sg._delayBegin = j * line.waveDelay;
                        list.Add(sg);
                    }
                }
            }
            if (skillunit.guidePolicy != null)
                PlaybackDuration = (line.waveDelay * line.waves + line.moveTime + skillunit.guidePolicy.guideTime + skillunit.guidePolicy.guidingTime) / 1000f;
            else
                PlaybackDuration = (line.waveDelay * line.waves + line.moveTime) / 1000f;

            return list.ToArray();
        }
    }
}