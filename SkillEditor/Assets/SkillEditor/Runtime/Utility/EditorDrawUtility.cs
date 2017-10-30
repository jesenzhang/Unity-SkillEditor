using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace CySkillEditor
{
    public class EditorDrawUtility
    {
        public static string GetSkillStringById(int id)
        {
            return EditorDataContainer.GetSkillStringById(id);
        }
        public static int GetSkillIdByString(string id)
        {
            return EditorDataContainer.GetSkillIdByString(id);
        }
        public static string GetStringById(int id)
        {
            return EditorDataContainer.GetStringById(id);
        }
        public static int GetIdByString(string id)
        {
            return EditorDataContainer.GetIdByString(id);
        }

        //绘制一个popup区域
        public static void DrawActionRect(string title, string inaction, out string content, List<string> stateNamelist)
        {
            EditorGUILayout.BeginHorizontal();
            string guidestr = EditorGUILayout.TextField(title, inaction);
            int gaindex = -1;
            if (stateNamelist.Contains(guidestr))
            {
                content = guidestr;
            }
            else
            {
                content = "-1";
            }
            if (stateNamelist != null && stateNamelist.Contains(content))
                gaindex = stateNamelist.IndexOf(content);
            int newgaindex = EditorGUILayout.Popup("", gaindex, stateNamelist.ToArray());
            if (gaindex != newgaindex)
            {
                content = stateNamelist[newgaindex];
            }
            EditorGUILayout.EndHorizontal();
        }
        //绘制特效的配置
        public static void DrawEffectConfigure(EffectConfigure effect)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("  特效配置：");
            effect.posType = (CySkillEditor.EffectConfigure.PosType)EditorGUILayout.Popup("  posType", (int)effect.posType, Enum.GetNames(typeof(CySkillEditor.EffectConfigure.PosType)));
            effect.effectName = EditorGUILayout.TextField("  effectName", effect.effectName);
            if (effect.posType == CySkillEditor.EffectConfigure.PosType.BODY)
            {
                effect.bodyHeight = EditorGUILayout.FloatField("  bodyHeight", effect.bodyHeight);
            }
            else
            if (effect.posType == CySkillEditor.EffectConfigure.PosType.HEAD)
            {
                effect.headHeight = EditorGUILayout.FloatField("  headHeight", effect.headHeight);
            }
            else
            if (effect.posType == CySkillEditor.EffectConfigure.PosType.BONE)
            {
                effect.boneName = EditorGUILayout.TextField("  boneName", "" + effect.boneName);
            }
            else
            if (effect.posType == CySkillEditor.EffectConfigure.PosType.BODY)
            {
                effect.bodyHeight = EditorGUILayout.FloatField("  bodyHeight", effect.bodyHeight);
            }
            else
            if (effect.posType == CySkillEditor.EffectConfigure.PosType.FEET)
            {
                effect.feetWidth = EditorGUILayout.FloatField("  feetWidth", effect.feetWidth);
            }
            effect.position = EditorGUILayout.Vector3Field("  position", effect.position);
            effect.rotation = EditorGUILayout.Vector3Field("  rotation", effect.rotation);
            effect.lifeTime = (CySkillEditor.EffectConfigure.LifeTime)EditorGUILayout.Popup("  lifeTime", (int)effect.lifeTime, Enum.GetNames(typeof(CySkillEditor.EffectConfigure.LifeTime)));
            EditorGUILayout.EndVertical();
        }
        //绘制特效单位
        public static void DrawSkillArtEffect(SkillArtEffect Art, List<string> Poplist = null, List<GameObject> Objlist = null)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("  特效：");
            GameObject effect = null;
            if (Poplist != null && Objlist != null)
            {
                int selet = -1;
                if (Art.effectObj != null)
                {
                    effect = Art.effectObj;
                    string effecttname = Art.effectObj.name;
                    if (Poplist.Contains(effecttname))
                    {
                        selet = Poplist.IndexOf(effecttname);
                        effect = Objlist[selet];
                    }
                }
                EditorGUILayout.BeginHorizontal();
                GameObject neweffect = (GameObject)EditorGUILayout.ObjectField("  unitEffect:", effect, typeof(GameObject), true);
                int newselect = EditorGUILayout.Popup(selet, Poplist.ToArray());
                EditorGUILayout.EndHorizontal();
                if (newselect != selet)
                {
                    Art.effect = GetIdByString(Poplist[newselect]);
                    Art.effectObj = Objlist[newselect];
                }
                if (neweffect != effect)
                {
                    Art.effectObj = neweffect;
                    string effecttname = Art.effectObj.name;
                }
            }
            else
            {
                if (Art.effectObj != null)
                    effect = Art.effectObj;
                Art.effectObj = (GameObject)EditorGUILayout.ObjectField("  unitEffect:", effect, typeof(GameObject), true);
            }
            Art.beginTime = EditorGUILayout.IntField("  beginTime:", Art.beginTime);
            Art.phaseTime = EditorGUILayout.IntField("  phaseTime:", Art.phaseTime);
            Art.height = EditorGUILayout.FloatField("  height:", Art.height);
            Art.effPos = (SkillArtEffect.EffPos)EditorGUILayout.Popup("  effPos:", (int)Art.effPos, Enum.GetNames(typeof(SkillArtEffect.EffPos)));
            EditorGUILayout.EndVertical();
        }
        //绘制特效单元
        public static void DrawSkillEffectUnit(SkillEffectUnit unit, List<string> Poplist = null, List<GameObject> Objlist = null)
        {
            DrawSkillArtEffect(unit.artEffect, Poplist, Objlist);
            DrawEffectConfigure(unit.configure);
        }
        //绘制形状
        public static void DrawSkillShape(string title, SkillShape hitarea)
        {
            EditorGUILayout.BeginVertical("Box");
            int newarea = EditorGUILayout.Popup(title, (int)hitarea.area, Enum.GetNames(typeof(SkillShape.Area)));
            if (newarea != (int)hitarea.area)
            {
                hitarea.area = (SkillShape.Area)newarea;
            }
            string p1 = "";
            string p2 = "";
            string p3 = "";
            if (hitarea.area == SkillShape.Area.QUADRATE)
            {
                p1 = "长"; p2 = "宽";
            }
            if (hitarea.area == SkillShape.Area.CIRCLE)
            {
                p1 = "半径";
            }
            if (hitarea.area == SkillShape.Area.SECTOR)
            {
                p1 = "半径"; p3 = "弧度";
            }
            if (hitarea.area == SkillShape.Area.TRIANGLE)
            {
                p1 = "高"; p2 = "底"; p3 = "1正2反";
            }

            hitarea.param1 = EditorGUILayout.FloatField("  param1:" + p1, hitarea.param1);
            hitarea.param2 = EditorGUILayout.FloatField("  param2:" + p2, hitarea.param2);
            hitarea.param3 = EditorGUILayout.FloatField("  param3:" + p3, hitarea.param3);
            EditorGUILayout.EndVertical();
        }
        //绘制相机特效
        public static void DrawCameraAction(SkillCameraAction action)
        {
            EditorGUILayout.BeginVertical("Box");
            string inType = Enum.GetName(typeof(SkillCameraAction.CameraAction), action.action);
            DrawActionRect("类型", inType, out inType, new List<string>(Enum.GetNames(typeof(SkillCameraAction.CameraAction))));
            action.action = (SkillCameraAction.CameraAction)Enum.Parse(typeof(SkillCameraAction.CameraAction), inType);
            if (action.action == SkillCameraAction.CameraAction.CAMERAACTION_SHAKE)
            {
                action.shakeRange = EditorGUILayout.FloatField("  shakeRange:", action.shakeRange);
                action.shakeInterval = EditorGUILayout.IntField("  shakeInterval:", action.shakeInterval);
            }

            action.param = EditorGUILayout.FloatField("  param:", action.param);
            action.delay = EditorGUILayout.FloatField("  delay:", action.delay);
            action.phaseTime = EditorGUILayout.FloatField("  phaseTime:", action.phaseTime);

            EditorGUILayout.EndVertical();
        }
        //绘制技能引导
        public static void DrawGuidePolicy(SkillGuidePolicy policy)
        {
            EditorGUILayout.BeginVertical("Box");
            string inType = Enum.GetName(typeof(SkillGuidePolicy.GuideType), policy.type);
            DrawActionRect("GuideType 类型:", inType, out inType, new List<string>(Enum.GetNames(typeof(SkillGuidePolicy.GuideType))));
            policy.type = (SkillGuidePolicy.GuideType)Enum.Parse(typeof(SkillGuidePolicy.GuideType), inType);
            policy.guideTime = EditorGUILayout.IntField("  guideTime:", policy.guideTime);
            policy.guidingTime = EditorGUILayout.IntField("  guidingTime:", policy.guidingTime);
            policy.endTime = EditorGUILayout.IntField("  endTime:", policy.endTime);
            EditorGUILayout.EndVertical();
        }
        //绘制弹道技能
        public static void DrawSkillSingleLine(SkillLine line)
        {
            EditorGUILayout.BeginVertical("Box");
            line.id = EditorGUILayout.IntField("  id", line.id);
            line.moveTime = EditorGUILayout.IntField("  moveTime", line.moveTime);
            line.speed = EditorGUILayout.FloatField("  speed:", line.speed);
            line.waves = EditorGUILayout.IntField("  waves:", line.waves);
            line.waveDelay = EditorGUILayout.IntField("  waveDelay:", line.waveDelay);
            line.maxInfluence = EditorGUILayout.IntField("  maxInfluence:", line.maxInfluence);
            line.canPierce = EditorGUILayout.Toggle("  canPierce:", line.canPierce);
            line.offset = EditorGUILayout.Vector3Field("  offset:", line.offset);
            SkillShape hitarea = line.hitArea;
            DrawSkillShape("  hitArea:", hitarea);
            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillMultiLine(SkillMultiLine line)
        {
            EditorGUILayout.BeginVertical("Box");
            line.id = EditorGUILayout.IntField("  id", line.id);
            line.moveTime = EditorGUILayout.IntField("  moveTime", line.moveTime);
            line.unitCount = EditorGUILayout.IntField("  unitCount:", line.unitCount);
            line.speed = EditorGUILayout.FloatField("  speed:", line.speed);
            line.waves = EditorGUILayout.IntField("  waves:", line.waves);
            line.waveDelay = EditorGUILayout.IntField("  waveDelay:", line.waveDelay);
            line.maxInfluence = EditorGUILayout.IntField("  maxInfluence:", line.maxInfluence);
            line.canPierce = EditorGUILayout.Toggle("  canPierce:", line.canPierce);
            line.offset = EditorGUILayout.Vector3Field("  offset:", line.offset);
            SkillShape hitarea = line.hitArea;
            DrawSkillShape("  hitArea:", hitarea);
            SkillShape shape = line.shape;
            DrawSkillShape("  shape:", shape);

            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillArea(SkillArea line)
        {
            EditorGUILayout.BeginVertical("Box");
            line.id = EditorGUILayout.IntField("  id", line.id);
            line.moveDelay = EditorGUILayout.IntField("  moveDelay", line.moveDelay);
            line.waves = EditorGUILayout.IntField("  waves:", line.waves);
            line.waveDelay = EditorGUILayout.IntField("  waveDelay:", line.waveDelay);
            line.maxInfluence = EditorGUILayout.IntField("  maxInfluence:", line.maxInfluence);
            JSkillUnit.BasePoint bp = line.basePoint;
            int newbp = EditorGUILayout.Popup("  basePoint:", (int)bp, Enum.GetNames(typeof(JSkillUnit.BasePoint)));
            if (newbp != (int)bp)
            {
                bp = (JSkillUnit.BasePoint)newbp;
            }
            JSkillUnit.ReferPoint shape = line.referPoint;
            int newshape = EditorGUILayout.Popup("  referPoint:", (int)shape, Enum.GetNames(typeof(JSkillUnit.ReferPoint)));
            if (newshape != (int)shape)
            {
                shape = (JSkillUnit.ReferPoint)newshape;
            }
            SkillShape hitarea = line.hitArea;
            DrawSkillShape("  hitArea:", hitarea);

            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillJump(SkillJump line)
        {
            EditorGUILayout.BeginVertical("Box");
            line.id = EditorGUILayout.IntField("  id", line.id);
            line.moveTime = EditorGUILayout.IntField("  moveTime", line.moveTime);
            line.speed = EditorGUILayout.FloatField("  speed:", line.speed);
            line.height = EditorGUILayout.FloatField("  height:", line.height);
            line.maxInfluence = EditorGUILayout.IntField("  maxInfluence:", line.maxInfluence);
            SkillJump.JumpType bp = line.jumpType;
            int newbp = EditorGUILayout.Popup("  jumpType:", (int)bp, Enum.GetNames(typeof(SkillJump.JumpType)));
            if (newbp != (int)bp)
            {
                bp = (SkillJump.JumpType)newbp;
            }
            SkillShape hitarea = line.hitArea;
            DrawSkillShape("  hitArea:", hitarea);
            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillAreaRandom(SkillAreaRand line)
        {
            EditorGUILayout.BeginVertical("Box");
            line.id = EditorGUILayout.IntField("  id", line.id);
            line.unitID = EditorGUILayout.IntField("  unitID", line.unitID);
            line.unitCount = EditorGUILayout.IntField("  unitCount:", line.unitCount);
            JSkillUnit.BasePoint bp = line.basePoint;
            int newbp = EditorGUILayout.Popup("  basePoint:", (int)bp, Enum.GetNames(typeof(JSkillUnit.BasePoint)));
            if (newbp != (int)bp)
            {
                bp = (JSkillUnit.BasePoint)newbp;
            }
            JSkillUnit.ReferPoint shape = line.referPoint;
            int newshape = EditorGUILayout.Popup("  referPoint:", (int)shape, Enum.GetNames(typeof(JSkillUnit.ReferPoint)));
            if (newshape != (int)shape)
            {
                shape = (JSkillUnit.ReferPoint)newshape;
            }
            SkillShape hitarea = line.area;
            DrawSkillShape("  area:", hitarea);
            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillHelix(SkillHelix line)
        {
            EditorGUILayout.BeginVertical("Box");
            line.id = EditorGUILayout.IntField("  id", line.id);
            line.moveTime = EditorGUILayout.IntField("  moveTime", line.moveTime);
            line.maxRadius = EditorGUILayout.FloatField("  maxRadius:", line.maxRadius);
            line.maxInfluence = EditorGUILayout.IntField("  maxInfluence:", line.maxInfluence);
            line.canPierce = EditorGUILayout.Toggle("  canPierce:", line.canPierce);
            line.offset = EditorGUILayout.Vector3Field("  offset:", line.offset);
            SkillShape hitarea = line.hitArea;
            DrawSkillShape("  hitArea:", hitarea);
            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillFollow(SkillFollow line)
        {
            EditorGUILayout.BeginVertical("Box");
            line.id = EditorGUILayout.IntField("  id", line.id);
            line.maxFollowTime = EditorGUILayout.IntField("  maxFollowTime", line.maxFollowTime);
            line.speed = EditorGUILayout.FloatField("  speed:", line.speed);
            line.waves = EditorGUILayout.IntField("  waves:", line.waves);
            line.waveDelay = EditorGUILayout.IntField("  waveDelay:", line.waveDelay);
            SkillShape hitarea = line.hitArea;
            DrawSkillShape("  hitArea:", hitarea);
            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillBackStab(SkillBackStab line)
        {
            EditorGUILayout.BeginVertical("Box");
            line.id = EditorGUILayout.IntField("  id", line.id);
            line.moveDelay = EditorGUILayout.IntField("  moveDelay", line.moveDelay);
            line.maxInfluence = EditorGUILayout.IntField("  maxInfluence:", line.maxInfluence);
            JSkillUnit.BasePoint bp = line.basePoint;
            int newbp = EditorGUILayout.Popup("  basePoint:", (int)bp, Enum.GetNames(typeof(JSkillUnit.BasePoint)));
            if (newbp != (int)bp)
            {
                bp = (JSkillUnit.BasePoint)newbp;
            }
            JSkillUnit.ReferPoint shape = line.referPoint;
            int newshape = EditorGUILayout.Popup("  referPoint:", (int)shape, Enum.GetNames(typeof(JSkillUnit.ReferPoint)));
            if (newshape != (int)shape)
            {
                shape = (JSkillUnit.ReferPoint)newshape;
            }
            SkillShape hitarea = line.hitArea;
            DrawSkillShape("  hitArea:", hitarea);
            EditorGUILayout.EndVertical();
        }
        //绘制技能信息区域
        public static void DrawSkillUnit(JSkillUnit unit)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Skill unit: 技能信息");
            JSkillUnit.LaunchType lt = unit.launchType;
            int newlt = EditorGUILayout.Popup("  LaunchType:", (int)lt, Enum.GetNames(typeof(JSkillUnit.LaunchType)));
            if (newlt != (int)lt)
            {
                unit.launchType = (JSkillUnit.LaunchType)newlt;
            }
            JSkillUnit.TargetType tt = unit.targetType;
            int newtt = EditorGUILayout.Popup("  TargetType:", (int)tt, Enum.GetNames(typeof(JSkillUnit.TargetType)));
            if (newtt != (int)tt)
            {
                unit.targetType = (JSkillUnit.TargetType)newtt;
            }
            unit.skillTime = EditorGUILayout.IntField("  skillTime:", unit.skillTime);
            unit.cd = EditorGUILayout.IntField("  cd:", unit.cd);
            unit.distance = EditorGUILayout.FloatField("  distance:", unit.distance);
            unit.referId = EditorGUILayout.IntField("  referId:", unit.referId);
            unit.artId = EditorGUILayout.IntField("  artId:", unit.artId);
            if (unit.guidePolicy != null)
                DrawGuidePolicy(unit.guidePolicy);
            else
            {
                if (GUILayout.Button("Add GuidePolicy:添加引导策略"))
                {
                    unit.guidePolicy = new SkillGuidePolicy();
                }
            }


            EditorGUILayout.LabelField("弹道信息:");
            if (unit.skillObj != null)
            {
                if (unit.launchType == JSkillUnit.LaunchType.SINGLELINE)
                {
                    if (!(unit.skillObj is SkillLine))
                    {
                        unit.skillObj = new SkillLine();
                    }
                    DrawSkillSingleLine((SkillLine)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.MULLINE)
                {
                    if (!(unit.skillObj is SkillMultiLine))
                    {
                        unit.skillObj = new SkillMultiLine();
                    }
                    DrawSkillMultiLine((SkillMultiLine)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.AREA)
                {
                    if (!(unit.skillObj is SkillArea))
                    {
                        unit.skillObj = new SkillArea();
                    }
                    DrawSkillArea((SkillArea)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.JUMP)
                {
                    if (!(unit.skillObj is SkillJump))
                    {
                        unit.skillObj = new SkillJump();
                    }
                    DrawSkillJump((SkillJump)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.AREA_RANDSKILL)
                {
                    if (!(unit.skillObj is SkillAreaRand))
                    {
                        unit.skillObj = new SkillAreaRand();
                    }
                    DrawSkillAreaRandom((SkillAreaRand)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.HELIX)
                {
                    if (!(unit.skillObj is SkillHelix))
                    {
                        unit.skillObj = new SkillHelix();
                    }
                    DrawSkillHelix((SkillHelix)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.FOLLOW)
                {
                    if (!(unit.skillObj is SkillFollow))
                    {
                        unit.skillObj = new SkillFollow();
                    }
                    DrawSkillFollow((SkillFollow)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.BACK_STAB)
                {
                    if (!(unit.skillObj is SkillBackStab))
                    {
                        unit.skillObj = new SkillBackStab();
                    }
                    DrawSkillBackStab((SkillBackStab)unit.skillObj);
                }
            }
            else
            {
                if (GUILayout.Button("Add New Unit:添加弹道"))
                {
                    unit.skillObj = new SkillLine();
                }
            }
            EditorGUILayout.EndVertical();
        }

        //绘制特效列表
        public static void DrawEffectUnitList(List<SkillEffectUnit> beginEffect, Vector2 beginScrollPos, out Vector2 oScrollPos, float width, ref ReorderableList beginEffectList, List<string> Poplist, List<GameObject> Objlist)
        {
            EditorGUILayout.BeginVertical();
            //起手动作特效
            oScrollPos = EditorGUILayout.BeginScrollView(beginScrollPos);
            if (beginEffectList == null)
            {
                // 加入数据数组
                beginEffectList = new ReorderableList(beginEffect, typeof(SkillEffectUnit), false, false, true, true);
            }
            beginEffectList.elementHeight = 300;
            if (beginEffect == null || beginEffect.Count == 0)
                beginEffectList.elementHeight = 20;
            // 绘制Item显示列表
            beginEffectList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SkillEffectUnit element = (SkillEffectUnit)beginEffect[index];
                Rect drawRect = new Rect(rect.x, rect.y + 20, width - 30, 300);
                GUILayout.BeginArea(drawRect);
                GUILayout.Label("index: " + index);
                EditorDrawUtility.DrawSkillEffectUnit(element, Poplist, Objlist);
                GUILayout.EndArea();
            };
            beginEffectList.onRemoveCallback = (ReorderableList l) =>
            {
                l.list.RemoveAt(l.index);
            };
            beginEffectList.onAddCallback = (ReorderableList l) =>
            {
                l.list.Add(new SkillEffectUnit());
            };
            beginEffectList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        //绘制特效列表
        public static void DrawCameraActionList(List<SkillCameraAction> beginEffect, Vector2 beginScrollPos, out Vector2 oScrollPos, float width, ref ReorderableList beginEffectList)
        {
            EditorGUILayout.BeginVertical();
            //起手动作特效
            oScrollPos = EditorGUILayout.BeginScrollView(beginScrollPos);
            if (beginEffectList == null)
            {
                // 加入数据数组
                beginEffectList = new ReorderableList(beginEffect, typeof(SkillCameraAction), false, false, true, true);
            }
            beginEffectList.elementHeight = 280;
            if (beginEffect == null || beginEffect.Count == 0)
                beginEffectList.elementHeight = 20;
            // 绘制Item显示列表
            beginEffectList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SkillCameraAction element = (SkillCameraAction)beginEffect[index];
                Rect drawRect = new Rect(rect.x, rect.y + 20, width - 30, 280);
                GUILayout.BeginArea(drawRect);
                GUILayout.Label("index: " + index);
                EditorDrawUtility.DrawCameraAction(element);
                GUILayout.EndArea();
            };
            beginEffectList.onRemoveCallback = (ReorderableList l) =>
            {
                l.list.RemoveAt(l.index);
            };
            beginEffectList.onAddCallback = (ReorderableList l) =>
            {
                l.list.Add(new SkillCameraAction());
            };
            beginEffectList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

    }
}