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

        //uniteffect数组
        Vector2 tipEffectScrollPos = new Vector2(0, 0);
        private ReorderableList tipEffectList;

        //uniteffect数组
        Vector2 endEffectScrollPos = new Vector2(0, 0);
        private ReorderableList endEffectList;

        //uniteffect数组
        Vector2 hitEffectScrollPos = new Vector2(0, 0);
        private ReorderableList hitEffectList;


        //beginCameraList
        Vector2 beginCameraScrollPos = new Vector2(0, 0);
        private ReorderableList beginCameraList;

        //movingCameraList
        Vector2 movingCameraScrollPos = new Vector2(0, 0);
        private ReorderableList movingCameraList;

        //hitCameraList
        Vector2 hitCameraScrollPos = new Vector2(0, 0);
        private ReorderableList hitCameraList;


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

        bool showUnit = true;
        bool showBeginEffect = true;
        bool showTipEffect = true;
        bool showEndEffect = true;
        bool showHitEffect = true;
        bool showBeginCamera = true;
        bool showMoveCamera = true;
        bool showHitCamera = true;

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
            RuntimeAnimatorController controller0 = AssetUtility.GetAnimationCtl(skillart.modelType, skillart.modelName, skillart.animationController);
            if (skillart.animationControllerObj == null && controller0 != null)
                skillart.animationControllerObj = controller0;
            RuntimeAnimatorController controller = skillart.animationControllerObj;// AssetUtility.GetAnimationCtl(skillart.modelType, modelName, skillart.animationController);
            if (force && controller == null)
                Debug.Log("Error! Error animationController Not Found");
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
        //绘制特效信息
        public void DrawSkillArtEffect(SkillArt skillart,float width)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Skill ArtEffect: 特效");
            //id
            if (skillart.idString == "")
                skillart.idString = EditorDrawUtility.GetSkillStringById(skillart.id);
            string id = EditorGUILayout.TextField("Id:", skillart.idString);
            if (id != skillart.idString)
            {
                skillart.idString = id;
                skillart.id = EditorDrawUtility.GetSkillIdByString(id);
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
            EditorGUILayout.TextField("animationControllerName:", skillart.animationController);
            RuntimeAnimatorController controller = AssetUtility.GetAnimationCtl(skillart.modelType, skillart.modelName, skillart.animationController);
         
            if (skillart.animationControllerObj == null && controller != null)
                skillart.animationControllerObj = controller;

            RuntimeAnimatorController newcontroller = (RuntimeAnimatorController)EditorGUILayout.ObjectField("AnimatorController:", skillart.animationControllerObj, typeof(RuntimeAnimatorController), true);

            if (newcontroller != controller)
            {
                skillart.animationController = newcontroller.name;
                skillart.animationControllerObj = newcontroller;
                LoadAnimationState();
            }
          
            EditorGUILayout.LabelField("GuideAction 开始动作:");
            skillart.guideFadeTime = EditorGUILayout.FloatField("  guideFadeTime:", skillart.guideFadeTime);
            EditorDrawUtility.DrawActionRect("  guideAction:", skillart.guideAction, out skillart.guideAction, stateNamelist);
            EditorDrawUtility.DrawActionRect("  guidingAction:", skillart.guidingAction, out skillart.guidingAction, stateNamelist);
            EditorDrawUtility.DrawActionRect("  endAction:", skillart.endAction, out skillart.endAction, stateNamelist);

           
            showBeginEffect = EditorGUILayout.Foldout(showBeginEffect, "beginEffect 起手特效:");
            //EditorGUILayout.LabelField("beginEffect 起手特效:");
            if (showBeginEffect)
            {
                if (skillart.beginEffect != null)
                {
                    EditorDrawUtility.DrawEffectUnitList(skillart.beginEffect, beginScrollPos, out beginScrollPos, width, ref beginEffectList, effectNamelist, SkillEffectPrefabs);
                }
                else
                {
                    if (GUILayout.Button("Add beginEffectList"))
                    {
                        skillart.beginEffect = new List<SkillEffectUnit>();
                    }
                }
            }
            showEndEffect = EditorGUILayout.Foldout(showEndEffect, "endEffect 结束特效:");
            // EditorGUILayout.LabelField("tipEffect 警告特效:");
            if (showEndEffect)
            {
                if (skillart.endEffect != null)
                {
                    EditorDrawUtility.DrawEffectUnitList(skillart.endEffect, endEffectScrollPos, out endEffectScrollPos, width, ref endEffectList, effectNamelist, SkillEffectPrefabs);
                }
                else
                {
                    if (GUILayout.Button("Add endEffectList"))
                    {
                        skillart.endEffect = new List<SkillEffectUnit>();
                    }
                }
            }
            showUnit = EditorGUILayout.Foldout(showUnit, "unitEffect 弹道特效:");
            //EditorGUILayout.LabelField("unitEffect 弹道特效:");
            if (showUnit)
            {
                if (skillart.unitEffect != null)
                {
                    EditorDrawUtility.DrawEffectUnitList(skillart.unitEffect, unitEffectScrollPos, out unitEffectScrollPos, width, ref unitEffectList, effectNamelist, SkillEffectPrefabs);
                }
                else
                {
                    if (GUILayout.Button("Add unitEffectList"))
                    {
                        skillart.unitEffect = new List<SkillEffectUnit>();
                    }
                }
            }
           
            showTipEffect = EditorGUILayout.Foldout(showTipEffect, "tipEffect 警告特效:");
            // EditorGUILayout.LabelField("tipEffect 警告特效:");
            if (showTipEffect)
            {
                if (skillart.tipEffect != null)
                {
                    EditorDrawUtility.DrawEffectUnitList(skillart.tipEffect, tipEffectScrollPos, out tipEffectScrollPos, width, ref tipEffectList, effectNamelist, SkillEffectPrefabs);
                }
                else
                {
                    if (GUILayout.Button("Add tipEffectList"))
                    {
                        skillart.tipEffect = new List<SkillEffectUnit>();
                    }
                }
            }
            showHitEffect = EditorGUILayout.Foldout(showHitEffect, "hitEffect 击中特效:");
            // EditorGUILayout.LabelField("tipEffect 警告特效:");
            if (showHitEffect)
            {
                if (skillart.hitEffect != null)
                {
                    EditorDrawUtility.DrawEffectUnitList(skillart.hitEffect, hitEffectScrollPos, out hitEffectScrollPos, width, ref hitEffectList, effectNamelist, SkillEffectPrefabs);
                }
                else
                {
                    if (GUILayout.Button("Add hitEffectList"))
                    {
                        skillart.tipEffect = new List<SkillEffectUnit>();
                    }
                }
            }
            showBeginCamera = EditorGUILayout.Foldout(showBeginCamera, "beginCameraAction 开始相机特效:");

            //EditorGUILayout.LabelField("beginCameraAction 开始相机特效:");
            if (showBeginCamera)
            {
                if (skillart.beginCameraAction != null)
                {
                    EditorDrawUtility.DrawCameraActionList(skillart.beginCameraAction, beginCameraScrollPos, out beginCameraScrollPos, width, ref beginCameraList);
                }
                else
                {
                    if (GUILayout.Button("Add beginCameraActionList"))
                    {
                        skillart.beginCameraAction = new List<SkillCameraAction>();
                    }
                }
            }
            showMoveCamera = EditorGUILayout.Foldout(showMoveCamera, "moveCameraAction 移动相机特效:");
            if (showMoveCamera)
            {
                //EditorGUILayout.LabelField("moveCameraAction 移动相机特效:");
                if (skillart.moveCameraAction != null)
                {
                    EditorDrawUtility.DrawCameraActionList(skillart.moveCameraAction, movingCameraScrollPos, out movingCameraScrollPos, width, ref movingCameraList);
                }
                else
                {
                    if (GUILayout.Button("Add moveCameraActionList"))
                    {
                        skillart.moveCameraAction = new List<SkillCameraAction>();
                    }
                }
            }
            showHitCamera = EditorGUILayout.Foldout(showHitCamera, "hitCameraAction 命中相机特效:");
            if (showHitCamera)
            {
               // EditorGUILayout.LabelField("hitCameraAction 命中相机特效:");
                if (skillart.hitCameraAction != null)
                {
                    EditorDrawUtility.DrawCameraActionList(skillart.hitCameraAction, hitCameraScrollPos, out hitCameraScrollPos, width, ref hitCameraList);
                }
                else
                {
                    if (GUILayout.Button("Add hitCameraActionList"))
                    {
                        skillart.hitCameraAction = new List<SkillCameraAction>();
                    }
                }
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
                EditorDrawUtility.DrawSkillUnit(DataConvert.ConvertSkillUnit(element));
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
        private void DrawSkillArtList(float width)
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("SkillArtEffect List");
            LoadFbx();
            LoadEffect();
            // 设置保存文件名字
            List<SkillUnit.SkillArt> skillart = EditorDataContainer.allSkillUnits.arts;
            artScrollPos = EditorGUILayout.BeginScrollView(artScrollPos);
            //列表
            if (artList == null)
            {
                // 加入数据数组
                artList = new ReorderableList(skillart, typeof(SkillUnit.SkillArt), false, false, true, true);
            }
            // 绘制Item显示列表
            artList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SkillUnit.SkillArt element = skillart[index];
                Rect drawRect = new Rect(rect.x, rect.y + 20, width - 30, 1200);
                GUILayout.BeginArea(drawRect);
                EditorGUILayout.TextField("Id:"+element.id, EditorDataContainer.GetSkillStringById(element.id));
                EditorGUILayout.FloatField("guideFadeTime:" , element.guideFadeTime);
                EditorGUILayout.TextField("guideAction:", element.guideAction);
                EditorGUILayout.TextField("guidingAction:", element.guidingAction);
                EditorGUILayout.TextField("endAction:", element.endAction);
                
                for (int i = 0; i < element.beginEffect.Count; i++)
                {
                    if (element.beginEffect[i] != null)
                    {
                        EditorGUILayout.LabelField("beginEffect:" + i);
                        EditorDrawUtility.DrawSkillArtEffect(DataConvert.ConvertSkillArtEffect(element.beginEffect[i]), null, null);
                    }
                }
                if (element.unitEffect != null)
                {
                    EditorGUILayout.LabelField("unitEffect:");
                    EditorDrawUtility.DrawSkillArtEffect(DataConvert.ConvertSkillArtEffect(element.unitEffect), null, null);
                }
                if (element.tipEffect != null)
                {
                    EditorGUILayout.LabelField("tipEffect:");
                    EditorDrawUtility.DrawSkillArtEffect(DataConvert.ConvertSkillArtEffect(element.tipEffect), null, null);
                }
                if (element.hitEffect != null)
                {
                    EditorGUILayout.LabelField("hitEffect:");
                    EditorDrawUtility.DrawSkillArtEffect(DataConvert.ConvertSkillArtEffect(element.hitEffect), null, null);
                }
                if (element.beginCameraAction != null)
                {
                    EditorGUILayout.LabelField("beginCameraAction:");
                    EditorDrawUtility.DrawCameraAction(DataConvert.ConvertCameraAction(element.beginCameraAction));
                }
                if (element.moveCameraAction != null)
                {
                    EditorGUILayout.LabelField("moveCameraAction:");
                    EditorDrawUtility.DrawCameraAction(DataConvert.ConvertCameraAction(element.moveCameraAction));
                }
                if (element.hitCameraAction !=null)
                {
                    EditorGUILayout.LabelField("hitCameraAction:");
                    EditorDrawUtility.DrawCameraAction(DataConvert.ConvertCameraAction(element.hitCameraAction));
                }
                GUILayout.EndArea();
                EditorGUILayout.Separator();
                artList.elementHeight = 1200;// 190 + element.beginEffect.Count * 80;
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
        private void DrawButtons()
        {
            //使用选择的技能参数
            if (GUILayout.Button(new GUIContent("Use Choose Unit\n加载选择的技能数据", "使用选择的技能Unit"),GUILayout.Height(40)))
            {
                DataConvert.ConvertSkillUnit(EditorDataContainer.currentskillAssetData.skillUnit,EditorDataContainer.allSkillUnits.units[SelectSkillUnit]);
                int curart = EditorDataContainer.currentskillAssetData.skillUnit.artId;
                SkillUnit.SkillArt oart = EditorDataContainer.allSkillUnits.arts[curart];
                DataConvert.ConvertSkillArt(EditorDataContainer.currentskillAssetData.skillArt, oart);
                EditorDataContainer.UseSkillArt(oart);
               
            }
            //使用选择的技能参数
            if (GUILayout.Button(new GUIContent("Use Choose Skill Art\n加载选择的特效数据", "使用选择的技能数据填充当前技能数据"), GUILayout.Height(40)))
            {
                SkillUnit.SkillArt oart = EditorDataContainer.allSkillUnits.arts[SelectSkillArt];
                EditorDataContainer.UseSkillArt(oart);
            }
            if (GUILayout.Button(new GUIContent("Generate Sequence\n生成序列", "生成序列"), GUILayout.Height(40)))
            {
                EditorDataContainer.GenerateSequence(EditorDataContainer.currentskillAssetData);
                Close();
            }
            if (GUILayout.Button(new GUIContent("Generate SkillData\n生成技能数据", "由当前序列生成技能数据对象"), GUILayout.Height(40)))
            {
                if (CurrentSequence != null)
                {
                    EditorDataContainer.currentskillAssetData = EditorDataContainer.MakeSkillAssetData(CurrentSequence);
                    Debug.Log(EditorDataContainer.currentskillAssetData.skillArt.beginEffect.Count);
                    Reset();
                }
            }
            if (GUILayout.Button("Search\n搜索技能", GUILayout.Height(40)))
            {
                var window = GetWindow<SearchWindow>();
                window.Show();
            }
            if (GUILayout.Button("Save Asset\n保存技能为Asset",GUILayout.Height(40)))
            {
                EditorDataContainer.SaveSkillAssetData();
            }
            if (GUILayout.Button("Load Asset\n加载Asset技能文件",GUILayout.Height(40)))
            {
                EditorDataContainer.LoadSkillAssetData();
                Reset();
                Repaint();
            }
            if (GUILayout.Button("Save SystemTables\n保存技能到系统表", GUILayout.Height(40)))
            {
                EditorDataContainer.SaveSystemTables();
                EditorDataContainer.LoadSystemTables();
                Reset();
                Repaint();
            }
          
            if (GUILayout.Button(showUnitList ? "HideSkillUnitList\n隐藏系统技能表" : "ShowSkillUnitList\n显示系统技能表", GUILayout.Height(40)))
            {
                showUnitList = !showUnitList;
            }
            if (GUILayout.Button(showArtList ? "HideSkillArtList\n隐藏系统特效表" : "ShowSkillArtList\n显示系统特效表", GUILayout.Height(40)))
            {
                showArtList = !showArtList;
            }
            if (GUILayout.Button("Close\n关闭窗口", GUILayout.Height(40)))
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
                DrawSkillArtList(artRect.width);
                GUILayout.EndArea();
            }

            GUILayout.BeginArea(skillunitRect);
            EditorDrawUtility.DrawSkillUnit(EditorDataContainer.currentskillAssetData.skillUnit);
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
