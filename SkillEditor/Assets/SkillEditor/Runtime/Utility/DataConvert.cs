using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace CySkillEditor
{
    public class DataConvert
    {
        public static SkillShape ConvertSkillShape(SkillUnit.SkillShape unit)
        {
            SkillShape oUnit = new SkillShape();
            oUnit.area = (SkillShape.Area)(int)unit.area;
            oUnit.param1 = unit.param1;
            oUnit.param2 = unit.param2;
            oUnit.param3 = unit.param3;
            return oUnit;
        }
        public static SkillUnit.SkillShape ConvertSkillShape(SkillShape unit)
        {
            SkillUnit.SkillShape oUnit = new SkillUnit.SkillShape();
            oUnit.area = (SkillUnit.SkillShape.Area)(int)unit.area;
            oUnit.param1 = unit.param1;
            oUnit.param2 = unit.param2;
            oUnit.param3 = unit.param3;
            return oUnit;
        }
        public static void ConvertSkillShape(SkillShape oUnit, SkillUnit.SkillShape unit)
        {
            oUnit.area = (SkillShape.Area)(int)unit.area;
            oUnit.param1 = unit.param1;
            oUnit.param2 = unit.param2;
            oUnit.param3 = unit.param3;
        }
        public static void ConvertSkillShape(SkillUnit.SkillShape  oUnit, SkillShape unit)
        {
            oUnit.area = (SkillUnit.SkillShape.Area)(int)unit.area;
            oUnit.param1 = unit.param1;
            oUnit.param2 = unit.param2;
            oUnit.param3 = unit.param3;
        }
        public static SkillGuidePolicy ConvertSkillGuidePolicy(SkillUnit.SkillGuidePolicy unit)
        {
            SkillGuidePolicy oUnit = new SkillGuidePolicy();
            oUnit.endTime = unit.endTime;
            oUnit.guideTime = unit.guideTime;
            oUnit.guidingTime = unit.guidingTime;
            oUnit.type = (SkillGuidePolicy.GuideType)(int)unit.type;
            return oUnit;
        }
        public static SkillUnit.SkillGuidePolicy ConvertSkillGuidePolicy(SkillGuidePolicy unit)
        {
            SkillUnit.SkillGuidePolicy oUnit = new SkillUnit.SkillGuidePolicy();
            oUnit.endTime = unit.endTime;
            oUnit.guideTime = unit.guideTime;
            oUnit.guidingTime = unit.guidingTime;
            oUnit.type = (SkillUnit.SkillGuidePolicy.GuideType)(int)unit.type;
            return oUnit;
        }
        public static void ConvertSkillGuidePolicy(SkillGuidePolicy oUnit, SkillUnit.SkillGuidePolicy unit)
        {
            oUnit.endTime = unit.endTime;
            oUnit.guideTime = unit.guideTime;
            oUnit.guidingTime = unit.guidingTime;
            oUnit.type = (SkillGuidePolicy.GuideType)(int)unit.type;
        }
        public static void ConvertSkillGuidePolicy(SkillUnit.SkillGuidePolicy oUnit, SkillGuidePolicy unit)
        {
            oUnit.endTime = unit.endTime;
            oUnit.guideTime = unit.guideTime;
            oUnit.guidingTime = unit.guidingTime;
            oUnit.type = (SkillUnit.SkillGuidePolicy.GuideType)(int)unit.type;
        }
        public static SkillLine ConvertSkillLine(SkillUnit.SkillLine unit)
        {
            SkillLine oUnit = new SkillLine();
            oUnit.id = unit.id;
            oUnit.moveTime = unit.moveTime;
            oUnit.canPierce = unit.canPierce;
            oUnit.offset = Vector3.zero;
            if (unit.offset != null)
                oUnit.offset = new Vector3(unit.offset.x, unit.offset.y, unit.offset.z);
            oUnit.speed = unit.speed;
            oUnit.waves = unit.waves;
            oUnit.waveDelay = unit.waveDelay;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillUnit.SkillLine ConvertSkillLine(SkillLine unit)
        {
            SkillUnit.SkillLine oUnit = new SkillUnit.SkillLine();
            oUnit.id = unit.id;
            oUnit.moveTime = unit.moveTime;
            oUnit.canPierce = unit.canPierce;
            oUnit.offset = new Math.Vector3f();
           
            oUnit.offset.x = unit.offset.x;
            oUnit.offset.y = unit.offset.y;
            oUnit.offset.z = unit.offset.z;
            
            oUnit.speed = unit.speed;
            oUnit.waves = unit.waves;
            oUnit.waveDelay = unit.waveDelay;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillMultiLine ConvertSkillMultiLine(SkillUnit.SkillMultiLine unit)
        {
            SkillMultiLine oUnit = new SkillMultiLine();
            oUnit.id = unit.id;
            oUnit.moveTime = unit.moveTime;
            oUnit.canPierce = unit.canPierce;
            oUnit.offset = Vector3.zero;
            if (unit.offset != null)
                oUnit.offset = new Vector3(unit.offset.x, unit.offset.y, unit.offset.z);
            oUnit.speed = unit.speed;
            oUnit.waves = unit.waves;
            oUnit.unitCount = unit.unitCount;
            oUnit.waveDelay = unit.waveDelay;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            oUnit.shape = ConvertSkillShape(unit.shape);
            return oUnit;
        }
        public static SkillUnit.SkillMultiLine ConvertSkillMultiLine(SkillMultiLine unit)
        {
            SkillUnit.SkillMultiLine oUnit = new SkillUnit.SkillMultiLine();
            oUnit.id = unit.id;
            oUnit.moveTime = unit.moveTime;
            oUnit.canPierce = unit.canPierce;
            oUnit.offset =new Math.Vector3f();
            
            oUnit.offset.x = unit.offset.x;
            oUnit.offset.y = unit.offset.y;
            oUnit.offset.z = unit.offset.z;
            
            oUnit.speed = unit.speed;
            oUnit.waves = unit.waves;
            oUnit.unitCount = unit.unitCount;
            oUnit.waveDelay = unit.waveDelay;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            oUnit.shape = ConvertSkillShape(unit.shape);
            return oUnit;
        }
        public static SkillArea ConvertSkillArea(SkillUnit.SkillArea unit)
        {
            SkillArea oUnit = new SkillArea();
            oUnit.id = unit.id;
            oUnit.referPoint = (JSkillUnit.ReferPoint)(int)unit.referPoint;
            oUnit.basePoint = (JSkillUnit.BasePoint)(int)unit.basePoint;
            oUnit.waves = unit.waves;
            oUnit.waveDelay = unit.waveDelay;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillUnit.SkillArea ConvertSkillArea(SkillArea unit)
        {
            SkillUnit.SkillArea oUnit = new SkillUnit.SkillArea();
            oUnit.id = unit.id;
            oUnit.referPoint = (SkillUnit.SkillUnit.ReferPoint)(int)unit.referPoint;
            oUnit.basePoint = (SkillUnit.SkillUnit.BasePoint)(int)unit.basePoint;
            oUnit.waves = unit.waves;
            oUnit.waveDelay = unit.waveDelay;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillHelix ConvertSkillHelix(SkillUnit.SkillHelix unit)
        {
            SkillHelix oUnit = new SkillHelix();
            oUnit.id = unit.id;
            oUnit.moveTime = unit.moveTime;
            oUnit.canPierce = unit.canPierce;
            oUnit.offset = Vector3.zero;
            if (unit.offset != null)
                oUnit.offset = new Vector3(unit.offset.x, unit.offset.y, unit.offset.z);
            oUnit.maxRadius = unit.maxRadius;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillUnit.SkillHelix ConvertSkillHelix(SkillHelix unit)
        {
            SkillUnit.SkillHelix oUnit = new SkillUnit.SkillHelix();
            oUnit.id = unit.id;
            oUnit.moveTime = unit.moveTime;
            oUnit.canPierce = unit.canPierce;
            oUnit.offset = new Math.Vector3f();
           
            oUnit.offset.x = unit.offset.x;
            oUnit.offset.y = unit.offset.y;
            oUnit.offset.z = unit.offset.z;
            
            oUnit.maxRadius = unit.maxRadius;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillAreaRand ConvertSkillAreaRand(SkillUnit.SkillAreaRand unit)
        {
            SkillAreaRand oUnit = new SkillAreaRand();
            oUnit.id = unit.id;
            oUnit.referPoint = (JSkillUnit.ReferPoint)(int)unit.referPoint;
            oUnit.basePoint = (JSkillUnit.BasePoint)(int)unit.basePoint;
            oUnit.unitID = unit.unitID;
            oUnit.unitCount = unit.unitCount;
            oUnit.area = ConvertSkillShape(unit.area);
            return oUnit;
        }
        public static SkillUnit.SkillAreaRand ConvertSkillAreaRand(SkillAreaRand unit)
        {
            SkillUnit.SkillAreaRand oUnit = new SkillUnit.SkillAreaRand();
            oUnit.id = unit.id;
            oUnit.referPoint = (SkillUnit.SkillUnit.ReferPoint)(int)unit.referPoint;
            oUnit.basePoint = (SkillUnit.SkillUnit.BasePoint)(int)unit.basePoint;
            oUnit.unitID = unit.unitID;
            oUnit.unitCount = unit.unitCount;
            oUnit.area = ConvertSkillShape(unit.area);
            return oUnit;
        }
        public static SkillFollow ConvertSkillFollow(SkillUnit.SkillFollow unit)
        {
            SkillFollow oUnit = new SkillFollow();
            oUnit.id = unit.id;
            oUnit.maxFollowTime = unit.maxFollowTime;
            oUnit.offset = Vector3.zero;
            if (unit.offset != null)
                oUnit.offset = new Vector3(unit.offset.x, unit.offset.y, unit.offset.z);
            oUnit.speed = unit.speed;
            oUnit.waves = unit.waves;
            oUnit.waveDelay = unit.waveDelay;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillUnit.SkillFollow ConvertSkillFollow(SkillFollow unit)
        {
            SkillUnit.SkillFollow oUnit = new SkillUnit.SkillFollow();
            oUnit.id = unit.id;
            oUnit.maxFollowTime = unit.maxFollowTime;
            oUnit.offset = new Math.Vector3f();
           
            oUnit.offset.x = unit.offset.x;
            oUnit.offset.y = unit.offset.y;
            oUnit.offset.z = unit.offset.z;
           
            oUnit.speed = unit.speed;
            oUnit.waves = unit.waves;
            oUnit.waveDelay = unit.waveDelay;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillJump ConvertSkillJump(SkillUnit.SkillJump unit)
        {
            SkillJump oUnit = new SkillJump();
            oUnit.id = unit.id;
            oUnit.height = unit.height;
            oUnit.speed = unit.speed;
            oUnit.moveTime = unit.moveTime;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.jumpType = (SkillJump.JumpType)(int)unit.jumpType;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillUnit.SkillJump ConvertSkillJump(SkillJump unit)
        {
            SkillUnit.SkillJump oUnit = new SkillUnit.SkillJump();
            oUnit.id = unit.id;
            oUnit.height = unit.height;
            oUnit.speed = unit.speed;
            oUnit.moveTime = unit.moveTime;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.jumpType = (SkillUnit.SkillJump.JumpType)(int)unit.jumpType;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillBackStab ConvertSkillBackStab(SkillUnit.SkillBackStab unit)
        {
            SkillBackStab oUnit = new SkillBackStab();
            oUnit.id = unit.id;
            oUnit.referPoint = (JSkillUnit.ReferPoint)(int)unit.referPoint;
            oUnit.basePoint = (JSkillUnit.BasePoint)(int)unit.basePoint;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.moveDelay = unit.moveDelay;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillUnit.SkillBackStab ConvertSkillBackStab(SkillBackStab unit)
        {
            SkillUnit.SkillBackStab oUnit = new SkillUnit.SkillBackStab();
            oUnit.id = unit.id;
            oUnit.referPoint = (SkillUnit.SkillUnit.ReferPoint)(int)unit.referPoint;
            oUnit.basePoint = (SkillUnit.SkillUnit.BasePoint)(int)unit.basePoint;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.moveDelay = unit.moveDelay;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillArtEffect ConvertSkillArtEffect(SkillUnit.SkillArtEffect unit)
        {
            SkillArtEffect oUnit = new SkillArtEffect();
            oUnit.effect = unit.effect;
            oUnit.effPos = (SkillArtEffect.EffPos)(int)unit.effPos;
            oUnit.height = unit.height;
            oUnit.phaseTime = unit.phaseTime;
            return oUnit;
        }
        public static SkillUnit.SkillArtEffect ConvertSkillArtEffect(SkillArtEffect unit)
        {
            SkillUnit.SkillArtEffect oUnit = new SkillUnit.SkillArtEffect();
            oUnit.effect = unit.effect;
            oUnit.effPos = (SkillUnit.SkillArtEffect.EffPos)(int)unit.effPos;
            oUnit.height = unit.height;
            oUnit.phaseTime = unit.phaseTime;
            return oUnit;
        }
        public static void ConvertSkillArtEffect(SkillArtEffect oUnit, SkillUnit.SkillArtEffect unit)
        {
            oUnit.effect = unit.effect;
            oUnit.effPos = (SkillArtEffect.EffPos)(int)unit.effPos;
            oUnit.height = unit.height;
            oUnit.phaseTime = unit.phaseTime;
        }
        public static void ConvertSkillArtEffect(SkillUnit.SkillArtEffect oUnit, SkillArtEffect unit)
        {
            oUnit.effect = unit.effect;
            oUnit.effPos = (SkillUnit.SkillArtEffect.EffPos)(int)unit.effPos;
            oUnit.height = unit.height;
            oUnit.phaseTime = unit.phaseTime;
        }
        public static SkillCameraAction ConvertCameraAction(SkillUnit.SkillCameraAction unit)
        {
            SkillCameraAction oUnit = new SkillCameraAction();
            oUnit.action = (SkillCameraAction.CameraAction)((int)unit.action);
            oUnit.delay = unit.delay;
            oUnit.param = unit.param;
            return oUnit;
        }
        public static void ConvertCameraAction(SkillCameraAction oUnit, SkillUnit.SkillCameraAction unit)
        {
            oUnit.action = (SkillCameraAction.CameraAction)((int)unit.action);
            oUnit.delay = unit.delay;
            oUnit.param = unit.param;
        }
        public static SkillUnit.SkillCameraAction ConvertCameraAction(SkillCameraAction unit)
        {
            SkillUnit.SkillCameraAction oUnit = new SkillUnit.SkillCameraAction();
            oUnit.action = (SkillUnit.SkillCameraAction.CameraAction)((int)unit.action);
            oUnit.delay = unit.delay;
            oUnit.param = unit.param;
            return oUnit;
        }
        public static void ConvertCameraAction(SkillUnit.SkillCameraAction oUnit, SkillCameraAction unit)
        {
            oUnit.action = (SkillUnit.SkillCameraAction.CameraAction)((int)unit.action);
            oUnit.delay = unit.delay;
            oUnit.param = unit.param;
        }
        public static JSkillUnit ConvertSkillUnit(SkillUnit.SkillUnit unit)
        {
            JSkillUnit oUnit = new JSkillUnit();
            oUnit.id = unit.id;
            oUnit.artId = unit.artId;
            oUnit.launchType = (JSkillUnit.LaunchType)(int)unit.launchType;
            oUnit.targetType = (JSkillUnit.TargetType)(int)unit.targetType;
            oUnit.skillTime = unit.skillTime;
            oUnit.cd = unit.cd;
            oUnit.distance = unit.distance;
            oUnit.referId = unit.referId;
            oUnit.guidePolicy = ConvertSkillGuidePolicy(unit.guidePolicy);

            if (oUnit.launchType == JSkillUnit.LaunchType.SINGLELINE)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.singeLines.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillLine line = EditorDataContainer.allSkillUnits.singeLines[unit.referId];
                    oUnit.skillObj = ConvertSkillLine(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.MULLINE)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.multLines.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillMultiLine line = EditorDataContainer.allSkillUnits.multLines[unit.referId];
                    oUnit.skillObj = ConvertSkillMultiLine(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.AREA)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.areas.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillArea line = EditorDataContainer.allSkillUnits.areas[unit.referId];
                    oUnit.skillObj = ConvertSkillArea(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.JUMP)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.jumps.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillJump line = EditorDataContainer.allSkillUnits.jumps[unit.referId];
                    oUnit.skillObj = ConvertSkillJump(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.AREA_RANDSKILL)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.areaRands.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillAreaRand line = EditorDataContainer.allSkillUnits.areaRands[unit.referId];
                    oUnit.skillObj = ConvertSkillAreaRand(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.HELIX)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.helixes.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillHelix line = EditorDataContainer.allSkillUnits.helixes[unit.referId];
                    oUnit.skillObj = ConvertSkillHelix(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.FOLLOW)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.follows.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillFollow line = EditorDataContainer.allSkillUnits.follows[unit.referId];
                    oUnit.skillObj = ConvertSkillFollow(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.BACK_STAB)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.backStabs.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillBackStab line = EditorDataContainer.allSkillUnits.backStabs[unit.referId];
                    oUnit.skillObj = ConvertSkillBackStab(line);
                }
            }
            return oUnit;
        }
        public static void ConvertSkillUnit(JSkillUnit oUnit, SkillUnit.SkillUnit unit)
        {
            oUnit.id = unit.id;
            oUnit.artId = unit.artId;
            oUnit.launchType = (JSkillUnit.LaunchType)(int)unit.launchType;
            oUnit.targetType = (JSkillUnit.TargetType)(int)unit.targetType;
            oUnit.skillTime = unit.skillTime;
            oUnit.cd = unit.cd;
            oUnit.distance = unit.distance;
            oUnit.referId = unit.referId;
            oUnit.guidePolicy = ConvertSkillGuidePolicy(unit.guidePolicy);

            if (oUnit.launchType == JSkillUnit.LaunchType.SINGLELINE)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.singeLines.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillLine line = EditorDataContainer.allSkillUnits.singeLines[unit.referId];
                    oUnit.skillObj = ConvertSkillLine(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.MULLINE)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.multLines.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillMultiLine line = EditorDataContainer.allSkillUnits.multLines[unit.referId];
                    oUnit.skillObj = ConvertSkillMultiLine(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.AREA)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.areas.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillArea line = EditorDataContainer.allSkillUnits.areas[unit.referId];
                    oUnit.skillObj = ConvertSkillArea(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.JUMP)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.jumps.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillJump line = EditorDataContainer.allSkillUnits.jumps[unit.referId];
                    oUnit.skillObj = ConvertSkillJump(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.AREA_RANDSKILL)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.areaRands.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillAreaRand line = EditorDataContainer.allSkillUnits.areaRands[unit.referId];
                    oUnit.skillObj = ConvertSkillAreaRand(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.HELIX)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.helixes.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillHelix line = EditorDataContainer.allSkillUnits.helixes[unit.referId];
                    oUnit.skillObj = ConvertSkillHelix(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.FOLLOW)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.follows.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillFollow line = EditorDataContainer.allSkillUnits.follows[unit.referId];
                    oUnit.skillObj = ConvertSkillFollow(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.BACK_STAB)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.backStabs.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillBackStab line = EditorDataContainer.allSkillUnits.backStabs[unit.referId];
                    oUnit.skillObj = ConvertSkillBackStab(line);
                }
            }
        }
        public static SkillUnit.SkillUnit ConvertSkillUnit(JSkillUnit unit)
        {
            SkillUnit.SkillUnit oUnit = new SkillUnit.SkillUnit();
            oUnit.id = unit.id;
            oUnit.artId = unit.artId;
            oUnit.launchType = (SkillUnit.SkillUnit.LaunchType)(int)unit.launchType;
            oUnit.targetType = (SkillUnit.SkillUnit.TargetType)(int)unit.targetType;
            oUnit.skillTime = unit.skillTime;
            oUnit.cd = unit.cd;
            oUnit.distance = unit.distance;
            oUnit.referId = unit.referId;
            oUnit.guidePolicy = ConvertSkillGuidePolicy(unit.guidePolicy);

            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.SINGLELINE)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.singeLines.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillLine line = EditorDataContainer.allSkillUnits.singeLines[unit.referId];
                    line = ConvertSkillLine((SkillLine)unit.skillObj);
                }
            }
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.MULLINE)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.multLines.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillMultiLine line = EditorDataContainer.allSkillUnits.multLines[unit.referId];
                    line= ConvertSkillMultiLine((SkillMultiLine)unit.skillObj);
                }
            }
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.AREA)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.areas.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillArea line = EditorDataContainer.allSkillUnits.areas[unit.referId];
                    line = ConvertSkillArea((SkillArea)unit.skillObj);
                }
            }
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.JUMP)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.jumps.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillJump line = EditorDataContainer.allSkillUnits.jumps[unit.referId];
                    line = ConvertSkillJump((SkillJump)unit.skillObj);
                }
            }
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.AREA_RANDSKILL)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.areaRands.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillAreaRand line = EditorDataContainer.allSkillUnits.areaRands[unit.referId];
                    line = ConvertSkillAreaRand((SkillAreaRand)unit.skillObj);
                }
            }
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.HELIX)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.helixes.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillHelix line = EditorDataContainer.allSkillUnits.helixes[unit.referId];
                    line= ConvertSkillHelix((SkillHelix)unit.skillObj);
                }
            }
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.FOLLOW)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.follows.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillFollow line = EditorDataContainer.allSkillUnits.follows[unit.referId];
                    line = ConvertSkillFollow((SkillFollow)unit.skillObj);
                }
            }
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.BACK_STAB)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.backStabs.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillBackStab line = EditorDataContainer.allSkillUnits.backStabs[unit.referId];
                    line = ConvertSkillBackStab((SkillBackStab)unit.skillObj);
                }
            }
            return oUnit;
        }
        public static void ConvertSkillUnit(SkillUnit.SkillUnit oUnit, JSkillUnit unit)
        {
            oUnit.id = unit.id;
            oUnit.artId = unit.artId;
            oUnit.launchType = (SkillUnit.SkillUnit.LaunchType)(int)unit.launchType;
            oUnit.targetType = (SkillUnit.SkillUnit.TargetType)(int)unit.targetType;
            oUnit.skillTime = unit.skillTime;
            oUnit.cd = unit.cd;
            oUnit.distance = unit.distance;
            oUnit.referId = unit.referId;
            oUnit.guidePolicy = ConvertSkillGuidePolicy(unit.guidePolicy);
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.SINGLELINE)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.singeLines.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillLine line = EditorDataContainer.allSkillUnits.singeLines[unit.referId];
                    line = ConvertSkillLine((SkillLine)unit.skillObj);
                }
            }
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.MULLINE)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.multLines.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillMultiLine line = EditorDataContainer.allSkillUnits.multLines[unit.referId];
                    line = ConvertSkillMultiLine((SkillMultiLine)unit.skillObj);
                }
            }
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.AREA)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.areas.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillArea line = EditorDataContainer.allSkillUnits.areas[unit.referId];
                    line = ConvertSkillArea((SkillArea)unit.skillObj);
                }
            }
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.JUMP)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.jumps.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillJump line = EditorDataContainer.allSkillUnits.jumps[unit.referId];
                    line = ConvertSkillJump((SkillJump)unit.skillObj);
                }
            }
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.AREA_RANDSKILL)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.areaRands.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillAreaRand line = EditorDataContainer.allSkillUnits.areaRands[unit.referId];
                    line = ConvertSkillAreaRand((SkillAreaRand)unit.skillObj);
                }
            }
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.HELIX)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.helixes.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillHelix line = EditorDataContainer.allSkillUnits.helixes[unit.referId];
                    line = ConvertSkillHelix((SkillHelix)unit.skillObj);
                }
            }
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.FOLLOW)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.follows.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillFollow line = EditorDataContainer.allSkillUnits.follows[unit.referId];
                    line = ConvertSkillFollow((SkillFollow)unit.skillObj);
                }
            }
            if (oUnit.launchType == SkillUnit.SkillUnit.LaunchType.BACK_STAB)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.backStabs.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillBackStab line = EditorDataContainer.allSkillUnits.backStabs[unit.referId];
                    line = ConvertSkillBackStab((SkillBackStab)unit.skillObj);
                }
            }
        }
 
        public static SkillArt ConvertSkillArt(SkillUnit.SkillArt unit)
        {
            SkillArt oUnit = new SkillArt();
            oUnit.id = unit.id;
            oUnit.guideAction = unit.guideAction;
            oUnit.guideFadeTime = unit.guideFadeTime;
            oUnit.guidingAction = unit.guidingAction;
            oUnit.endAction = unit.endAction;

            oUnit.unitEffect = new List<SkillEffectUnit>();
            oUnit.unitEffect.Add(new SkillEffectUnit());
            oUnit.unitEffect[0].artEffect = ConvertSkillArtEffect(unit.unitEffect);

            oUnit.tipEffect = new List<SkillEffectUnit>();
            oUnit.tipEffect.Add(new SkillEffectUnit());
            oUnit.tipEffect[0].artEffect = ConvertSkillArtEffect(unit.tipEffect);

            oUnit.hitEffect = new List<SkillEffectUnit>();
            oUnit.hitEffect.Add(new SkillEffectUnit());
            oUnit.hitEffect[0].artEffect = ConvertSkillArtEffect(unit.hitEffect);

            oUnit.beginCameraAction = new List<SkillCameraAction>();
            oUnit.beginCameraAction.Add(new SkillCameraAction());
            oUnit.beginCameraAction[0] = ConvertCameraAction(unit.beginCameraAction);

            oUnit.moveCameraAction = new List<SkillCameraAction>();
            oUnit.moveCameraAction.Add(new SkillCameraAction());
            oUnit.moveCameraAction[0] = ConvertCameraAction(unit.moveCameraAction);

            oUnit.hitCameraAction = new List<SkillCameraAction>();
            oUnit.hitCameraAction.Add(new SkillCameraAction());
            oUnit.hitCameraAction[0] = ConvertCameraAction(unit.hitCameraAction);

            oUnit.beginEffect = new List<SkillEffectUnit>();
            for (int i = 0; i < unit.beginEffect.Count; i++)
            {
                SkillEffectUnit Effect = new SkillEffectUnit();
                Effect.artEffect = ConvertSkillArtEffect(unit.beginEffect[i]);
                oUnit.beginEffect.Add(Effect);
            }

            return oUnit;
        }
        public static SkillUnit.SkillArt ConvertSkillArt(SkillArt unit)
        {
            SkillUnit.SkillArt oUnit = new SkillUnit.SkillArt();
            oUnit.id = unit.id;
            oUnit.guideAction = unit.guideAction;
            oUnit.guideFadeTime = unit.guideFadeTime;
            oUnit.guidingAction = unit.guidingAction;
            oUnit.endAction = unit.endAction;
            
            if(unit.unitEffect!=null && unit.unitEffect.Count>=1)
                oUnit.unitEffect = ConvertSkillArtEffect(unit.unitEffect[0].artEffect);
            if (unit.tipEffect != null && unit.tipEffect.Count >= 1)
                oUnit.tipEffect = ConvertSkillArtEffect(unit.tipEffect[0].artEffect);
            if (unit.hitEffect != null && unit.hitEffect.Count >= 1)
                oUnit.hitEffect = ConvertSkillArtEffect(unit.hitEffect[0].artEffect);

            if (unit.beginCameraAction != null && unit.beginCameraAction.Count >= 1)
                oUnit.beginCameraAction = ConvertCameraAction(unit.beginCameraAction[0]);
            if (unit.moveCameraAction != null && unit.moveCameraAction.Count >= 1)
                oUnit.moveCameraAction = ConvertCameraAction(unit.moveCameraAction[0]);
            if (unit.hitCameraAction != null && unit.hitCameraAction.Count >= 1)
                oUnit.hitCameraAction = ConvertCameraAction(unit.hitCameraAction[0]);

            List<SkillUnit.SkillArtEffect> beginlist = oUnit.beginEffect;
            for (int i = 0; i < unit.beginEffect.Count; i++)
            {
                SkillUnit.SkillArtEffect Effect = new SkillUnit.SkillArtEffect();
                Effect = ConvertSkillArtEffect(unit.beginEffect[i].artEffect);
                oUnit.beginEffect.Add(Effect);
            }

            return oUnit;
        }
        public static void ConvertSkillArt(SkillArt oUnit, SkillUnit.SkillArt unit)
        {
            oUnit.id = unit.id;
            oUnit.guideAction = unit.guideAction;
            oUnit.guideFadeTime = unit.guideFadeTime;
            oUnit.guidingAction = unit.guidingAction;
            oUnit.endAction = unit.endAction;
            if (oUnit.unitEffect == null)
            {
                oUnit.unitEffect = new List<SkillEffectUnit>();
            }
            if (oUnit.tipEffect == null)
            {
                oUnit.tipEffect = new List<SkillEffectUnit>();
            }
            if (oUnit.hitEffect == null)
            {
                oUnit.hitEffect = new List<SkillEffectUnit>();
            }

            if (oUnit.unitEffect.Count == 0)
            {
                oUnit.unitEffect.Add(new SkillEffectUnit());
            }
            if (oUnit.tipEffect.Count == 0)
            {
                oUnit.tipEffect.Add(new SkillEffectUnit());
            }
            if (oUnit.hitEffect.Count == 0)
            {
                oUnit.hitEffect.Add(new SkillEffectUnit());
            }
            ConvertSkillArtEffect(oUnit.unitEffect[0].artEffect, unit.unitEffect);
            ConvertSkillArtEffect(oUnit.tipEffect[0].artEffect, unit.tipEffect);
            ConvertSkillArtEffect(oUnit.hitEffect[0].artEffect, unit.hitEffect);
            if (oUnit.beginEffect == null)
                oUnit.beginEffect = new List<SkillEffectUnit>();
            oUnit.beginEffect.Clear();
            for (int i = 0; i < unit.beginEffect.Count; i++)
            {
                SkillEffectUnit Effect = new SkillEffectUnit();
                ConvertSkillArtEffect(Effect.artEffect, unit.beginEffect[i]);
                oUnit.beginEffect.Add(Effect);
            }

        }
        public static void ConvertSkillArt(SkillUnit.SkillArt oUnit, SkillArt unit)
        {
            oUnit.id = unit.id;
            oUnit.guideAction = unit.guideAction;
            oUnit.guideFadeTime = unit.guideFadeTime;
            oUnit.guidingAction = unit.guidingAction;
            oUnit.endAction = unit.endAction;

            oUnit.unitEffect = ConvertSkillArtEffect(unit.unitEffect[0].artEffect);
            oUnit.tipEffect = ConvertSkillArtEffect(unit.tipEffect[0].artEffect);
            oUnit.hitEffect = ConvertSkillArtEffect(unit.hitEffect[0].artEffect);

            oUnit.beginCameraAction = ConvertCameraAction(unit.beginCameraAction[0]);
            oUnit.moveCameraAction = ConvertCameraAction(unit.moveCameraAction[0]);
            oUnit.hitCameraAction = ConvertCameraAction(unit.hitCameraAction[0]);

            List<SkillUnit.SkillArtEffect> beginlist = oUnit.beginEffect;
            for (int i = 0; i < unit.beginEffect.Count; i++)
            {
                SkillUnit.SkillArtEffect Effect = new SkillUnit.SkillArtEffect();
                Effect = ConvertSkillArtEffect(unit.beginEffect[i].artEffect);
                oUnit.beginEffect.Add(Effect);
            }
        }
    }


}