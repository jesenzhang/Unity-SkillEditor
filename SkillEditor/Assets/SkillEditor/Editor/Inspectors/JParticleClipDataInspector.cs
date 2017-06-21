using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

namespace CySkillEditor
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(JParticleClipData))]
    public class JParticleClipDataInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var serializedProperty = serializedObject.GetIterator();
            serializedProperty.Next(true);

            while (serializedProperty.NextVisible(true))
            {
                if (serializedProperty.name == "effectDuration")
                {
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(serializedProperty);
                    GUI.enabled = true;
                }
                else
                {
                    EditorGUILayout.PropertyField(serializedProperty);
                }
            }
            //if (serializedProperty.name == "effectConfig")
            {
                JParticleClipData ss = (JParticleClipData)target;
                CySkillEditor.EffectConfigure effect = ss.EffectConfig;
                EditorDrawUtility.DrawEffectConfigure(effect);
                
            }
            var particleTrack = serializedObject.FindProperty("track").objectReferenceValue as JParticleTrack;
            var targetObject = serializedObject.FindProperty("targetObject").objectReferenceValue;
            var targetGameObject = targetObject as GameObject;

            if (!particleTrack)
                return;

            if (!targetGameObject)
                return;


            if (serializedObject.ApplyModifiedProperties())
            {
                JWindow[] windows = Resources.FindObjectsOfTypeAll<JWindow>();
                foreach (var window in windows)
                    window.Repaint();
            }
        }
    }
}