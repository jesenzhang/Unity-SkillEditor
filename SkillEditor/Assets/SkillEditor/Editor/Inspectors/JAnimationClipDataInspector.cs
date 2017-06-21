using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace CySkillEditor
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(JAnimationClipData))]
    public class JAnimationClipDataInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var serializedProperty = serializedObject.GetIterator();
            serializedProperty.Next(true);

            while (serializedProperty.NextVisible(true))
            {
                if (serializedProperty.name == "stateDuration")
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
            var stateNameProperty = serializedObject.FindProperty("stateName");
            var animationTrack = serializedObject.FindProperty("track").objectReferenceValue as JAnimationTrack;
            var targetObject = serializedObject.FindProperty("targetObject").objectReferenceValue;
            var targetGameObject = targetObject as GameObject;

            if (!animationTrack)
                return;

            if (!targetGameObject)
                return;

            var availableStateNames = MecanimAnimationUtility.GetAllStateNames(targetGameObject, animationTrack.Layer);

            var existingState = -1;
            var existingStateName = stateNameProperty.stringValue;

            for (int clipIndex = 0; clipIndex < availableStateNames.Count; clipIndex++)
            {
                if (availableStateNames[clipIndex] == existingStateName)
                    existingState = clipIndex;
            }

            var newState = EditorGUILayout.Popup("Clip", existingState, availableStateNames.ToArray());

            if (newState != existingState)
                stateNameProperty.stringValue = availableStateNames[newState];

            if (serializedObject.ApplyModifiedProperties())
            {
                JWindow[] windows = Resources.FindObjectsOfTypeAll<JWindow>();
                foreach (var window in windows)
                    window.Repaint();
            }
        }
    }
}