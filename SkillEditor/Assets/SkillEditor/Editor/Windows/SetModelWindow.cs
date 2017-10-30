using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CySkillEditor
{
    public class SetModelWindow : EditorWindow
    {
        public static Vector2 minWindowSize = new Vector2(750.0f, 250.0f);
        private GameObject m_PreviewInstance;
        private PreviewExampleInspector m_Editor;

        private void OnDestroy()
        {
            if (m_Editor != null)
            {
                DestroyImmediate(m_Editor);
            }
            m_Editor = null;
        }

 
        private void OnGUI()
        {
            if (m_Editor == null)
            {
                // 第一个参数这里暂时没关系，因为编辑器没有取目标对象
               m_Editor = (PreviewExampleInspector)Editor.CreateEditor(this, typeof(PreviewExampleInspector));
            }
            ModelTargetType modelType = EditorDataContainer.currentskillAssetData.skillArt.modelType;
            GameObject model = EditorDataContainer.currentskillAssetData.skillArt.model;
            
            EditorGUILayout.BeginVertical();

            int newtypeindex = EditorGUILayout.Popup("ModelType:", (int)modelType, Enum.GetNames(typeof(ModelTargetType)));
            if (newtypeindex != (int)modelType)
            {
                EditorDataContainer.currentskillAssetData.skillArt.modelType = (ModelTargetType)newtypeindex;
            }
            List<GameObject> allFbx = AssetUtility.GetAllFBXWithType((ModelTargetType)newtypeindex);
            List<string> fbxNamelist = new List<string>();
            for (int i = 0; i < allFbx.Count; i++)
            {
                fbxNamelist.Add(allFbx[i].name);
            }
            int fbxselet = -1;
            if (model != null)
            {
                if (fbxNamelist.Contains(model.name))
                {
                    fbxselet = fbxNamelist.IndexOf(model.name);
                }
            }
            int newselectfbx = EditorGUILayout.Popup(fbxselet, fbxNamelist.ToArray());
            if (newselectfbx != fbxselet)
            {
                EditorDataContainer.currentskillAssetData.skillArt.model = allFbx[newselectfbx];
            }
            GameObject NewFbx = (GameObject)EditorGUILayout.ObjectField("Model:", model, typeof(GameObject), true);
            if (NewFbx != model)
            {
                EditorDataContainer.currentskillAssetData.skillArt.model = NewFbx;
                EditorDataContainer.currentskillAssetData.skillArt.modelName = EditorDataContainer.currentskillAssetData.skillArt.model.name;
            }
            if (EditorDataContainer.currentskillAssetData.skillArt.model != null)
            {
                EditorDataContainer.currentskillAssetData.skillArt.modelName = EditorDataContainer.currentskillAssetData.skillArt.model.name;

                EditorDataContainer.currentskillAssetData.skillArt.animationController = EditorDataContainer.currentskillAssetData.skillArt.model.name + "_Anim";
                EditorGUILayout.TextField("animationControllerName", EditorDataContainer.currentskillAssetData.skillArt.animationController);
                RuntimeAnimatorController controller = AssetUtility.GetAnimationCtl(EditorDataContainer.currentskillAssetData.skillArt.modelType, EditorDataContainer.currentskillAssetData.skillArt.modelName, EditorDataContainer.currentskillAssetData.skillArt.animationController);

                if (EditorDataContainer.currentskillAssetData.skillArt.animationControllerObj==null&&controller!=null)
                    EditorDataContainer.currentskillAssetData.skillArt.animationControllerObj = controller;

                RuntimeAnimatorController newcontroller = (RuntimeAnimatorController)EditorGUILayout.ObjectField("AnimatorController:", EditorDataContainer.currentskillAssetData.skillArt.animationControllerObj, typeof(RuntimeAnimatorController), true);
                if (newcontroller != controller)
                {
                    EditorDataContainer.currentskillAssetData.skillArt.animationController = newcontroller.name;
                    EditorDataContainer.currentskillAssetData.skillArt.animationControllerObj = newcontroller;
                }
            }
            if (GUILayout.Button("Create Empty Skill"))
            {
               EditorDataContainer.CreateEmptySkill("NewSkill", EditorDataContainer.currentskillAssetData.skillArt.model, EditorDataContainer.currentskillAssetData.skillArt.modelType, EditorDataContainer.currentskillAssetData.skillArt.animationControllerObj);
            }
            if (EditorDataContainer.currentskillAssetData.skillArt.model != null)
            {
                //Texture pre = AssetPreview.GetAssetPreview(EditorDataContainer.currentskillAssetData.skillArt.model);
                GUILayout.Label("");
                Rect rect = GUILayoutUtility.GetLastRect();
                rect.width = position.width;
                rect.height = position.height - rect.y;
                m_Editor.model = EditorDataContainer.currentskillAssetData.skillArt.model;
                //   EditorGUI.DrawPreviewTexture(rect, pre);
                m_Editor.DrawPreview(rect);
            }
            EditorGUILayout.EndVertical();

      
        }
          }

    
}