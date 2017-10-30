using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

namespace CySkillEditor
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(JTrajectoryClipData))]
    public class JTrajectoryClipDataInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            JTrajectoryClipData clip = (JTrajectoryClipData)target;
            var serializedProperty = serializedObject.FindProperty("Target");
            EditorGUILayout.PropertyField(serializedProperty);

            var serializedStartTime = serializedObject.FindProperty("startTime");
            EditorGUILayout.PropertyField(serializedStartTime);
            var serializedplaybackDuration = serializedObject.FindProperty("playbackDuration");
            EditorGUILayout.PropertyField(serializedplaybackDuration);
            var serializedlooping = serializedObject.FindProperty("looping");
            EditorGUILayout.PropertyField(serializedlooping);
            
            EditorDrawUtility.DrawSkillEffectUnit(clip.effectunit);
            EditorDrawUtility.DrawSkillUnit(clip.skillunit);
            
            if (serializedObject.ApplyModifiedProperties())
            {
                JWindow[] windows = Resources.FindObjectsOfTypeAll<JWindow>();
                foreach (var window in windows)
                    window.Repaint();
            }
        }
    }
}