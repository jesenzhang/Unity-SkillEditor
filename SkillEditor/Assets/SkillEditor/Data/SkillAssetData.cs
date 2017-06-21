using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CySkillEditor
{

    [Serializable]
    public class SkillAssetData : ScriptableObject 
    {
        [SerializeField]
        //技能表现
        public  SkillArt skillArt;
        [SerializeField]
        //技能表现
        public JSkillUnit skillUnit;

        private void OnEnable()
        {
            if(skillArt==null)
                skillArt = new SkillArt();
            if (skillUnit == null)
                skillUnit = new JSkillUnit();
        }
        public SkillAssetData Copy()
        {
            SkillAssetData copyskill = new SkillAssetData();
            copyskill.skillArt = skillArt.Copy();
            copyskill.skillUnit = skillUnit.Copy();
            return copyskill;
        }
   
    }

}