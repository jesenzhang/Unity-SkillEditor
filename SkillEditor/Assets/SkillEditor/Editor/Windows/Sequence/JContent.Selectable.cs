using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;
namespace CySkillEditor
{
    public partial class JContent : ScriptableObject
    {
        [SerializeField]
        private List<UnityEngine.Object> selectedObjects = new List<UnityEngine.Object>();
        public List<UnityEngine.Object> SelectedObjects
        {
            get { return selectedObjects; }
            set { selectedObjects = value; }
        }

        public void ResetSelection()
        {
            if (SelectedObjects != null && SelectedObjects.Count > 0)
            {
                USEditorUtility.RemoveFromUnitySelection(SelectedObjects);
                SelectedObjects.Clear();
                SourcePositions.Clear();
            }
        }

        public void OnSelectedObjects(List<UnityEngine.Object> selectedObjects)
        {
            foreach (var selectedObject in selectedObjects)
            {
                if (!SelectedObjects.Contains(selectedObject))
                {
                    SelectedObjects.Add(selectedObject);
                    var selection = Selection.objects != null ? Selection.objects.ToList() : new List<UnityEngine.Object>();
                    selection.Add(selectedObject);
                    Selection.objects = selection.ToArray();
                }
            }
        }
        public void OnDeSelectedObjects(List<UnityEngine.Object> selectedObjects)
        {
            foreach (var selectedObject in selectedObjects)
            {
                SelectedObjects.Remove(selectedObject);
                var selection = Selection.objects != null ? Selection.objects.ToList() : new List<UnityEngine.Object>();
                selection.Remove(selectedObject);
                Selection.objects = selection.ToArray();
            }
        }
        public void DeleteSelection()
        {
            foreach (var selectedObject in SelectedObjects)
            {
                JClipRenderData clip = selectedObject as JClipRenderData;
                RemoveClip(clip);
            }
            SelectedObjects.Clear();
        }
        #region ExtensionRegion
        public void OnSingleClipSelected(JClipRenderData selectobj)
        {
            OnSingleAnimationSelected(selectobj);
            OnSingleParticleSelected(selectobj);
            OnSingleSoundSelected(selectobj);
            OnSingleTransformSelected(selectobj);
            OnSingleEventSelected(selectobj);
            OnSingleTrajectorySelected(selectobj);
            OnSingleCameraSelected(selectobj);
            OnSingleEffectSelected(selectobj);
        }
        #endregion

        #region ExtensionRegion
        public void StartDraggingObjects()
        {
            foreach (var selectedObject in selectedObjects)
            {
                var clipData = selectedObject as JClipRenderData;
                StartDraggingAnimationClip(clipData);
                StartDraggingParticleClip(clipData);
                StartDraggingSoundClip(clipData);
                StartDraggingTransformClip(clipData);
                StartDraggingEventClip(clipData);
                StartDraggingTrajectoryClip(clipData);
                StartDraggingCameraClip(clipData);
                StartDraggingEffectClip(clipData);
            }
        }
        #endregion
        #region ExtensionRegion
        public void ProcessDraggingObjects(Vector2 mouseDelta)
        {
            foreach (var selectedObject in SelectedObjects)
            {
                var clip = selectedObject as JClipRenderData;
                ProcessDraggingAnimationClip(clip, mouseDelta);
                ProcessDraggingParticleClip(clip, mouseDelta);
                ProcessDraggingSoundClip(clip, mouseDelta);
                ProcessDraggingTransformClip(clip, mouseDelta);
                ProcessDraggingEventClip(clip, mouseDelta);
                ProcessDraggingTrajectoryClip(clip, mouseDelta);
                ProcessDraggingCameraClip(clip, mouseDelta);
                ProcessDraggingEffectClip(clip, mouseDelta);
            }
        }
        #endregion
    }
}
