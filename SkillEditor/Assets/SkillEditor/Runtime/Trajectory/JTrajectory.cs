using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
namespace CySkillEditor
{
    public enum TRAJSTATE
    {
        STATE_OVER = 0,
        STATE_BEGIN=1,
        STATE_MOVE=2,
        STATE_IDLE=3
    }
    [System.Serializable]
    public abstract class JAbstractTrajectory
    {
        public Transform Target;
        public GameObject TargetObject;
        public List<GameObject> effecrObj = new List<GameObject>();
        public JSkillUnit skillunit;
        public SkillEffectUnit effectunit;
        public int _delayBegin;
        public TRAJSTATE state = TRAJSTATE.STATE_OVER;
        public float _time = 0;
        public float _startTime = 0;
        public bool _moved = false;
        public bool _active = false;
        public bool _hited = false;
        protected Vector3 _originPos = Vector3.zero;
        public Vector3 _originPosOffset = Vector3.zero;
        public Vector3 _originDir = Vector3.zero;
        public Vector3 _originDirOffset = Vector3.zero;
        public List<ParticleSystem> particleSys = new List<ParticleSystem>();
        public virtual JSkillUnit.LaunchType GetLaunchType()
        {
            return  JSkillUnit.LaunchType.LAUNCH_NONE;
        }

        public virtual void ShowEffect(TRAJSTATE state , bool show)
        { 
        }
        public virtual void Reset()
        {
            state = TRAJSTATE.STATE_OVER;
            _time = 0;
            _startTime = 0;
            _active = false;
            _moved = false;
            _hited = false;
            SetState(TRAJSTATE.STATE_BEGIN);
            if (effecrObj.Count >= 1)
            {
                foreach(var e in effecrObj)
                GameObject.DestroyImmediate(e);
            }
            effecrObj.Clear();
        }
        public virtual void Begin()
        {
            _startTime = Time.realtimeSinceStartup;
            _time = Time.realtimeSinceStartup;
          
            _active = true;
            _moved = false;
            _hited = false;
            SetState(TRAJSTATE.STATE_BEGIN);
        }

        
        public virtual void OnBeginStart()
        { 
        }
        public virtual void OnBeginOver()
        {
        }
        public virtual void OnBeginUpdate(float temptime)
        {
        }


        public virtual void SetState(TRAJSTATE astate)
        {
            StateChange(astate);
        }
        public virtual void OnMoveStart()
        {

        }
        public virtual void OnMoveUpdate(float time)
        {

        }
        public virtual void OnMoveOver()
        {

        }

        public virtual float GetBeginTime()
        {
            if (skillunit.guidePolicy.type == SkillGuidePolicy.GuideType.GUIDE)
            {
                return skillunit.guidePolicy.guideTime / 1000f;
            }
            else
            {
                return (skillunit.guidePolicy.guideTime+skillunit.guidePolicy.guidingTime) / 1000f; ;
            }
          
        }
        public virtual float GetMoveTime()
        {
            return 0;
        }
        public virtual float GetHitTime()
        {
            return 0;
        }

        public virtual void OnHitStart()
        {

        }
        public virtual void OnHitUpdate(float time )
        {

        }
        public virtual void OnHitOver()
        {

        }
        public virtual void StateChange(TRAJSTATE astate)
        {
            if (state != astate)
            {
                _time = Time.realtimeSinceStartup;
                if (astate == TRAJSTATE.STATE_BEGIN)
                {
                    OnBeginStart();
                }
                else if (astate == TRAJSTATE.STATE_MOVE)
                {
                    ShowEffect(TRAJSTATE.STATE_MOVE, true);
                    OnMoveStart();
                }
                else if (astate == TRAJSTATE.STATE_IDLE)
                {
                    ShowEffect(TRAJSTATE.STATE_MOVE, false);
                    OnHitStart();
                }
                else if (astate == TRAJSTATE.STATE_OVER)
                {
                    ShowEffect(TRAJSTATE.STATE_OVER, true); 
                }
                state = astate;
            }

        }
        public virtual bool CheckBeginOver(float tempTime)
        {
            return tempTime >= GetBeginTime();
        } 
        public virtual bool CheckMoveOver(float tempTime)
        {
            return tempTime >= GetMoveTime();
        }
   
        public virtual void DoUpdate(float curTime)
        {
            if (state == TRAJSTATE.STATE_IDLE)
            {
                float tempTime = curTime - _time;
                if (tempTime >= GetHitTime() && _hited)
                {
                    if (!_hited)
                    {
                        OnHitUpdate(tempTime);
                        _hited = true;
                    }
                    OnHitOver();
                    SetState(TRAJSTATE.STATE_OVER);
                }
                else
                {
                    OnHitUpdate(tempTime);
                    _hited = true;
                }
            }
            else if (state == TRAJSTATE.STATE_MOVE)
            {
                float tempTime = curTime - _time;
                //结束移动
                if (CheckMoveOver(tempTime))
                {
                    if (!_moved)
                    {
                        OnMoveUpdate(tempTime);
                        _moved = true;
                    }
                    OnMoveOver();
                    SetState(TRAJSTATE.STATE_OVER);
                }
                else
                {
                    OnMoveUpdate(tempTime);
                    _moved = true;
                }
            }
            else if (state == TRAJSTATE.STATE_BEGIN)
            {
                if (CheckBeginOver(curTime - _startTime - _delayBegin / 1000f))
                {
                    OnBeginOver();
                    SetState(TRAJSTATE.STATE_MOVE);
                }
                else
                {
                    OnBeginUpdate(curTime - _startTime - _delayBegin / 1000f);
                }
            }
        }

      
    }
    
}