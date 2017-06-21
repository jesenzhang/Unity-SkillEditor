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
        List<string> showproperty = new List<string>();
        public override void OnInspectorGUI()
        {
            JTrajectoryClipData clip = (JTrajectoryClipData)target;
            var serializedProperty = serializedObject.FindProperty("Target");
            EditorGUILayout.PropertyField(serializedProperty);

            clip.StartTime = EditorGUILayout.FloatField("StartTime:", clip.StartTime);
            clip.PlaybackDuration = EditorGUILayout.FloatField("PlaybackDuration:",clip.PlaybackDuration);
            clip.Looping = EditorGUILayout.Toggle("Looping:", clip.Looping);
            
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