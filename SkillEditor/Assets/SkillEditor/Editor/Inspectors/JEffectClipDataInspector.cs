
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

namespace CySkillEditor
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(JEffectClipData))]
    public class JEffectClipDataInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            JEffectClipData clip = (JEffectClipData)target;
            var serializedProperty = serializedObject.FindProperty("effectType");
            EditorGUILayout.PropertyField(serializedProperty);

            var startTime = serializedObject.FindProperty("startTime");
            EditorGUILayout.PropertyField(startTime);
            var playbackDuration = serializedObject.FindProperty("playbackDuration");
            EditorGUILayout.PropertyField(playbackDuration);
            var serializedlooping = serializedObject.FindProperty("looping");
            EditorGUILayout.PropertyField(serializedlooping);

            float starttime = 0.5f;
            float phasetime = 0.5f;

            if (clip.effectType == EffectType.Particle || clip.effectType == EffectType.Trajectory)
            {
                 starttime = clip.effectunit.artEffect.beginTime/1000f;
                 phasetime = clip.effectunit.artEffect.phaseTime / 1000f;
            }
            else
            if (clip.effectType == EffectType.Camera)
            {
                 starttime = clip.cameraAction.delay;
                 phasetime = clip.cameraAction.phaseTime;
            }

            bool apply = serializedObject.ApplyModifiedProperties();
            if (apply)
            {
                clip.PlaybackDuration = serializedObject.FindProperty("playbackDuration").floatValue;
                clip.StartTime = serializedObject.FindProperty("startTime").floatValue;
            }
            if (clip.effectType == EffectType.Particle)
            {
                EditorDrawUtility.DrawSkillEffectUnit(clip.effectunit);
                if (clip.effectunit.artEffect.beginTime / 1000f != starttime || clip.effectunit.artEffect.phaseTime / 1000f != phasetime)
                { 
                    clip.PlaybackDuration = (clip.effectunit.artEffect.phaseTime / 1000f);
                    clip.StartTime = (clip.effectunit.artEffect.beginTime / 1000f);
                    apply = true;
                }
            }
            if (clip.effectType == EffectType.Camera)
            {
                var TargetCamera = serializedObject.FindProperty("TargetCamera");
                EditorGUILayout.PropertyField(TargetCamera);
                EditorDrawUtility.DrawCameraAction(clip.cameraAction);
                if (clip.cameraAction.delay != starttime || clip.cameraAction.phaseTime != phasetime)
                {
                    clip.PlaybackDuration = clip.cameraAction.phaseTime;
                    clip.StartTime = clip.cameraAction.delay;
                    apply = true;
                }
            }
            if (clip.effectType == EffectType.Trajectory)
            {
                var Target = serializedObject.FindProperty("Target");
                EditorGUILayout.PropertyField(Target);
                var targetObject = serializedObject.FindProperty("targetObject");
                EditorGUILayout.PropertyField(targetObject);
                EditorDrawUtility.DrawSkillEffectUnit(clip.effectunit);
                EditorDrawUtility.DrawSkillUnit(clip.skillunit);
                if (clip.effectunit.artEffect.beginTime / 1000f != starttime || clip.effectunit.artEffect.phaseTime / 1000f != phasetime)
                {
                    clip.PlaybackDuration = clip.effectunit.artEffect.phaseTime / 1000f;
                    clip.StartTime = clip.effectunit.artEffect.beginTime / 1000f;
                    apply = true;
                }
            }
            if (clip.effectType == EffectType.Sound)
            {
                var sound = serializedObject.FindProperty("sound");
                EditorGUILayout.PropertyField(sound);
                if((AudioClip)sound.objectReferenceValue != null)
                    EditorGUILayout.FloatField("Duration",((AudioClip)sound.objectReferenceValue).length);
                var targetObject = serializedObject.FindProperty("targetObject");
                EditorGUILayout.PropertyField(targetObject);
                
                if(serializedObject.ApplyModifiedProperties())
                {
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.FindProperty("playbackDuration").floatValue = ((AudioClip)sound.objectReferenceValue).length;
                    apply = true;
                }
            }
            if (clip.effectType == EffectType.Animation)
            {
                var targetObject = serializedObject.FindProperty("targetObject");
                var animController = serializedObject.FindProperty("animController");
                var stateNameProperty = serializedObject.FindProperty("stateName");
                var layer = serializedObject.FindProperty("layer");
                
                GameObject targetObj = (GameObject)targetObject.objectReferenceValue;
                apply = EditorGUILayout.PropertyField(targetObject);
                RuntimeAnimatorController anim0 = (RuntimeAnimatorController)animController.objectReferenceValue;
                Animator anim = targetObj.GetComponent<Animator>();
                if (anim != null)
                {
                    if (anim0 != null)
                        anim.runtimeAnimatorController = anim0;
                    if (anim0 == null && anim.runtimeAnimatorController != null)
                        animController.objectReferenceValue = anim.runtimeAnimatorController;
                    apply = true;
                }
                EditorGUILayout.PropertyField(animController); 

                var availableLayerNames = MecanimAnimationUtility.GetAllLayerNames(targetObj);
                var olayer = EditorGUILayout.Popup("Layer", layer.intValue, availableLayerNames.ToArray());
                if (olayer != layer.intValue)
                    layer.intValue = olayer;
                var availableStateNames = MecanimAnimationUtility.GetAllStateNames(targetObj, layer.intValue);
                var existingState = -1;
                var existingStateName = stateNameProperty.stringValue;
                if (availableStateNames.Contains(existingStateName))
                {
                    existingState = availableStateNames.IndexOf(existingStateName);
                }
                var newState = EditorGUILayout.Popup("StateName", existingState, availableStateNames.ToArray());
                if (newState != existingState)
                {
                    stateNameProperty.stringValue = availableStateNames[newState];
                    float duration = MecanimAnimationUtility.GetStateDurationWithAnimatorController(stateNameProperty.stringValue, (RuntimeAnimatorController)animController.objectReferenceValue);
                    clip.PlaybackDuration = duration;
                    serializedObject.FindProperty("playbackDuration").floatValue = duration;
                    apply = true;
                }
                
            }
            if (apply)
            {
                JWindow[] windows = Resources.FindObjectsOfTypeAll<JWindow>();
                foreach (var window in windows)
                    window.Repaint();
            }
        }
    }
}