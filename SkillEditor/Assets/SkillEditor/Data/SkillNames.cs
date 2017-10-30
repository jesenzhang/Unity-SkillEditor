using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Collections.Generic;

namespace CySkillEditor
{
    public class SkillNames
    {
        public static List<string> ActionNames = new List<string> { "GuideAction", "GuidingAction","EndAction" };
        public static List<string> EffectNames = new List<string> {
             "GuideAction", "GuidingAction","EndAction" ,"BeginEffect0", "BeginEffect1", "EndEffect",
            "TipEffect", "UnitEffect", "HitEffect",
            "BeginCameraAction", "MoveCameraAction", "HitCameraAction","Sound1", "Sound2", "Sound3"};
        public static List<string> SoundNames = new List<string> { "Sound1", "Sound2", "Sound3" };

        public enum GuideAction
        {
            GuideAction = 0,
            GuidingAction = 1,
            EndAction = 2
        }

        public enum Effect
        {
            GuideAction = 0,
            GuidingAction = 1,
            EndAction = 2,

            BeginEffect0 = 3,
            BeginEffect1 = 4,
            EndEffect = 5,
            TipEffect = 6,
            UnitEffect = 7,
            HitEffect =8,
            BeginCameraAction =9,
            MoveCameraAction = 10,
            HitCameraAction = 11,

            Sound1 = 12,
            Sound2 = 13,
            Sound3 = 14
        }
        public enum Sound
        {
            Sound1 = 0,
            Sound2 = 1,
            Sound3 = 2
        }
    }
}