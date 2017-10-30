using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace CySkillEditor
{
    [CustomEditor(typeof(JTimelineContainer))]
    public class JTimelineContainerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var typeindex = serializedObject.FindProperty("modelType");
            var affectedObject = serializedObject.FindProperty("affectedObject");
            var affectedObjectPath = serializedObject.FindProperty("affectedObjectPath");
            var index = serializedObject.FindProperty("index");

            EditorGUILayout.PropertyField(affectedObject);

            int newtypeindex = EditorGUILayout.Popup("ModelType:", (int)typeindex.intValue, Enum.GetNames(typeof(ModelTargetType)));
            if (newtypeindex != (int)typeindex.intValue)
            {
                serializedObject.FindProperty("modelType").intValue = newtypeindex;
            }
            List<GameObject> allFbx = AssetUtility.GetAllFBXWithType((ModelTargetType)serializedObject.FindProperty("modelType").intValue);
            List<string> fbxNamelist = new List<string>();
            for (int i = 0; i < allFbx.Count; i++)
            {
                fbxNamelist.Add(allFbx[i].name);
            }
            int fbxselet = -1;
            if (affectedObject.objectReferenceValue != null)
            {
                if (fbxNamelist.Contains(affectedObject.objectReferenceValue.name))
                {
                    fbxselet = fbxNamelist.IndexOf(affectedObject.objectReferenceValue.name);
                }
            }
            int newselectfbx = EditorGUILayout.Popup(fbxselet, fbxNamelist.ToArray());
            if (newselectfbx != fbxselet)
            {
                Transform parent = ((Transform)serializedObject.FindProperty("affectedObject").objectReferenceValue).parent;
                GameObject.DestroyImmediate(((Transform)serializedObject.FindProperty("affectedObject").objectReferenceValue).gameObject);
                GameObject newmodel = GameObject.Instantiate<GameObject>(allFbx[newselectfbx]);
                newmodel.transform.SetParent(parent);
                newmodel.name = allFbx[newselectfbx].name;
                serializedObject.FindProperty("affectedObject").objectReferenceValue = newmodel.transform;
            }
            EditorGUILayout.PropertyField(affectedObjectPath);
            EditorGUILayout.PropertyField(index);
            if (serializedObject.ApplyModifiedProperties())
            {
                JWindow[] windows = Resources.FindObjectsOfTypeAll<JWindow>();
                foreach (var window in windows)
                    window.Repaint();
            }
        }
    }
}