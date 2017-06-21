using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CySkillEditor
{
    public class ConfigureWindow : EditorWindow
    {
        public static Vector2 minWindowSize = new Vector2(750.0f, 250.0f);
        private JSequencer currentSequence;
        public JSequencer CurrentSequence
        {
            set
            {
                currentSequence = value;
            }
            get
            {
                return currentSequence;
            }
        }

        //模型数组
        List<GameObject> allFbx = null;
        //模型名称数组
        List<string> fbxNamelist = new List<string>();
        //动作名称数组
        List<string> stateNamelist = new List<string>();
        //技能特效数组
        List<GameObject> SkillEffectPrefabs = null;
        //技能特效名称
        List<string> effectNamelist = null;

        //begineffect数组
        Vector2 beginScrollPos = new Vector2(0, 0);
        private ReorderableList beginEffectList;

        //uniteffect数组
        Vector2 unitEffectScrollPos = new Vector2(0, 0);
        private ReorderableList unitEffectList;


        //skillunit列表
        Vector2 unitScrollPos = new Vector2(0, 0);
        private ReorderableList unitList;
        //art列表
        Vector2 artScrollPos = new Vector2(0, 0);
        private ReorderableList artList;

        //列表显示标记
        bool showUnitList = false;
        bool showArtList = false;
        //列表选择标记
        int SelectSkillUnit = 0;
        int SelectSkillArt = 0;

        //根据模型类型 加载FBX
        private void LoadFbx()
        {
            SkillArt skillart = EditorDataContainer.currentskillAssetData.skillArt;
            if (allFbx == null)
            {
                allFbx = AssetUtility.GetAllFBXWithType(skillart.modelType);
                fbxNamelist = new List<string>();
                for (int i = 0; i < allFbx.Count; i++)
                {
                    fbxNamelist.Add(allFbx[i].name);
                }
            }
        }
        //加载动作
        private void LoadAnimationState(bool force = true)
        {
            SkillArt skillart = EditorDataContainer.currentskillAssetData.skillArt;
            string modelName = "";
            modelName = skillart.modelName;
            RuntimeAnimatorController controller = AssetUtility.GetAnimationCtl(skillart.modelType, modelName, skillart.animationController);
            if (force && controller == null)
                EditorUtility.DisplayDialog("Error!", "Error animationController Not Found", "OK");
            if (controller != null)
            {
                if (force)
                {
                    stateNamelist = new List<string>();
                    stateNamelist = MecanimAnimationUtility.GetAllStateNamesWithController(controller);
                }
                else
                {
                    if (stateNamelist == null)
                    {
                        stateNamelist = new List<string>();
                        stateNamelist = MecanimAnimationUtility.GetAllStateNamesWithController(controller);
                    }
                }
            }
            else if (stateNamelist == null)
            {
                stateNamelist = new List<string>();
            }
            else if (stateNamelist != null)
            {
                stateNamelist.Clear();
            }

        }
        //加载技能特效
        private void LoadEffect()
        {
            SkillArt skillart = EditorDataContainer.currentskillAssetData.skillArt;
            if (SkillEffectPrefabs == null)
            {
                string modelName = "";
                if (skillart.model != null)
                {
                    modelName = skillart.model.name;
                }
                SkillEffectPrefabs = AssetUtility.GetModelSkillEffectPrefabs(modelName); //GetAllSkillEffectPrefabs();
                effectNamelist = new List<string>();
                for (int i = 0; i < SkillEffectPrefabs.Count; i++)
                {
                    effectNamelist.Add(SkillEffectPrefabs[i].name);
                }
            }
        }
        private string GetSkillStringById(int id)
        {
            return EditorDataContainer.GetSkillStringById(id);
        }
        private int GetSkillIdByString(string id)
        {
            return EditorDataContainer.GetSkillIdByString(id);
        }
        private string GetStringById(int id)
        {
            return EditorDataContainer.GetStringById(id);
        }
        private int GetIdByString(string id)
        {
            return EditorDataContainer.GetIdByString(id);
        }
        
        //绘制特效的配置
        public void DrawEffectConfigure(EffectConfigure effect)
        {
            EditorGUILayout.BeginVertical();
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
                effect.headHeight = EditorGUILayout.FloatField("  headHeight",effect.headHeight);
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
        //绘制一个特效单位
        public void DrawSkillEffectUnit(SkillEffectUnit Art, List<string> Poplist, List<GameObject> Objlist)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GameObject effect = null;
            int selet = -1;
            if (Art.artEffect.effectObj != null)
            {
                effect = Art.artEffect.effectObj;
                string effecttname = Art.artEffect.effectObj.name;
                if (Poplist.Contains(effecttname))
                {
                    selet = Poplist.IndexOf(effecttname);
                    effect = Objlist[selet];
                }
            }
            GameObject neweffect = (GameObject)EditorGUILayout.ObjectField("  unitEffect:", effect, typeof(GameObject), true);
            int newselect = EditorGUILayout.Popup(selet, Poplist.ToArray());
            if (newselect != selet)
            {
                Art.artEffect.effect = GetIdByString(Poplist[newselect]);
                Art.artEffect.effectObj = Objlist[newselect];
            }
            if (neweffect != effect)
            {
                Art.artEffect.effectObj = neweffect;
                string effecttname = Art.artEffect.effectObj.name;
            }
            GUILayout.EndHorizontal();
            Art.artEffect.beginTime = EditorGUILayout.IntField("  beginTime:",  Art.artEffect.beginTime);
            Art.artEffect.phaseTime = EditorGUILayout.IntField("  phaseTime:", Art.artEffect.phaseTime);
            Art.artEffect.height = EditorGUILayout.FloatField("  height:", Art.artEffect.height);
            Art.artEffect.effPos = (SkillArtEffect.EffPos)EditorGUILayout.Popup("  effPos:", (int)Art.artEffect.effPos, Enum.GetNames(typeof(SkillArtEffect.EffPos)));
            DrawEffectConfigure(Art.configure);
            EditorGUILayout.EndVertical();
        }
        //绘制特效列表
        public void DrawEffectUnitList(List<SkillEffectUnit> beginEffect, Vector2 beginScrollPos, out Vector2 oScrollPos,float width, ref ReorderableList beginEffectList, List<string> Poplist, List<GameObject> Objlist)
        {
            EditorGUILayout.BeginVertical();
            //起手动作特效
            oScrollPos = EditorGUILayout.BeginScrollView(beginScrollPos);
            if (beginEffectList == null)
            {
                // 加入数据数组
                beginEffectList = new ReorderableList(beginEffect, typeof(SkillEffectUnit), false, false, true, true);
            }
            beginEffectList.elementHeight = 250;
            if (beginEffect == null || beginEffect.Count == 0)
                beginEffectList.elementHeight = 20;
            // 绘制Item显示列表
            beginEffectList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SkillEffectUnit element = (SkillEffectUnit)beginEffect[index];
                Rect drawRect = new Rect(rect.x, rect.y + 20, width-30, 230);
                GUILayout.BeginArea(drawRect);
                GUILayout.Label("index: " + index);
                DrawSkillEffectUnit(element, Poplist, Objlist);
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
        //绘制特效信息
        public void DrawSkillArtEffect(SkillArt skillart,float width)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Skill ArtEffect: 特效");
            //id
            if (skillart.idString == "")
                skillart.idString = GetSkillStringById(skillart.id);
            string id = EditorGUILayout.TextField("Id:", skillart.idString);
            if (id != skillart.idString)
            {
                skillart.idString = id;
                skillart.id = GetSkillIdByString(id);
            }
            //modelType
            int typeindex = (int)skillart.modelType;
            int newtypeindex = EditorGUILayout.Popup("ModelTargetType:", typeindex, Enum.GetNames(typeof(ModelTargetType)));
            if (newtypeindex != typeindex)
            {
                skillart.modelType = (ModelTargetType)newtypeindex;
                allFbx = null;
                LoadFbx();
            }
            int fbxselet = -1;
            // GameObject fbx = AssetUtility.GetFBXWithName(EditorDataContainer.currentskillAssetData.modelName);
            GameObject fbx = EditorDataContainer.currentskillAssetData.skillArt.model;
            if (fbx != null)
            {
                if (fbxNamelist.Contains(fbx.name))
                {
                    fbxselet = fbxNamelist.IndexOf(fbx.name);
                }
            }
            LoadAnimationState(false);
            EditorGUILayout.BeginHorizontal();
            GameObject NewFbx = (GameObject)EditorGUILayout.ObjectField("Model:", fbx, typeof(GameObject), true);
            int newselectfbx = EditorGUILayout.Popup(fbxselet, fbxNamelist.ToArray());
            EditorGUILayout.EndHorizontal();
            if (newselectfbx != fbxselet)
            {
                skillart.model = allFbx[newselectfbx];
                skillart.modelName = allFbx[newselectfbx].name;
                skillart.animationController = allFbx[newselectfbx].name + "_Anim";
                LoadAnimationState();
                SkillEffectPrefabs = null;
                LoadEffect();
            }
            else
            {
                if (NewFbx == null)
                {
                    skillart.model = null;
                    skillart.modelName = "";
                    skillart.animationController = "";
                }
                else if (NewFbx != skillart.model)
                {
                    skillart.model = NewFbx;
                    skillart.modelName = NewFbx.name;
                    skillart.animationController = NewFbx.name + "_Anim";
                    LoadAnimationState();
                    SkillEffectPrefabs = null;
                    LoadEffect();
                }
            }
            string newcontroller = EditorGUILayout.TextField("animationController:", skillart.animationController);
            if (newcontroller != skillart.animationController)
            {
                skillart.animationController = newcontroller;
                LoadAnimationState();
            }
            EditorGUILayout.LabelField("GuideAction 开始动作:");
            skillart.guideFadeTime = EditorGUILayout.FloatField("  guideFadeTime:", skillart.guideFadeTime);
            EditorDrawUtility.DrawActionRect("  guideAction:", skillart.guideAction, out skillart.guideAction, stateNamelist);
            EditorDrawUtility.DrawActionRect("  guidingAction:", skillart.guidingAction, out skillart.guidingAction, stateNamelist);
            EditorDrawUtility.DrawActionRect("  endAction:", skillart.endAction, out skillart.endAction, stateNamelist);
         
            EditorGUILayout.LabelField("unitEffect 弹道特效:");
            if (skillart.unitEffect != null)
            {
                DrawEffectUnitList(skillart.unitEffect, unitEffectScrollPos, out unitEffectScrollPos, width, ref unitEffectList, effectNamelist, SkillEffectPrefabs);
            }
            else
            {
                if (GUILayout.Button("Add unitEffectList"))
                {
                    skillart.unitEffect = new List<SkillEffectUnit>();
                }
            }
            EditorGUILayout.LabelField("beginEffect 起手特效:");
            if (skillart.beginEffect != null)
            {
                DrawEffectUnitList(skillart.beginEffect, beginScrollPos, out beginScrollPos, width, ref beginEffectList, effectNamelist, SkillEffectPrefabs);
            }
            else
            {
                if (GUILayout.Button("Add beginEffectList"))
                {
                    skillart.beginEffect = new List<SkillEffectUnit>();
                }
            }
            EditorGUILayout.LabelField("beginCameraAction 开始相机特效:");
            if (skillart.beginCameraAction != null)
                EditorDrawUtility.DrawCameraAction(skillart.beginCameraAction);
            else
                if (GUILayout.Button("Add beginCameraAction 开始相机特效"))
            {
                skillart.beginCameraAction = new SkillCameraAction();
            }
            EditorGUILayout.LabelField("moveCameraAction 移动相机特效:");
            if (skillart.moveCameraAction != null)
                EditorDrawUtility.DrawCameraAction(skillart.moveCameraAction);
            else
              if (GUILayout.Button("Add moveCameraAction 移动相机特效"))
            {
                skillart.moveCameraAction = new SkillCameraAction();
            }

            EditorGUILayout.LabelField("hitCameraAction 命中相机特效:");
            if (skillart.hitCameraAction != null)
                EditorDrawUtility.DrawCameraAction(skillart.hitCameraAction);
            else
           if (GUILayout.Button("Add hitCameraAction 命中相机特效"))
            {
                skillart.hitCameraAction = new SkillCameraAction();
            }

            EditorGUILayout.EndVertical();
        }
        private void DrawUnitList(float width)
        {
            //unit list
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("SkillUnit List");
            List<SkillUnit.SkillUnit> skillinits = EditorDataContainer.allSkillUnits.units;
            unitScrollPos = EditorGUILayout.BeginScrollView(unitScrollPos);
            //列表
            if (unitList == null)
            {
                // 加入数据数组
                unitList = new ReorderableList(skillinits, typeof(SkillUnit.SkillUnit), false, false, true, true);
            }
            // 绘制Item显示列表
            unitList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SkillUnit.SkillUnit element = skillinits[index];
                Rect drawRect = new Rect(rect.x, rect.y + 20, width-30, 600);
                GUILayout.BeginArea(drawRect);
                GUILayout.Label("index: " + index);
                DrawSkillUnit(DataConvert.ConvertSkillUnit(element));
                GUILayout.EndArea();
                EditorGUILayout.Separator();
            };
            unitList.elementHeight = 600;
            // 绘制表头
            unitList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "SkillUnitList");
            };
            // 选择回调
            unitList.onSelectCallback = (ReorderableList l) =>
            {
                SelectSkillUnit = l.index;
            };
            unitList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        private void DrawSkillArtList()
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("SkillArtEffect List");
            LoadFbx();
            LoadEffect();
            // 设置保存文件名字
            List<SkillUnit.SkillArt> skillart = EditorDataContainer.allSkillUnits.arts;
            artScrollPos = EditorGUILayout.BeginScrollView(artScrollPos, GUILayout.MaxWidth(350));
            //列表
            if (artList == null)
            {
                // 加入数据数组
                artList = new ReorderableList(skillart, typeof(SkillUnit.SkillArt), false, false, true, true);
            }
            // 绘制Item显示列表
            artList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                float titlewid = 100;
                SkillUnit.SkillArt element = skillart[index];
                rect.y += 2;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, titlewid, EditorGUIUtility.singleLineHeight), "Id:" + " " + element.id);
                EditorGUI.TextField(new Rect(rect.x + titlewid + 5, rect.y, rect.width - titlewid, EditorGUIUtility.singleLineHeight), "" + EditorDataContainer.GetSkillStringById(element.id));
                EditorGUI.LabelField(new Rect(rect.x, rect.y + 20, titlewid, EditorGUIUtility.singleLineHeight), "guideFadeTime:");
                element.guideFadeTime =EditorGUI.FloatField(new Rect(rect.x + titlewid + 5, rect.y + 20, rect.width - titlewid, EditorGUIUtility.singleLineHeight), element.guideFadeTime);

                EditorGUI.LabelField(new Rect(rect.x, rect.y + 40, titlewid, EditorGUIUtility.singleLineHeight), "guideAction:");
                EditorGUI.TextField(new Rect(rect.x + titlewid + 5, rect.y + 40, rect.width - titlewid, EditorGUIUtility.singleLineHeight), element.guideAction);

                EditorGUI.LabelField(new Rect(rect.x, rect.y + 60, titlewid, EditorGUIUtility.singleLineHeight), "guidingAction:");
                EditorGUI.TextField(new Rect(rect.x + titlewid + 5, rect.y + 60, rect.width - titlewid, EditorGUIUtility.singleLineHeight), element.guidingAction);

                EditorGUI.LabelField(new Rect(rect.x, rect.y + 80, titlewid, EditorGUIUtility.singleLineHeight), "endAction:");
                EditorGUI.TextField(new Rect(rect.x + titlewid + 5, rect.y + 80, rect.width - titlewid, EditorGUIUtility.singleLineHeight), element.endAction);

                EditorGUI.LabelField(new Rect(rect.x, rect.y + 100, titlewid, EditorGUIUtility.singleLineHeight), "unitEffect:");
                EditorGUI.TextField(new Rect(rect.x + titlewid + 5, rect.y + 100, rect.width - titlewid, EditorGUIUtility.singleLineHeight), "" + EditorDataContainer.GetStringById(element.unitEffect.effect));
                EditorGUI.LabelField(new Rect(rect.x, rect.y + 120, titlewid, EditorGUIUtility.singleLineHeight), "untEftphaseTime:");
                EditorGUI.TextField(new Rect(rect.x + titlewid + 5, rect.y + 120, rect.width - titlewid, EditorGUIUtility.singleLineHeight), "" + element.unitEffect.phaseTime);
                EditorGUI.LabelField(new Rect(rect.x, rect.y + 140, titlewid, EditorGUIUtility.singleLineHeight), "untEftheight:");
                EditorGUI.TextField(new Rect(rect.x + titlewid + 5, rect.y + 140, rect.width - titlewid, EditorGUIUtility.singleLineHeight), "" + element.unitEffect.height);
                EditorGUI.LabelField(new Rect(rect.x, rect.y + 160, titlewid, EditorGUIUtility.singleLineHeight), "untEfteffPos:");
                EditorGUI.TextField(new Rect(rect.x + titlewid + 5, rect.y + 160, rect.width - titlewid, EditorGUIUtility.singleLineHeight), "" + Enum.GetName(typeof(SkillUnit.SkillArtEffect.EffPos), element.unitEffect.effPos));

                for (int i = 0; i < element.beginEffect.Count; i++)
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + 180 + i * 80, titlewid, EditorGUIUtility.singleLineHeight), "beginEffect:" + i);
                    EditorGUI.TextField(new Rect(rect.x + titlewid + 5, rect.y + 180 + i * 80, rect.width - titlewid, EditorGUIUtility.singleLineHeight), "" + EditorDataContainer.GetStringById(element.beginEffect[i].effect));
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + 180 + i * 80 + 20, titlewid, EditorGUIUtility.singleLineHeight), "phaseTime:" + i);
                    EditorGUI.TextField(new Rect(rect.x + titlewid + 5, rect.y + 180 + i * 80 + 20, rect.width - titlewid, EditorGUIUtility.singleLineHeight), "" + element.beginEffect[i].phaseTime);
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + 180 + i * 80 + 40, titlewid, EditorGUIUtility.singleLineHeight), "height:" + i);
                    EditorGUI.TextField(new Rect(rect.x + titlewid + 5, rect.y + 180 + i * 80 + 40, rect.width - titlewid, EditorGUIUtility.singleLineHeight), "" + element.beginEffect[i].height);
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + 180 + i * 80 + 60, titlewid, EditorGUIUtility.singleLineHeight), "effPos:" + i);
                    EditorGUI.TextField(new Rect(rect.x + titlewid + 5, rect.y + 180 + i * 80 + 60, rect.width - titlewid, EditorGUIUtility.singleLineHeight), "" + Enum.GetName(typeof(SkillUnit.SkillArtEffect.EffPos), element.beginEffect[i].effPos));

                }
                EditorGUILayout.Separator();
                artList.elementHeight = 190 + element.beginEffect.Count * 80;
            };
            // 绘制表头
            artList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "SkillArtList");
            };
            // 选择回调
            artList.onSelectCallback = (ReorderableList l) =>
            {
                SelectSkillArt = l.index;
            };
            artList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        //绘制技能信息区域
        public void DrawSkillUnit(JSkillUnit unit)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Skill unit: 技能信息");
            unit.id = GetSkillIdByString(EditorGUILayout.TextField("  id " + unit.id, "" + GetSkillStringById(unit.id)));
            unit.artId = GetSkillIdByString(EditorGUILayout.TextField("  artId " + unit.artId, "" + GetSkillStringById(unit.artId)));
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
            unit.cd = EditorGUILayout.IntField("  cd:",  unit.cd);
            unit.distance = EditorGUILayout.FloatField("  distance:",  unit.distance);
            unit.referId = EditorGUILayout.IntField("  referId:", unit.referId);
            EditorGUILayout.LabelField("弹道信息:");
            if (unit.skillObj != null)
            {
                if (unit.launchType == JSkillUnit.LaunchType.SINGLELINE)
                {
                    if (!(unit.skillObj is SkillLine))
                    {
                        unit.skillObj = new SkillLine();
                    }
                    EditorDrawUtility.DrawSkillSingleLine((SkillLine)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.MULLINE)
                {
                    if (!(unit.skillObj is SkillMultiLine))
                    {
                        unit.skillObj = new SkillMultiLine();
                    }
                    EditorDrawUtility.DrawSkillMultiLine((SkillMultiLine)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.AREA)
                {
                    if (!(unit.skillObj is SkillArea))
                    {
                        unit.skillObj = new SkillArea();
                    }
                    EditorDrawUtility.DrawSkillArea((SkillArea)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.JUMP)
                {
                    if (!(unit.skillObj is SkillJump))
                    {
                        unit.skillObj = new SkillJump();
                    }
                    EditorDrawUtility.DrawSkillJump((SkillJump)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.AREA_RANDSKILL)
                {
                    if (!(unit.skillObj is SkillAreaRand))
                    {
                        unit.skillObj = new SkillAreaRand();
                    }
                    EditorDrawUtility.DrawSkillAreaRandom((SkillAreaRand)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.HELIX)
                {
                    if (!(unit.skillObj is SkillHelix))
                    {
                        unit.skillObj = new SkillHelix();
                    }
                    EditorDrawUtility.DrawSkillHelix((SkillHelix)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.FOLLOW)
                {
                    if (!(unit.skillObj is SkillFollow))
                    {
                        unit.skillObj = new SkillFollow();
                    }
                    EditorDrawUtility.DrawSkillFollow((SkillFollow)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.BACK_STAB)
                {
                    if (!(unit.skillObj is SkillBackStab))
                    {
                        unit.skillObj = new SkillBackStab();
                    }
                    EditorDrawUtility.DrawSkillBackStab((SkillBackStab)unit.skillObj);
                }
            }
            else
            {
                if (GUILayout.Button("Add New Unit:添加弹道"))
                {
                    unit.skillObj = new SkillObj();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawButtons()
        {
            //使用选择的技能参数
            if (GUILayout.Button(new GUIContent("Use Choose Unit", "使用选择的技能Unit"),GUILayout.Height(40)))
            {
                DataConvert.ConvertSkillUnit(EditorDataContainer.currentskillAssetData.skillUnit,EditorDataContainer.allSkillUnits.units[SelectSkillUnit]);
                int curart = EditorDataContainer.currentskillAssetData.skillUnit.artId;
                SkillUnit.SkillArt oart = EditorDataContainer.allSkillUnits.arts[curart];
                DataConvert.ConvertSkillArt(EditorDataContainer.currentskillAssetData.skillArt, oart);
                EditorDataContainer.UseSkillArt(oart);
               
            }
            //使用选择的技能参数
            if (GUILayout.Button(new GUIContent("Use Choose Skill Art", "使用选择的技能数据填充当前技能数据"), GUILayout.Height(40)))
            {
                SkillUnit.SkillArt oart = EditorDataContainer.allSkillUnits.arts[SelectSkillArt];
                EditorDataContainer.UseSkillArt(oart);
            }
            if (GUILayout.Button(new GUIContent("Generate Sequence", "生成序列"), GUILayout.Height(40)))
            {
                EditorDataContainer.GenerateSequence(EditorDataContainer.currentskillAssetData);
                Close();
            }
            if (GUILayout.Button(new GUIContent("Generate SkillData", "由当前序列生成技能数据对象"), GUILayout.Height(40)))
            {
                if (CurrentSequence != null)
                {
                    EditorDataContainer.currentskillAssetData = EditorDataContainer.MakeSkillAssetData(CurrentSequence);
                     Reset();
                }
            }

            if (GUILayout.Button("Save",GUILayout.Height(40)))
            {
                EditorDataContainer.SaveSkillAssetData();
            }
            if (GUILayout.Button("Load",GUILayout.Height(40)))
            {
                EditorDataContainer.LoadSkillAssetData();
                Reset();
                Repaint();
               
            }
            if (GUILayout.Button(showUnitList ? "HideSkillUnitList" : "ShowSkillUnitList", GUILayout.Height(40)))
            {
                showUnitList = !showUnitList;
            }
            if (GUILayout.Button(showArtList ? "HideSkillArtList" : "ShowSkillArtList", GUILayout.Height(40)))
            {
                showArtList = !showArtList;
            }
            if (GUILayout.Button("Close", GUILayout.Height(40)))
            {
                Close();
            }
        }

        private void OnEnable()
        {
            EditorDataContainer.LoadSystemTables();

        }
        private void Reset()
        {
            unitList = null;
            artList = null;
            allFbx = null;
            beginEffectList = null;
            stateNamelist = null;
        }

        private void OnDestroy()
        {
            Reset();
        }

        Rect unitRect = new Rect();
        Rect artRect = new Rect();
        Rect skillunitRect = new Rect();
        Rect skillartRect = new Rect();
        Rect buttonrect = new Rect();
        private void OnGUI()
        {
            LoadFbx();
            LoadEffect();
            EditorGUILayout.BeginHorizontal();
            if (showUnitList)
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Box("UnitList", USEditorUtility.ContentBackground, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                if (UnityEngine.Event.current.type == EventType.Repaint)
                {
                    if (unitRect != GUILayoutUtility.GetLastRect())
                    {
                        unitRect = GUILayoutUtility.GetLastRect();
                        this.Repaint();
                    }
                }
                EditorGUILayout.EndVertical();
            }

            if (showArtList)
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Box("ArtList", USEditorUtility.ContentBackground, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                if (UnityEngine.Event.current.type == EventType.Repaint)
                {
                    if (artRect != GUILayoutUtility.GetLastRect())
                    {
                        artRect = GUILayoutUtility.GetLastRect();
                        this.Repaint();
                    }
                }
                EditorGUILayout.EndVertical();

            }

            EditorGUILayout.BeginVertical();
            GUILayout.Box("SkillUnit", USEditorUtility.ContentBackground, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                if (skillunitRect != GUILayoutUtility.GetLastRect())
                {
                    skillunitRect = GUILayoutUtility.GetLastRect();
                    this.Repaint();
                }
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical();
            GUILayout.Box("SkillArt", USEditorUtility.ContentBackground, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                if (skillartRect != GUILayoutUtility.GetLastRect())
                {
                    skillartRect = GUILayoutUtility.GetLastRect();
                    this.Repaint();
                }
            }
            EditorGUILayout.EndVertical();

            //EditorGUILayout.BeginVertical();
            GUILayout.Box("Buttons", USEditorUtility.ContentBackground, GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                if (buttonrect != GUILayoutUtility.GetLastRect())
                {
                    buttonrect = GUILayoutUtility.GetLastRect();
                    this.Repaint();
                }
            }
           // EditorGUILayout.EndVertical();

         
            EditorGUILayout.EndHorizontal();

            if (showUnitList)
            {
                GUILayout.BeginArea(unitRect);
                DrawUnitList(unitRect.width);
                GUILayout.EndArea();
            }

            if (showArtList)
            {
                GUILayout.BeginArea(artRect);
                DrawSkillArtList();
                GUILayout.EndArea();
            }

            GUILayout.BeginArea(skillunitRect);
            DrawSkillUnit(EditorDataContainer.currentskillAssetData.skillUnit);
            GUILayout.EndArea();

            GUILayout.BeginArea(skillartRect);
            DrawSkillArtEffect(EditorDataContainer.currentskillAssetData.skillArt, skillartRect.width);
            GUILayout.EndArea();

            GUILayout.BeginArea(buttonrect);
            DrawButtons();
            GUILayout.EndArea();
        }
    }
}
