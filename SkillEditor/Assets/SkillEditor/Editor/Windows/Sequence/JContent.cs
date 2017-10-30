using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace CySkillEditor
{
    [Serializable]
    public partial class JContent : ScriptableObject
    {
        private List<JTimelineMarkerCachedData> cachedMarkerData = new List<JTimelineMarkerCachedData>();

        /// <summary>
        ///  key 时间线 value 渲染元素数组 包含所有可见clip
        /// </summary>
        private Dictionary<UnityEngine.Object, List<JClipRenderData>> timelineClipRenderDataMap = new Dictionary<UnityEngine.Object, List<JClipRenderData>>();

        private Dictionary<JClipRenderData, float> sourcePositions = new Dictionary<JClipRenderData, float>();
        private Dictionary<JClipRenderData, float> SourcePositions
        {
            get { return sourcePositions; }
            set { sourcePositions = value; }
        }

        private float totalPixelWidthOfTimeline = 1.0f;
        private bool ZoomInvert = false;
        private float ZoomFactor = 0.01f;
        private float FloatingWidth = 100.0f;
        private bool extendingFloatingWidth = false;
        private float additionalFloatingWidth = 0.0f;
        private float lineHeight = 18.0f;
        [SerializeField]
        private float baseFloatingWidth = 250.0f;
        private float BaseFloatingWidth
        {
            get { return baseFloatingWidth; }
            set { baseFloatingWidth = value; }
        }

        List<List<JTimelineBase>> TypeList;

        //注册显示的数组
        List<TimeLineType> showTypeList = Configure.showTypeList;

        //目录fold字典
        private Dictionary<int, bool> foldStateDict;
        public Dictionary<int, bool> FoldStateDict
        {
            get
            {
                if (foldStateDict == null || foldStateDict.Keys.Count != CurrentSequence.TimelineContainerCount + CurrentSequence.TimelineContainerCount * showTypeList.Count)
                {

                    foldStateDict = new Dictionary<int, bool>();
                    for (int i = 0; i < CurrentSequence.TimelineContainerCount; i++)
                    {
                        foldStateDict.Add(i + 1, true);
                        for (int j = 0; j < showTypeList.Count; j++)
                        {
                            TimeLineType type = showTypeList[j];
                            int index = (int)type;
                            foldStateDict.Add((i + 1) * 100 + index, true);
                        }
                    }
                }

                return foldStateDict;
            }
            set { foldStateDict = value; }
        }


        bool hasObjectsUnderMouse = false;
        private float XScale
        {
            get
            {
                return (totalPixelWidthOfTimeline / ScrubArea.width);
            }
        }
        private float XScroll
        {
            get
            {
                return ScrollInfo.currentScroll.x;
            }
        }

        private float YScroll
        {
            get
            {
                return ScrollInfo.currentScroll.y;
            }
        }

        private Vector2 scrollPos = Vector2.zero;
        public Vector2 ScrollPos
        {
            get { return scrollPos; }
            set { scrollPos = value; }
        }

        [SerializeField]
        private JSequencer currentSequence;
        private JSequencer CurrentSequence
        {
            get { return currentSequence; }
            set
            {
                currentSequence = value;
                InitializeRenderMapWithSequence();
                SequenceWindow.Repaint();
            }
        }

        [SerializeField]
        private JZoomInfo zoomInfo;
        private JZoomInfo ZoomInfo
        {
            get { return zoomInfo; }
            set { zoomInfo = value; }
        }

        [SerializeField]
        private JScrollInfo scrollInfo;
        private JScrollInfo ScrollInfo
        {
            get { return scrollInfo; }
            set { scrollInfo = value; }
        }

        [SerializeField]
        private JWindow sequenceWindow;
        public JWindow SequenceWindow
        {
            get { return sequenceWindow; }
            set { sequenceWindow = value; }
        }

        [SerializeField]
        private bool snap;
        public bool Snap
        {
            get { return snap; }
            set { snap = value; }
        }

        public float SnapAmount
        {
            get { return EditorPrefs.GetFloat("WellFired-uSequencer-SnapAmout", 1.0f); }
            set { EditorPrefs.SetFloat("WellFired-uSequencer-SnapAmout", value); }
        }

        private Rect FloatingArea
        {
            get;
            set;
        }

        private Rect ScrubArea
        {
            get;
            set;
        }

        private Rect HierarchyArea
        {
            get;
            set;

        }
        private Rect VisibleArea
        {
            get;
            set;
        }

        private Rect LabelArea
        {
            get;
            set;
        }
        private Rect TotalArea
        {
            get;
            set;
        }

        private Rect HorizontalScrollArea
        {
            get;
            set;
        }

        private Rect VerticalScrollArea
        {
            get;
            set;
        }
        private Rect DisplayArea
        {
            get;
            set;
        }

        [SerializeField]
        private Texture cachedDragTexture;
        private Texture DragAreaTexture
        {
            get
            {
                if (!cachedDragTexture)
                    cachedDragTexture = Resources.Load("DragAreaTexture", typeof(Texture)) as Texture;
                return cachedDragTexture;
            }
            set {; }
        }

        [SerializeField]
        private Rect SelectableArea
        {
            get;
            set;
        }

        [SerializeField]
        private Rect SelectionArea
        {
            get;
            set;
        }

        [SerializeField]
        private bool IsDragging
        {
            get;
            set;
        }

        [SerializeField]
        private bool HasStartedDrag
        {
            get;
            set;
        }

        [SerializeField]
        private bool HasProcessedInitialDrag
        {
            get;
            set;
        }

        [SerializeField]
        private bool IsBoxSelecting
        {
            get;
            set;
        }

        [SerializeField]
        private bool IsDuplicating
        {
            get;
            set;
        }

        [SerializeField]
        private bool HasDuplicated
        {
            get;
            set;
        }

        [SerializeField]
        private Vector2 DragStartPosition
        {
            get;
            set;
        }

        [SerializeField]
        public bool ScrubHandleDrag
        {
            get;
            private set;
        }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;

            if (ScrollInfo == null)
            {
                ScrollInfo = ScriptableObject.CreateInstance<JScrollInfo>();
                ScrollInfo.Reset();
            }
            if (ZoomInfo == null)
            {
                ZoomInfo = ScriptableObject.CreateInstance<JZoomInfo>();
                ZoomInfo.Reset();
            }
            if (currentSequence)
                UpdateCachedMarkerInformation();
            if (currentSequence)
                InitializeRenderMapWithSequence();
        }
        int CountClip = 0;
        public void InitializeRenderMapWithSequence()
        {
            if (currentSequence)
            {
                timelineClipRenderDataMap.Clear();
                JTimelineBase[] timelines = currentSequence.SortedTimelines;
                //遍历时间线容器
                for (int i = 0; i < timelines.Length; i++)
                {
                    #region ExtensionRegion
                    //处理动画时间线
                    AddRenderDataForAnimation(timelines[i]);
                    AddRenderDataForParticle(timelines[i]);
                    AddRenderDataForSound(timelines[i]);
                    AddRenderDataForTransform(timelines[i]);
                    AddRenderDataForEvent(timelines[i]);
                    AddRenderDataForTrajectory(timelines[i]);
                    AddRenderDataForCamera(timelines[i]);
                    AddRenderDataForEffect(timelines[i]);
                    #endregion
                }
            }
        }
        /// <summary>
        /// 添加新的时间线渲染数据
        /// </summary>
        /// <param name="line"></param>
        public void AddNewTimeLineForRender(JTimelineBase line)
        {
            if (CurrentSequence)
            {
                #region ExtensionRegion
                AddRenderDataForAnimation(line);
                AddRenderDataForParticle(line);
                AddRenderDataForSound(line);
                AddRenderDataForTransform(line);
                AddRenderDataForEvent(line);
                AddRenderDataForTrajectory(line);
                AddRenderDataForCamera(line);
                AddRenderDataForEffect(line);
                #endregion
            }
        }

        public void AddNewTrack(JTimelineBase line)
        {
            if (CurrentSequence)
            {
                #region ExtensionRegion
                AddNewAnimationTrack(line);
                AddNewSoundTrack(line);
                AddNewParticleTrack(line);
                AddNewTransFormTrack(line);
                AddNewEventTrack(line);
                AddNewTrajectoryTrack(line);
                AddNewCameraTrack(line);
                AddNewEffectTrack(line);
                #endregion
            }
        }
        /// <summary>
        /// 移除时间线的渲染数据
        /// </summary>
        /// <param name="line"></param>
        public void RemoveTimeLine(JTimelineBase line)
        {/*
        if (CurrentSequence)
        {
            if (line is JTimelineAnimation)
                RemoveAnimationTimeline((JTimelineAnimation)line);
            else
            if (line is JTimeLineParticle)
                RemoveParticleTimeline((JTimeLineParticle)line);
            if (line is JTimelineSound)
                RemoveSoundTimeline((JTimelineSound)line);

        }*/
        }
        private void OnDestroy()
        {

        }

        /// <summary>
        /// 基本布局
        /// </summary>
        private void LayoutAreas()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        {
                            //时间标尺背景
                            GUILayout.Box("Floating", USEditorUtility.ContentBackground, GUILayout.Width(FloatingWidth), GUILayout.Height(lineHeight));
                            if (UnityEngine.Event.current.type == EventType.Repaint)
                            {
                                if (FloatingArea != GUILayoutUtility.GetLastRect())
                                {
                                    FloatingArea = GUILayoutUtility.GetLastRect();
                                    SequenceWindow.Repaint();
                                }
                            }
                            //时间标尺
                            GUILayout.Box("Scrub", USEditorUtility.ContentBackground, GUILayout.ExpandWidth(true), GUILayout.Height(lineHeight));
                            if (UnityEngine.Event.current.type == EventType.Repaint)
                            {
                                if (ScrubArea != GUILayoutUtility.GetLastRect())
                                {
                                    ScrubArea = GUILayoutUtility.GetLastRect();
                                    SequenceWindow.Repaint();
                                    UpdateCachedMarkerInformation();
                                }
                            }
                        }
                        GUILayout.EndHorizontal();

                        //整个内容显示区域
                        GUILayout.Box("Hierarchy", USEditorUtility.ContentBackground, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        if (UnityEngine.Event.current.type == EventType.Repaint)
                        {
                            if (HierarchyArea != GUILayoutUtility.GetLastRect())
                            {
                                HierarchyArea = GUILayoutUtility.GetLastRect();

                                SequenceWindow.Repaint();
                                UpdateCachedMarkerInformation();
                            }
                        }
                    }
                    //垂直滚动条
                    GUILayout.EndVertical();

                    GUILayout.Box("Scroll", USEditorUtility.ContentBackground, GUILayout.Width(lineHeight), GUILayout.ExpandHeight(true));
                    if (UnityEngine.Event.current.type == EventType.Repaint)
                    {
                        if (VerticalScrollArea != GUILayoutUtility.GetLastRect())
                        {
                            VerticalScrollArea = GUILayoutUtility.GetLastRect();
                            SequenceWindow.Repaint();
                            UpdateCachedMarkerInformation();
                        }
                    }
                }

                GUILayout.EndHorizontal();
                //水平滚动条
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Box("Scroll", USEditorUtility.ContentBackground, GUILayout.ExpandWidth(true), GUILayout.Height(lineHeight));
                    if (UnityEngine.Event.current.type == EventType.Repaint)
                    {
                        if (HorizontalScrollArea != GUILayoutUtility.GetLastRect())
                        {
                            HorizontalScrollArea = GUILayoutUtility.GetLastRect();
                            SequenceWindow.Repaint();
                            UpdateCachedMarkerInformation();
                        }
                    }
                    //右下角的横纵滚动条的边角
                    GUILayout.Box("block bit", USEditorUtility.ContentBackground, GUILayout.Width(lineHeight), GUILayout.Height(lineHeight));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        public void OnGUI()
        {
            LayoutAreas();
            if (CurrentSequence == null)
                return;
            ///标签区域
            GUI.Box(FloatingArea, "", USEditorUtility.ContentBackground);
            //右边标尺区域
            GUI.Box(ScrubArea, "", USEditorUtility.ScrubBarBackground);

            float widthOfContent = ScrubArea.x + (CurrentSequence.Duration / ZoomInfo.meaningOfEveryMarker * ZoomInfo.currentXMarkerDist);
            //鼠标放在标尺上 滚轮缩放
            if (UnityEngine.Event.current.type == EventType.ScrollWheel && ScrubArea.Contains(UnityEngine.Event.current.mousePosition))
            {
                float zoom = (UnityEngine.Event.current.delta.y * ZoomFactor);
                if (UnityEngine.Event.current.control)
                    zoom *= 10.0f;

                ZoomInfo.currentZoom += (zoom * (ZoomInvert == true ? -1.0f : 1.0f));
                if (ZoomInfo.currentZoom <= 1.0f)
                    ZoomInfo.currentZoom = 1.0f;

                ScrollInfo.currentScroll.x += UnityEngine.Event.current.delta.x;
                ScrollInfo.currentScroll.x = Mathf.Clamp(ScrollInfo.currentScroll.x, 0, widthOfContent);
                UnityEngine.Event.current.Use();

                var newWidthOfContent = ScrubArea.x + (CurrentSequence.Duration / ZoomInfo.meaningOfEveryMarker * ZoomInfo.currentXMarkerDist);
                var woc = newWidthOfContent - FloatingWidth;
                var mxFromLeft = (UnityEngine.Event.current.mousePosition.x - FloatingWidth + ScrollInfo.currentScroll.x);
                var ratio = mxFromLeft / woc;
                var contentWidthDiff = (newWidthOfContent - widthOfContent) * ratio;
                ScrollInfo.currentScroll.x += contentWidthDiff;
                UpdateCachedMarkerInformation();
            }
            //绘制 时间标尺 当前时间刻度
            GUILayout.BeginArea(ScrubArea);
            {
                foreach (var cachedMarker in cachedMarkerData)
                {
                    GUI.DrawTexture(cachedMarker.MarkerRenderRect, USEditorUtility.TimelineMarker);
                    if (cachedMarker.MarkerRenderLabel != string.Empty)
                        GUI.Label(cachedMarker.MarkerRenderLabelRect, cachedMarker.MarkerRenderLabel);
                }

                // Render our scrub Handle
                float currentScrubPosition = TimeToContentX(CurrentSequence.RunningTime);

                float halfScrubHandleWidth = 5.0f;
                Rect scrubHandleRect = new Rect(currentScrubPosition - halfScrubHandleWidth, 0.0f, halfScrubHandleWidth * 2.0f, ScrubArea.height);
                GUI.color = new Color(1.0f, 0.1f, 0.1f, 0.65f);
                GUI.DrawTexture(scrubHandleRect, USEditorUtility.TimelineScrubHead);
                GUI.color = Color.white;

                // Render the running time here
                scrubHandleRect.x += scrubHandleRect.width;
                scrubHandleRect.width = 100.0f;
                GUI.Label(scrubHandleRect, CurrentSequence.RunningTime.ToString("#.####"));

                if (UnityEngine.Event.current.type == EventType.MouseDown)
                    ScrubHandleDrag = true;
                if (UnityEngine.Event.current.rawType == EventType.MouseUp)
                    ScrubHandleDrag = false;

                if (ScrubHandleDrag && UnityEngine.Event.current.isMouse)
                {
                    float mousePosOnTimeline = ContentXToTime(FloatingWidth + UnityEngine.Event.current.mousePosition.x);
                    sequenceWindow.SetRunningTime(mousePosOnTimeline);
                    UnityEngine.Event.current.Use();
                }

            }
            GUILayout.EndArea();

            UpdateGrabHandle();

            float height = TotalArea.height;
            if (TotalArea.height < HierarchyArea.height)
                height = HierarchyArea.height;
            float temp = ScrollInfo.currentScroll.x;
            ScrollInfo.currentScroll.y = GUI.VerticalScrollbar(VerticalScrollArea, ScrollInfo.currentScroll.y, HierarchyArea.height, 0.0f, height);
            ScrollInfo.currentScroll.x = GUI.HorizontalScrollbar(HorizontalScrollArea, ScrollInfo.currentScroll.x, FloatingWidth + HierarchyArea.width, 0.0f, widthOfContent);
            ScrollInfo.currentScroll.x = Mathf.Clamp(ScrollInfo.currentScroll.x, 0, widthOfContent);
            ScrollInfo.currentScroll.y = Mathf.Clamp(ScrollInfo.currentScroll.y, 0, TotalArea.height);
            if (temp != ScrollInfo.currentScroll.x)
            {
                UpdateCachedMarkerInformation();
            }
            ContentGUI();

            // Render our red line
            Rect scrubMarkerRect = new Rect(ScrubArea.x + TimeToContentX(CurrentSequence.RunningTime), HierarchyArea.y, 1.0f, HierarchyArea.height);

            // Don't render an offscreen scrub line.
            if (scrubMarkerRect.x < HierarchyArea.x)
                return;
            GUI.color = new Color(1.0f, 0.1f, 0.1f, 0.65f);
            GUI.DrawTexture(scrubMarkerRect, USEditorUtility.TimelineScrubTail);
            GUI.color = Color.white;
        }

        public void OnSceneGUI()
        {
            if (CurrentSequence)
            {
                JTimelineBase[] timelines = CurrentSequence.SortedTimelines;
                for (int i = 0; i < timelines.Length; i++)
                {
                    OnTransformSceneGUI(timelines[i]);
                }
            }

        }
        protected float ConvertTimeToXPos(float time)
        {
            float xPos = DisplayArea.width * (time / CurrentSequence.Duration);
            return DisplayArea.x + ((xPos * XScale) - XScroll);
        }
        /// <summary>
        /// 侧边栏渲染
        /// </summary>
        private void DrawSideBarAndTimeLines()
        {
            CountClip = 0;
            foreach (KeyValuePair<UnityEngine.Object, List<JClipRenderData>> kvp in timelineClipRenderDataMap)
            {
                foreach (var clip in kvp.Value)
                {
                    clip.index = CountClip++;
                }
            }

            TypeList = CurrentSequence.SortedTimelinesLists;
            JTimelineContainer[] containers = CurrentSequence.SortedTimelineContainers;

            for (int i = 0; i < containers.Length; i++)
            {
                GUILayout.BeginVertical();
                JTimelineContainer container = containers[i];
                GUILayout.BeginHorizontal();
                GUILayout.Box(new GUIContent("", ""), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.MaxWidth(FloatingWidth));
                Rect FloatingRect = GUILayoutUtility.GetLastRect();
                GUILayout.Box(new GUIContent("", ""), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));

                GUILayout.EndHorizontal();
                FloatingRect.width -=  lineHeight + 1;

                bool CurcontainerfoldState = false;
                if (FoldStateDict.ContainsKey(i + 1))
                {
                    Rect temp = FloatingRect;
                    temp.width = 20;
                    FoldStateDict[i + 1] = EditorGUI.Foldout(temp, FoldStateDict[i + 1], "");
                    CurcontainerfoldState = FoldStateDict[i + 1];
                    Rect temp1 = FloatingRect;
                    temp1.x += 20;
                  //  temp1.y -= 1;
                    temp1.width -= 20;
                  //  temp1.height -=2;
                    if (GUI.Button(temp1, new GUIContent(container.AffectedObject.name, "模型"), EditorStyles.toolbarButton))
                    {
                        ResetSelection();
                        Selection.activeGameObject = container.gameObject;
                    }

                    Rect menuBtn = FloatingRect;
                    menuBtn.x = menuBtn.x + menuBtn.width ;
                    menuBtn.width = lineHeight;
                    if (GUI.Button(menuBtn, new GUIContent("", USEditorUtility.DeleteButton, "Delete Timelines"),  USEditorUtility.ToolbarButtonSmall))
                    {
                        GameObject.DestroyImmediate(container.gameObject);
                    }
                }

                if (CurcontainerfoldState)
                {
                    foreach (TimeLineType type in showTypeList)
                    {
                        // TODO: 遍历操作
                        int index = (int)type;
                        GUILayout.BeginVertical();
                        GUILayout.BeginHorizontal();
                        GUILayout.Box(new GUIContent("", ""), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.MaxWidth(FloatingWidth));
                        Rect FloatingRect1 = GUILayoutUtility.GetLastRect();
                        GUILayout.Box(new GUIContent("", ""), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));

                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        FloatingRect1.x = +20;
                        FloatingRect1.width -= (20 + lineHeight + 1);
                        int foldkey = (i + 1) * 100 + index;
                        bool Curfoldstate = false;
                        if (FoldStateDict.ContainsKey(foldkey))
                        {
                            FoldStateDict[foldkey] = EditorGUI.Foldout(FloatingRect1, FoldStateDict[foldkey], Enum.GetName(typeof(TimeLineType), type));
                            Curfoldstate = FoldStateDict[foldkey];
                        }

                        Rect menuBtn = FloatingRect1;
                        menuBtn.x = menuBtn.x + menuBtn.width + 1.0f;
                        menuBtn.width = lineHeight;
                        if (GUI.Button(menuBtn, new GUIContent("", USEditorUtility.MoreButton, "Add Timeline"), USEditorUtility.ToolbarButtonSmall))
                        {
                            JTimelineBase line = container.AddNewTimeline(type);
                            if (line)
                                AddNewTrack(line);
                        }

                        if (Curfoldstate)
                        {
                            List<JTimelineBase> timelines = TypeList[index];
                            for (int k = 0; k < timelines.Count; k++)
                            {
                                GUILayout.BeginVertical();
                                JTimelineBase line = timelines[k];
                                if (line.TimelineContainer == container)
                                {
                                    #region ExtensionRegion
                                    SideBarAndLineForAnimation(line);
                                    SideBarAndLineForParticle(line);
                                    SideBarAndLineForSound(line);
                                    SideBarAndLineForTranform(line);
                                    SideBarAndLineForEvent(line);
                                    SideBarAndLineForTrajectory(line);
                                    SideBarAndLineForCamera(line);
                                    SideBarAndLineForEffect(line);
                                    #endregion
                                }
                                GUILayout.EndVertical();
                            }
                        }
                    }
                }
                GUILayout.EndVertical();
            }
        }
        private void UpdateGrabHandle()
        {
            FloatingWidth = additionalFloatingWidth + BaseFloatingWidth;
            Rect resizeHandle = new Rect(FloatingWidth - 10.0f, ScrubArea.y, 10.0f, ScrubArea.height);
            GUILayout.BeginArea(resizeHandle, "", "box");

            if (UnityEngine.Event.current.type == EventType.MouseDown && UnityEngine.Event.current.button == 0)
                extendingFloatingWidth = true;

            if (extendingFloatingWidth && UnityEngine.Event.current.type == EventType.MouseDrag)
            {
                additionalFloatingWidth += UnityEngine.Event.current.delta.x;
                FloatingWidth = additionalFloatingWidth + BaseFloatingWidth;
                UpdateCachedMarkerInformation();
                SequenceWindow.Repaint();
            }
            GUILayout.EndArea();

            if (UnityEngine.Event.current.type == EventType.MouseUp)
                extendingFloatingWidth = false;
        }
        private void ContentGUI()
        {
            GUILayout.BeginArea(HierarchyArea);
            {
                if (UnityEngine.Event.current.type == EventType.ScrollWheel)
                {
                    ScrollInfo.currentScroll.x += UnityEngine.Event.current.delta.x;
                    ScrollInfo.currentScroll.y += 2 * UnityEngine.Event.current.delta.y;
                    float widthOfContent = ScrubArea.x + (CurrentSequence.Duration / ZoomInfo.meaningOfEveryMarker * ZoomInfo.currentXMarkerDist);
                    ScrollInfo.currentScroll.x = Mathf.Clamp(ScrollInfo.currentScroll.x, 0, widthOfContent);
                    UpdateCachedMarkerInformation();
                    UnityEngine.Event.current.Use();
                }
                GUILayout.BeginVertical();

                GUILayout.Box("", USEditorUtility.ContentBackground, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                if (UnityEngine.Event.current.type == EventType.Repaint)
                {
                    if (VisibleArea != GUILayoutUtility.GetLastRect())
                    {
                        VisibleArea = GUILayoutUtility.GetLastRect();
                        SequenceWindow.Repaint();
                    }
                }
                GUILayout.BeginArea(VisibleArea);

                GUILayout.BeginScrollView(ScrollInfo.currentScroll, GUIStyle.none, GUIStyle.none);

                GUILayout.BeginVertical();

                DrawSideBarAndTimeLines();

                GUILayout.EndVertical();
                if (UnityEngine.Event.current.type == EventType.Repaint)
                {
                    if (TotalArea != GUILayoutUtility.GetLastRect())
                    {
                        TotalArea = GUILayoutUtility.GetLastRect();
                        SequenceWindow.Repaint();
                    }
                }
                GUILayout.EndScrollView();

                GUILayout.EndArea();

              //  SelectionArea = VisibleArea;
                if (VisibleArea.Contains(UnityEngine.Event.current.mousePosition) || UnityEngine.Event.current.rawType == EventType.MouseUp || UnityEngine.Event.current.rawType == EventType.MouseDrag)
                    HandleEvent(UnityEngine.Event.current.rawType == EventType.MouseUp ? UnityEngine.Event.current.rawType : UnityEngine.Event.current.type, UnityEngine.Event.current.button, UnityEngine.Event.current.mousePosition);

                // Render our mouse drag box.
                if (IsBoxSelecting && HasStartedDrag)
                {
                    Vector2 mousePosition = UnityEngine.Event.current.mousePosition;
                    Vector2 origin = DragStartPosition;
                    Vector2 destination = mousePosition;

                    if (mousePosition.x < DragStartPosition.x)
                    {
                        origin.x = mousePosition.x;
                        destination.x = DragStartPosition.x;
                    }

                    if (mousePosition.y < DragStartPosition.y)
                    {
                        origin.y = mousePosition.y;
                        destination.y = DragStartPosition.y;
                    }

                    Vector2 mouseDelta = destination - origin;
                    SelectionArea = new Rect(origin.x, origin.y, mouseDelta.x, mouseDelta.y);
                    if (!EditorGUIUtility.isProSkin)
                        GUI.Box(SelectionArea, "", USEditorUtility.USeqSkin.box);
                    else
                        GUI.Box(SelectionArea, "");

                    SequenceWindow.Repaint();
                }

            }
            GUILayout.EndVertical();
            GUILayout.EndArea();

        }
        public void AddNewTimelineContainer(JTimelineContainer timelineContainer)
        {
        }
        public void UpdateCachedMarkerInformation()
        {
            cachedMarkerData.Clear();

            // Update the area
            // The marker area has one big Marker per region and ten small markers per region, as we zoom in the small regions
            // become bigger and as we zoom in, they disappear.
            // A block from n -> n + 1 defines this time region.
            float currentZoomUpper = Mathf.Ceil(ZoomInfo.currentZoom);
            float currentZoomlower = Mathf.Floor(ZoomInfo.currentZoom);
            float zoomRatio = (ZoomInfo.currentZoom - currentZoomlower) / (currentZoomUpper / currentZoomUpper);

            float maxDuration = Mathf.Min(CurrentSequence.Duration, 600);
            float minXMarkerDist = ScrubArea.width / maxDuration;
            float maxXMarkerDist = minXMarkerDist * 2.0f;
            ZoomInfo.currentXMarkerDist = maxXMarkerDist * zoomRatio + minXMarkerDist * (1.0f - zoomRatio);

            float minXSmallMarkerHeight = ScrubArea.height * 0.1f;
            float maxXSmallMarkerHeight = ScrubArea.height * 0.8f;
            float currentXSmallMarkerHeight = maxXSmallMarkerHeight * zoomRatio + minXSmallMarkerHeight * (1.0f - zoomRatio);

            // Calculate our maximum zoom out.
            float maxNumberOfMarkersInPane = ScrubArea.width / minXMarkerDist;
            ZoomInfo.meaningOfEveryMarker = Mathf.Ceil(CurrentSequence.Duration / maxNumberOfMarkersInPane);

            int levelsDeep = Mathf.FloorToInt(ZoomInfo.currentZoom);
            while (levelsDeep > 1)
            {
                ZoomInfo.meaningOfEveryMarker *= 0.5f;
                levelsDeep -= 1;
            }

            totalPixelWidthOfTimeline = ZoomInfo.currentXMarkerDist * (CurrentSequence.Duration / ZoomInfo.meaningOfEveryMarker);

            // Calculate how much we can see, for our horizontal scroll bar, this is for clamping.
            ScrollInfo.visibleScroll.x = (ScrubArea.width / ZoomInfo.currentXMarkerDist) * ZoomInfo.currentXMarkerDist;
            if (ScrollInfo.visibleScroll.x > totalPixelWidthOfTimeline)
                ScrollInfo.visibleScroll.x = totalPixelWidthOfTimeline;

            // Create our markers
            //TimeToContentX(CurrentSequence.RunningTime)
            float markerValue = 0.0f;

            Rect markerRect = new Rect(-ScrollInfo.currentScroll.x, 0.0f, 1.0f, maxXSmallMarkerHeight);
            while (markerRect.x < ScrubArea.width)
            {
                if (markerValue > CurrentSequence.Duration)
                    break;

                // Big marker
                cachedMarkerData.Add(new JTimelineMarkerCachedData());

                cachedMarkerData[cachedMarkerData.Count - 1].MarkerRenderRect = markerRect;
                cachedMarkerData[cachedMarkerData.Count - 1].MarkerRenderLabelRect = new Rect(markerRect.x + 2.0f, markerRect.y, 40.0f, ScrubArea.height);
                cachedMarkerData[cachedMarkerData.Count - 1].MarkerRenderLabel = markerValue.ToString();

                // Small marker
                for (int n = 1; n <= 10; n++)
                {
                    float xSmallPos = markerRect.x + ZoomInfo.currentXMarkerDist / 10.0f * n;

                    Rect smallMarkerRect = markerRect;
                    smallMarkerRect.x = xSmallPos;
                    smallMarkerRect.height = minXSmallMarkerHeight;

                    if (n == 5)
                        smallMarkerRect.height = currentXSmallMarkerHeight;

                    cachedMarkerData.Add(new JTimelineMarkerCachedData());
                    cachedMarkerData[cachedMarkerData.Count - 1].MarkerRenderRect = smallMarkerRect;
                }

                markerRect.x += ZoomInfo.currentXMarkerDist;
                markerValue += ZoomInfo.meaningOfEveryMarker;
            }
        }

        public void OnSequenceChange(JSequencer newSequence)
        {
            CurrentSequence = newSequence;
            ZoomInfo.Reset();
            ScrollInfo.Reset();
            totalPixelWidthOfTimeline = 1.0f;
            UpdateCachedMarkerInformation();
            InitializeRenderMapWithSequence();
            SequenceWindow.Repaint();
        }

        public void HandleEvent(EventType eventType, int button, Vector2 mousePosition)
        {
            hasObjectsUnderMouse = false;

            List<UnityEngine.Object> allObjectsUnderMouse = new List<UnityEngine.Object>();

            foreach (KeyValuePair<UnityEngine.Object, List<JClipRenderData>> kvp in timelineClipRenderDataMap)
            {
                foreach (JClipRenderData renderclip in kvp.Value)
                {
                    if (IsBoxSelecting && HasStartedDrag)
                    {
                        Rect temp = SelectionArea;
                        temp.y += ScrollInfo.currentScroll.y;
                        if (USEditorUtility.DoRectsOverlap(temp, renderclip.renderRect))
                        {
                            allObjectsUnderMouse.Add(renderclip);
                        }
                    }
                    else
                    {
                        if (renderclip.renderRect.Contains(mousePosition))
                            allObjectsUnderMouse.Add(renderclip);
                    }
                }
            }
            if (allObjectsUnderMouse.Count > 0)
            {
                hasObjectsUnderMouse = true;
            }

            switch (eventType)
            {
                case EventType.MouseDown:
                    {
                        HasProcessedInitialDrag = false;
                        IsDragging = false;
                        IsBoxSelecting = false;
                        DragStartPosition = mousePosition;

                        if (!hasObjectsUnderMouse && UnityEngine.Event.current.button == 0)
                            IsBoxSelecting = true;
                        if (hasObjectsUnderMouse && UnityEngine.Event.current.button == 0)
                            IsDragging = true;
                        if (IsDragging && UnityEngine.Event.current.alt && UnityEngine.Event.current.control)
                            IsDuplicating = true;

                        // if we have no objects under our mouse, then we are likely trying to clear our selection
                        if (!hasObjectsUnderMouse && (!UnityEngine.Event.current.control && !UnityEngine.Event.current.command))
                        {
                            ResetSelection();
                        }

                        if (!UnityEngine.Event.current.control && !UnityEngine.Event.current.command)
                        {
                            Selection.activeGameObject = null;
                            Selection.activeObject = null;
                            Selection.activeTransform = null;
                            Selection.objects = new UnityEngine.Object[] { };
                        }

                        HasStartedDrag = false;
                        SequenceWindow.Repaint();
                    }
                    break;
                case EventType.MouseDrag:
                    {
                        if (!HasStartedDrag)
                            HasStartedDrag = true;

                        SequenceWindow.Repaint();
                    }
                    break;
                case EventType.MouseUp:
                    {
                        HasProcessedInitialDrag = false;
                        IsBoxSelecting = false;
                        IsDragging = false;
                        IsDuplicating = false;
                        HasDuplicated = false;
                        SequenceWindow.Repaint();
                    }
                    break;
            }

            //单选
            if ((!UnityEngine.Event.current.control && !UnityEngine.Event.current.command) && hasObjectsUnderMouse && !HasStartedDrag && ((eventType == EventType.MouseUp && button == 0) || (eventType == EventType.MouseDown && button == 1)))
            {

                EditorGUI.FocusTextInControl("");
                ResetSelection();
                OnSelectedObjects(allObjectsUnderMouse);

                if (SelectedObjects.Count == 1)
                {
                    //单选 处理更类型的属性显示
                    var selectobj = SelectedObjects[0] as JClipRenderData;
                    OnSingleClipSelected(selectobj);
                }
            }
            else
            //多选 添加和删除
            if ((UnityEngine.Event.current.control || UnityEngine.Event.current.command) && hasObjectsUnderMouse && !HasStartedDrag && eventType == EventType.MouseUp)
            {
                foreach (var selectedObject in allObjectsUnderMouse)
                {
                    if (!SelectedObjects.Contains(selectedObject))
                        OnSelectedObjects(new List<UnityEngine.Object> { selectedObject });
                    else
                        OnDeSelectedObjects(new List<UnityEngine.Object> { selectedObject });
                }
            }
            else if (IsBoxSelecting && HasStartedDrag)
            {
                OnSelectedObjects(allObjectsUnderMouse);
            }
            //移动
            if (IsDragging && HasStartedDrag)
            {
                if(allObjectsUnderMouse.Count==1)
                if (!selectedObjects.Contains(allObjectsUnderMouse[0]))
                {
                        ResetSelection();
                }
                OnSelectedObjects(allObjectsUnderMouse);
                DragStartPosition = new Vector2(DragStartPosition.x, DragStartPosition.y);
                Vector2 mouseDelta = UnityEngine.Event.current.mousePosition - DragStartPosition;

                if (!HasProcessedInitialDrag)
                {
                    StartDraggingObjects();
                    HasProcessedInitialDrag = true;
                }
                ProcessDraggingObjects(mouseDelta);
                if (IsDuplicating && !HasDuplicated)
                {

                }
                else
                {
                  ///  ProcessDraggingObjects(mouseDelta);
                }
            }

        }

        /// <summary>
        /// 移除可见渲染块
        /// </summary>
        /// <param name="clip"></param>
        private void RemoveClip(JClipRenderData clip)
        {
            RemoveAnimationClip(clip);
            RemoveParticleClip(clip);
            RemoveSoundClip(clip);
            RemoveKeyFrame(clip);
            RemoveTrajectoryClip(clip);
            RemoveEffectClip(clip);
        }
        private float TimeToContentX(float time)
        {
            return (time / ZoomInfo.meaningOfEveryMarker * ZoomInfo.currentXMarkerDist) - ScrollInfo.currentScroll.x;
        }

        private float ContentXToTime(float pos)
        {
            return (((pos + scrollInfo.currentScroll.x - ScrubArea.x) / ScrubArea.width) * CurrentSequence.Duration) / (totalPixelWidthOfTimeline / ScrubArea.width);
        }

        public void StoreBaseState()
        {

        }

        public void RestoreBaseState()
        {

        }

        public void ExternalModification()
        {

        }


    }
}
