using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CySkillEditor
{
    public class JEffectBase
    {
        public virtual void SetData(object[] data)
        {
        }

        public virtual void Init()
        {
        }
        public virtual void OnUpdate(float time)
        {

        }
        public virtual void Reset()
        {
        }
    }
}