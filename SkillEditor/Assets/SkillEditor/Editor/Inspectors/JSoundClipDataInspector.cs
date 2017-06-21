using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace CySkillEditor
{

    [CanEditMultipleObjects()]
    [CustomEditor(typeof(JSoundClipData))]
    public class JSoundClipDataInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var serializedProperty = serializedObject.GetIterator();
            serializedProperty.Next(true);
            while (serializedProperty.NextVisible(true))
            {
                if (serializedProperty.name == "soundDuration")
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
            var SoundTrack = serializedObject.FindProperty("track").objectReferenceValue as JSoundTrack;
            var targetObject = serializedObject.FindProperty("targetObject").objectReferenceValue;

            var targetGameObject = targetObject as GameObject;

            if (!SoundTrack)
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