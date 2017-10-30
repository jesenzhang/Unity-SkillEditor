using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace CySkillEditor
{
    [Serializable]
    public class JWindow : EditorWindow
    {

        public static Vector2 minWindowSize = new Vector2(750.0f, 250.0f);

        [SerializeField]
        private static JWindow thisWindow = null;

        [MenuItem("Window/Memory Used")]
        public static void MemoryUsed()
        {
            //Texture target = Selection.activeObject as Texture;
            //Debug.Log("内存占用：" + EditorUtility.FormatBytes(UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(Selection.activeObject)));
            //Debug.Log("硬盘占用：" + EditorUtility.FormatBytes((int)UnityEditor.TextureUtil.GetStorageMemorySizeLong(target)));
        }

        [MenuItem("Window/JSequencer/Open JSequencer")]
        static void OpenJSequencerWindow()
        {
            var window = GetWindow<JWindow>();
            window.Show();
        }

        [MenuItem("Window/JSequencer/Close JSequencer")]
        static void CloseJSequencerWindow()
        {
            var window = GetWindow<JWindow>();
            window.Close();
        }


        [SerializeField]
        private JSequencer currentSequence;
        public JSequencer CurrentSequence
        {
            get { return currentSequence; }
            private set
            {
                currentSequence = value;
                if (currentSequence)
                    currentSequence.ResetCachedData();
            }
        }
        [SerializeField]
        private JContent contentRenderer;
        private JContent ContentRenderer
        {
            get { return contentRenderer; }
            set { contentRenderer = value; }
        }
        [SerializeField]
        private float PreviousTime
        {
            get;
            set;
        }

        private Rect TopBar
        {
            get;
            set;
        }

        private Rect Content
        {
            get;
            set;
        }

        private Rect BottomBar
        {
            get;
            set;
        }
        //是否在拖动
        public static bool IsScrubbing
        {
            get { return thisWindow.ContentRenderer.ScrubHandleDrag; }
            set {; }
        }

        private double showAnimationModeTime
        {
            get;
            set;
        }
        private void OnEnable()
        {
            showAnimationModeTime = EditorApplication.timeSinceStartup;
            thisWindow = this;
            hideFlags = HideFlags.HideAndDontSave;

            if (ContentRenderer == null)
                ContentRenderer = ScriptableObject.CreateInstance<JContent>();
            ContentRenderer.SequenceWindow = this;

            EditorApplication.hierarchyWindowChanged -= OnHierarchyChanged;
            EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
            SceneView.onSceneGUIDelegate -= OnScene;
            SceneView.onSceneGUIDelegate += OnScene;
            Undo.undoRedoPerformed -= UndoRedoCallback;
            Undo.undoRedoPerformed += UndoRedoCallback;
            EditorApplication.playmodeStateChanged -= PlayModeStateChanged;
            EditorApplication.playmodeStateChanged += PlayModeStateChanged;
            EditorApplication.update -= SequenceUpdate;
            EditorApplication.update += SequenceUpdate;

        }



        private void OnDestroy()
        {
            thisWindow = null;
            EditorApplication.hierarchyWindowChanged -= OnHierarchyChanged;
            SceneView.onSceneGUIDelegate -= OnScene;
            Undo.undoRedoPerformed -= UndoRedoCallback;
            EditorApplication.playmodeStateChanged -= PlayModeStateChanged;
            EditorApplication.update -= SequenceUpdate;

            StopProcessingAnimationMode();

            if (CurrentSequence)
                CurrentSequence.Stop();
        }

        private void UndoRedoCallback()
        {
            // This hack ensures that we process and update our in progress Sequence when Undo / Redoing
            if (CurrentSequence)
            {
                var previousRunningTime = CurrentSequence.RunningTime;

                // if we undo we always revert to our base state.
                ContentRenderer.RestoreBaseState();

                // if our running time is then > 0.0f, we process the timeline
                if (previousRunningTime > 0.0f)
                {
                    CurrentSequence.RunningTime = previousRunningTime;
                }
            }

            Repaint();
        }

        private void PlayModeStateChanged()
        {
            StopProcessingAnimationMode();
        }

        private void SaveObjectValueInAnimationMode(Transform parent, ref List<Component> listObjs)
        {
            if (parent && parent.gameObject)
            {
                Component[] componentList = parent.gameObject.GetComponents<Component>();
                for (int i = 0; i < componentList.Length; i++)
                    listObjs.Add(componentList[i]);

                foreach (Transform transform in parent)
                    SaveObjectValueInAnimationMode(transform, ref listObjs);
            }
        }

        private void StartProcessingAnimationMode()
        {
            if (Application.isPlaying)
                return;
            List<Component> objects = new List<Component>();

            foreach (Transform observedObject in CurrentSequence.ObservedObjects)
                SaveObjectValueInAnimationMode(observedObject, ref objects);

            ContentRenderer.StoreBaseState();
        }

        private void StopProcessingAnimationMode()
        {

            if (CurrentSequence)
                CurrentSequence.Stop();

            if (Application.isPlaying)
                return;

            ContentRenderer.RestoreBaseState();
        }

        private void PlayOrPause()
        {
            if (!CurrentSequence)
                return;

            if (CurrentSequence.IsPlaying)
            {
                CurrentSequence.Pause();
            }
            else
            {
                StartProcessingAnimationMode();
                CurrentSequence.Play();
            }
        }

        private void Stop()
        {
            if (!CurrentSequence)
                return;
            CurrentSequence.Stop();
            StopProcessingAnimationMode();
        }
        private void ProcessHotkeys()
        {
            if (UnityEngine.Event.current.rawType == EventType.KeyDown && (UnityEngine.Event.current.keyCode == KeyCode.Backspace || UnityEngine.Event.current.keyCode == KeyCode.Delete))
            {
                if (contentRenderer.SelectedObjects.Count > 0)
                {
                    contentRenderer.DeleteSelection();
                }
                UnityEngine.Event.current.Use();
            }

        }
        private static void OnScene(SceneView sceneview)
        {
            if (thisWindow != null)
                thisWindow.OnSceneGUI();
        }
        private void OnSceneGUI()
        {
            ContentRenderer.OnSceneGUI();
        }

        private void OnGUI()
        {
            if (!CurrentSequence)
                ShowNotification(new GUIContent("Select a Sequence Or Create New One"));
            if (CurrentSequence && CurrentSequence.TimelineContainerCount < 1)
                ShowNotification(new GUIContent("Drag A Object To Sequence Or add a New Container"));

            if (CurrentSequence && CurrentSequence.TimelineContainerCount >= 1 && EditorApplication.timeSinceStartup - showAnimationModeTime > 3000)
                RemoveNotification();

            GUILayout.BeginVertical();
            {
                DisplayTopBar();
                DisplayBottomBar();
                DisplayEdittableArea();
            }
            GUILayout.EndVertical();


            ProcessHotkeys();

            if (UnityEngine.Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                UnityEngine.Event.current.Use();
            }

            if (UnityEngine.Event.current.type == EventType.DragPerform)
            {
                foreach (UnityEngine.Object dragObject in DragAndDrop.objectReferences)
                {
                    GameObject GO = dragObject as GameObject;
                    if (CurrentSequence)
                    {
                        if (GO != CurrentSequence.gameObject)
                        {
                            DragAndDrop.AcceptDrag();
                            List<JTimelineBase> list = CurrentSequence.CreateContainers(GO.transform);
                            foreach (var line in list)
                            {
                                ContentRenderer.AddNewTimeLineForRender(line);
                            }
                        }
                    }
                }
                UnityEngine.Event.current.Use();

            }
        }

        private void DisplayTopBar()
        {
            float space = 16.0f;
            GUILayout.Box("", EditorStyles.toolbar, GUILayout.ExpandWidth(true), GUILayout.Height(18.0f));

            if (UnityEngine.Event.current.type == EventType.Repaint)
                TopBar = GUILayoutUtility.GetLastRect();

            GUILayout.BeginArea(TopBar);
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Create New Sequence", EditorStyles.toolbarButton))
                    {
                        GameObject newSequence = new GameObject("Sequence");
                        JSequencer sequence = newSequence.AddComponent<JSequencer>();
                        //USUtility.CreateAndAttachObserver(sequence);

                        if (CurrentSequence == null)
                        {
                            Selection.activeGameObject = newSequence;
                            Selection.activeTransform = newSequence.transform;
                            SequenceSwitch(sequence);
                        }

                        Repaint();
                    }

                    string currentSequence = CurrentSequence != null ? CurrentSequence.name : "";
                    string label = "Select a Sequence";
                    if (CurrentSequence != null)
                        label = String.Format("Editting : {0}", currentSequence);
                    if (GUILayout.Button(label, EditorStyles.toolbarButton, GUILayout.Width(150.0f)))
                    {
                        GenericMenu menu = new GenericMenu();
                        JSequencer[] sequences = FindObjectsOfType(typeof(JSequencer)) as JSequencer[];
                        var orderedSequences = sequences.OrderBy(sequence => sequence.name);
                        foreach (JSequencer sequence in orderedSequences)
                            menu.AddItem(new GUIContent(sequence.name),
                                         currentSequence == sequence.name ? true : false,
                                         (obj) => Selection.activeGameObject = (GameObject)obj,
                                         sequence.gameObject);
                        menu.ShowAsContext();
                    }

                    GUILayout.Space(space);
                    GUILayout.Box("", USEditorUtility.SeperatorStyle, GUILayout.Height(18.0f));
                    GUILayout.Space(space);

                    if (CurrentSequence != null)
                    {
                        if (GUILayout.Button(new GUIContent(!CurrentSequence.IsPlaying ? USEditorUtility.PlayButton : USEditorUtility.PauseButton, "Toggle Play Mode (P)"), USEditorUtility.ToolbarButtonSmall))
                            PlayOrPause();
                        if (GUILayout.Button(USEditorUtility.StopButton, USEditorUtility.ToolbarButtonSmall))
                            Stop();

                        GUILayout.Space(space);
                        GUILayout.Box("", USEditorUtility.SeperatorStyle, GUILayout.Height(18.0f));
                        GUILayout.Space(space);
                    }
                    if (GUILayout.Button("CreateNewSkill", EditorStyles.toolbarButton))
                    {
                        Debug.Log("创建新技能");
                        var window = GetWindow<SetModelWindow>();
                        window.Show();

                    }
                    if (GUILayout.Button("Configure", EditorStyles.toolbarButton))
                    {
                        Debug.Log("配置面板");
                        var window = GetWindow<ConfigureWindow>();
                        window.CurrentSequence = CurrentSequence;
                        window.Show();

                    }
                    if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                    {
                        EditorDataContainer.SaveSkillAssetData();
                    }
                    if (GUILayout.Button("Load", EditorStyles.toolbarButton))
                    {
                        EditorDataContainer.LoadSkillAssetData();
                    }
                  
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }

        private void DisplayBottomBar()
        {
            float space = 16.0f;
            GUILayout.Box("", EditorStyles.toolbar, GUILayout.ExpandWidth(true), GUILayout.Height(20.0f));

            if (UnityEngine.Event.current.type == EventType.Repaint)
                BottomBar = GUILayoutUtility.GetLastRect();

            if (CurrentSequence == null)
                return;
            GUILayout.BeginArea(BottomBar);
            {
                GUILayout.BeginHorizontal();
                {
                    string[] showAllOptions = { "Show All", "Show Only Animated" };
                    int selectedShowAll = 0;
                    selectedShowAll = EditorGUILayout.Popup(selectedShowAll, showAllOptions, EditorStyles.toolbarPopup, GUILayout.MaxWidth(120.0f));

                    //开始检查是否有任何界面元素变化
                    EditorGUI.BeginChangeCheck();
                    string[] playBackOptions = { "Playback : Normal", "Playback : Looping", "Playback : PingPong" };
                    int selectedAnimationType = 0;
                    if (CurrentSequence.IsLopping)
                        selectedAnimationType = 1;
                    else if (CurrentSequence.IsPingPonging)
                        selectedAnimationType = 2;
                    selectedAnimationType = EditorGUILayout.Popup(selectedAnimationType, playBackOptions, EditorStyles.toolbarPopup, GUILayout.MaxWidth(120.0f));
                    if (EditorGUI.EndChangeCheck())
                    {
                        CurrentSequence.IsLopping = false;
                        CurrentSequence.IsPingPonging = false;
                        if (selectedAnimationType == 1)
                        {
                            CurrentSequence.IsLopping = true;
                            CurrentSequence.IsPingPonging = false;
                        }
                        else if (selectedAnimationType == 2)
                        {
                            CurrentSequence.IsLopping = false;
                            CurrentSequence.IsPingPonging = true;
                        }
                    }

                    GUILayout.Space(space);
                    GUILayout.Box("", USEditorUtility.SeperatorStyle, GUILayout.Height(18.0f));
                    GUILayout.Space(space);

                    EditorGUILayout.LabelField("", "Running Time", GUILayout.MaxWidth(100.0f));

                    //开始检查是否有任何界面元素变化
                    EditorGUI.BeginChangeCheck();
                    float runningTime = EditorGUILayout.FloatField("", CurrentSequence.RunningTime, GUILayout.MaxWidth(50.0f));
                    if (EditorGUI.EndChangeCheck())
                    {
                        SetRunningTime(runningTime);
                    }
                    GUILayout.Space(space);
                    GUILayout.Box("", USEditorUtility.SeperatorStyle, GUILayout.Height(18.0f));
                    GUILayout.Space(space);

                    EditorGUILayout.LabelField("", "Duration", GUILayout.MaxWidth(50.0f));
                    EditorGUI.BeginChangeCheck();
                    CurrentSequence.Duration = EditorGUILayout.FloatField("", CurrentSequence.Duration, GUILayout.MaxWidth(50.0f));
                    if (EditorGUI.EndChangeCheck())
                    {
                        ContentRenderer.UpdateCachedMarkerInformation();
                    }

                    GUILayout.Space(space);
                    GUILayout.Box("", USEditorUtility.SeperatorStyle, GUILayout.Height(18.0f));
                    GUILayout.Space(space);

                    EditorGUILayout.LabelField("", "PlaybackRate", GUILayout.MaxWidth(80.0f));
                    EditorGUI.BeginChangeCheck();
                    float playbackRate = EditorGUILayout.FloatField("", CurrentSequence.PlaybackRate, GUILayout.MaxWidth(50.0f));
                    if (EditorGUI.EndChangeCheck())
                    {
                        CurrentSequence.PlaybackRate = playbackRate;
                        ContentRenderer.UpdateCachedMarkerInformation();
                    }
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }

        private void DisplayEdittableArea()
        {
            if (CurrentSequence)
                contentRenderer.OnGUI();
            else
                GUILayout.Box("", USEditorUtility.ContentBackground, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }

        public void SetRunningTime(float newRunningTime)
        {
            StartProcessingAnimationMode();
            if (!CurrentSequence.IsPlaying)
                CurrentSequence.Play();
            CurrentSequence.Pause();
            CurrentSequence.RunningTime = newRunningTime;
        }

        private void SequenceUpdate()
        {

            if (CurrentSequence)
            {
                float currentTime = Time.realtimeSinceStartup;
                float deltaTime = currentTime - PreviousTime;
                if (Mathf.Abs(deltaTime) > JSequencer.SequenceUpdateRate)
                {
                    if (CurrentSequence.IsPlaying && !Application.isPlaying)
                    {
                        //CurrentSequence.UpdateSequencer(deltaTime * Time.timeScale);
                        Repaint();
                    }
                    PreviousTime = currentTime;
                }
            }
            JSequencer nextSequence = null;

            if (Selection.activeGameObject != null && (CurrentSequence == null || Selection.activeGameObject != CurrentSequence.gameObject))
            {
                nextSequence = Selection.activeGameObject.GetComponent<JSequencer>();
                if (nextSequence != null)
                {
                    bool isPrefab = PrefabUtility.GetPrefabParent(nextSequence.gameObject) == null && PrefabUtility.GetPrefabObject(nextSequence.gameObject) != null;
                    if (isPrefab)
                        nextSequence = null;
                }
            }
            else
            {
                return;
            }

            if (nextSequence == null)
                return;

            if (!Application.isPlaying && CurrentSequence != nextSequence)
            {
                if (CurrentSequence)
                    CurrentSequence.Stop();

                if (nextSequence)
                    nextSequence.Stop();

                StopProcessingAnimationMode();
            }

            SequenceSwitch(nextSequence);

            Repaint();
        }

        private void OnHierarchyChanged()
        {
            if (Application.isPlaying)
                return;
            if (CurrentSequence)
                CurrentSequence.ResetCachedData();
        }
        private void SequenceSwitch(JSequencer nextSequence)
        {
            CurrentSequence = nextSequence;
            ContentRenderer.OnSequenceChange(CurrentSequence);
            //TryToFixPropertyTimelines(CurrentSequence);
            //TryToFixObserverTimelines(CurrentSequence);
        }
    }
}