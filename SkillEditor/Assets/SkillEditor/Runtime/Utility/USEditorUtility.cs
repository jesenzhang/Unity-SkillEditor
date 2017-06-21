using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
namespace CySkillEditor
{
    public static class USEditorUtility
    {
        public static string SelectSequence = "Select a Sequence in the Hierarchy or Hit Create New Sequence to begin.";
        public static string NoAnimatableObjects = "Drag an object from your hierarchy to start animating it.";
        public static string Compiling = "Editor is compiling, we'll be with you shortly";
        public static string AnimationWindowIsOpen = "uSequencer doesn't work whilst the Animation Window is open, close it before continuing.";
        public static string AddingNewAffectedObjectWhilstInAnimationMode = "You cannot add a new Affected Object whilst in Animation Mode (Press the Stop Button to exit Animation Mode)";


        [NonSerialized]
        static private GUISkin uSeqSkin = null;
        static public GUISkin USeqSkin
        {
            get
            {
                if (!uSeqSkin)
                {
                    string Skin = "USequencerProSkin";

                    if (!EditorGUIUtility.isProSkin)
                        Skin = "USequencerFreeSkin";

                    uSeqSkin = Resources.Load(Skin, typeof(GUISkin)) as GUISkin;
                }

                if (!uSeqSkin)
                    Debug.LogError("Couldn't find the uSequencer Skin, it is possible it has been moved from the resources folder");

                return uSeqSkin;
            }
        }

        private static GUIStyle toolbar;
        public static GUIStyle Toolbar
        {
            get
            {
                if (toolbar == null)
                    toolbar = new GUIStyle(EditorStyles.toolbarButton);
                toolbar.normal.background = null;
                return toolbar;
            }
            set {; }
        }

        private static GUIStyle contentBackground;
        public static GUIStyle ContentBackground
        {
            get
            {
                if (contentBackground == null)
                    contentBackground = USEditorUtility.USeqSkin.GetStyle("USContentBackground");
                return contentBackground;
            }
            set {; }
        }

        private static GUIStyle seperatorStyle;
        public static GUIStyle SeperatorStyle
        {
            get
            {
                if (seperatorStyle == null)
                    seperatorStyle = USEditorUtility.USeqSkin.GetStyle("Seperator");
                return seperatorStyle;
            }
            set {; }
        }

        private static Texture playButton;
        public static Texture PlayButton
        {
            get
            {
                if (playButton == null)
                    playButton = LoadTexture("Play Button");
                return playButton;
            }
            set {; }
        }

        private static Texture pauseButton;
        public static Texture PauseButton
        {
            get
            {
                if (pauseButton == null)
                    pauseButton = LoadTexture("Pause Button");
                return pauseButton;
            }
            set {; }
        }
        private static Texture editButton;
        public static Texture EditButton
        {
            get
            {
                if (editButton == null)
                    editButton = LoadTexture("EditButton");
                return editButton;
            }
            set {; }
        }


        private static Texture stopButton;
        public static Texture StopButton
        {
            get
            {
                if (stopButton == null)
                    stopButton = LoadTexture("Stop Button");
                return stopButton;
            }
            set {; }
        }

        private static Texture recordButton;
        public static Texture RecordButton
        {
            get
            {
                if (recordButton == null)
                    recordButton = LoadTexture("Record Button");
                return recordButton;
            }
            set {; }
        }

        private static Texture prevKeyframeButton;
        public static Texture PrevKeyframeButton
        {
            get
            {
                if (prevKeyframeButton == null)
                    prevKeyframeButton = LoadTexture("Prev Keyframe Button");
                return prevKeyframeButton;
            }
            set {; }
        }

        private static Texture nextKeyframeButton;
        public static Texture NextKeyframeButton
        {
            get
            {
                if (nextKeyframeButton == null)
                    nextKeyframeButton = LoadTexture("Next Keyframe Button");
                return nextKeyframeButton;
            }
            set {; }
        }

        private static GUIStyle toolbarButtonSmall;
        public static GUIStyle ToolbarButtonSmall
        {
            get
            {
                if (toolbarButtonSmall == null)
                    toolbarButtonSmall = USeqSkin.GetStyle("ToolbarButtonSmall");
                return toolbarButtonSmall;
            }
            set {; }
        }

        private static GUIStyle buttonNoOutline;
        public static GUIStyle ButtonNoOutline
        {
            get
            {
                if (buttonNoOutline == null)
                    buttonNoOutline = USeqSkin.GetStyle("ButtonNoOutline");
                return buttonNoOutline;
            }
            set {; }
        }

        private static Texture timelineMarker;
        public static Texture TimelineMarker
        {
            get
            {
                if (timelineMarker == null)
                    timelineMarker = Resources.Load("TimelineMarker") as Texture;
                return timelineMarker;
            }
            set {; }
        }

        private static Texture timelineScrubHead;
        public static Texture TimelineScrubHead
        {
            get
            {
                if (timelineScrubHead == null)
                    timelineScrubHead = Resources.Load("TimelineScrubHead") as Texture;
                return timelineScrubHead;
            }
            set {; }
        }

        private static Texture timelineScrubTail;
        public static Texture TimelineScrubTail
        {
            get
            {
                if (timelineScrubTail == null)
                    timelineScrubTail = Resources.Load("TimelineScrubTail") as Texture;
                return timelineScrubTail;
            }
            set {; }
        }

        private static Texture moreButton;
        public static Texture MoreButton
        {
            get
            {
                if (moreButton == null)
                    moreButton = LoadTexture("More Button") as Texture;
                return moreButton;
            }
            set {; }
        }
        private static Texture deleteButton;
        public static Texture DeleteButton
        {
            get
            {
                if (deleteButton == null)
                    deleteButton = LoadTexture("Delete Button") as Texture;
                return deleteButton;
            }
            set {; }
        }
        private static Texture favouriteButtonEmpty;
        public static Texture FavouriteButtonEmpty
        {
            get
            {
                if (favouriteButtonEmpty == null)
                    favouriteButtonEmpty = LoadTexture("PropertyButtonFavouriteEmpty") as Texture;
                return favouriteButtonEmpty;
            }
            set {; }
        }

        private static Texture favouriteButtonFill;
        public static Texture FavouriteButtonFill
        {
            get
            {
                if (favouriteButtonFill == null)
                    favouriteButtonFill = LoadTexture("PropertyButtonFavouriteFill") as Texture;
                return favouriteButtonFill;
            }
            set {; }
        }

        private static Texture shrinkButton;
        public static Texture ShrinkButton
        {
            get
            {
                if (shrinkButton == null)
                    shrinkButton = LoadTexture("TimelineButtonSmaller") as Texture;
                return shrinkButton;
            }
            set {; }
        }

        private static Texture growButton;
        public static Texture GrowButton
        {
            get
            {
                if (growButton == null)
                    growButton = LoadTexture("TimelineButtonBigger") as Texture;
                return growButton;
            }
            set {; }
        }

        private static GUIStyle scrubBarBackground;
        public static GUIStyle ScrubBarBackground
        {
            get
            {
                if (scrubBarBackground == null)
                    scrubBarBackground = USeqSkin.GetStyle("USScrubBarBackground");
                return scrubBarBackground;
            }
            set {; }
        }

        private static GUIStyle normalWhiteOutLineBG;
        public static GUIStyle NormalWhiteOutLineBG
        {
            get
            {
                if (normalWhiteOutLineBG == null)
                    normalWhiteOutLineBG = USeqSkin.GetStyle("NormalWhiteOutLineBG");
                return normalWhiteOutLineBG;
            }
            set {; }
        }
        private static GUIStyle normalWhiteBG;
        public static GUIStyle NormalWhiteBG
        {
            get
            {
                if (normalWhiteBG == null)
                    normalWhiteBG = USeqSkin.GetStyle("NormalWhiteBG");
                return normalWhiteBG;
            }
            set {; }
        }

        static public Texture LoadTexture(string textureName)
        {
            string directoryName = EditorGUIUtility.isProSkin ? "_ProElements" : "_FreeElements";
            string fullFilename = String.Format("{0}/{1}", directoryName, textureName);
            return Resources.Load(fullFilename) as Texture;
        }

        private static void CreateNew(GameObject gameObject, String localPath)
        {
            UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(localPath);
            GameObject prefabGO = PrefabUtility.ReplacePrefab(gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
            EditorUtility.SetDirty(prefabGO);
        }

        private static void UpdateExisting(GameObject gameObject, String localPath)
        {
            GameObject prefabGO = PrefabUtility.ReplacePrefab(gameObject, PrefabUtility.GetPrefabParent(gameObject), ReplacePrefabOptions.ConnectToPrefab);
            EditorUtility.SetDirty(prefabGO);
        }

        public static System.Type[] GetAllSubTypes(System.Type aBaseClass)
        {
            var result = new System.Collections.Generic.List<System.Type>();
            System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var A in AS)
            {
                try
                {
                    System.Type[] types = A.GetTypes();
                    foreach (var T in types)
                    {
                        if (T.IsSubclassOf(aBaseClass))
                            result.Add(T);
                    }
                }
                catch (System.Reflection.ReflectionTypeLoadException) {; }
            }
            return result.ToArray();
        }

        public static bool DoRectsOverlap(Rect RectA, Rect RectB)
        {
            return RectA.xMin < RectB.xMax && RectA.xMax > RectB.xMin &&
                RectA.yMin < RectB.yMax && RectA.yMax > RectB.yMin;
        }

        public static int FindKeyframeIndex(AnimationCurve curve, Keyframe keyframe)
        {
            for (int n = 0; n < curve.keys.Length; n++)
            {
                if (curve.keys[n].Equals(keyframe))
                    return n;
            }

            return -1;
        }

        public static void RemoveFromUnitySelection(UnityEngine.Object objectToRemove)
        {
            var newSelection = Selection.objects.ToList();
            newSelection.RemoveAll(element => element == objectToRemove);
            Selection.objects = newSelection.ToArray();
        }

        public static void RemoveFromUnitySelection(List<UnityEngine.Object> objectToRemove)
        {
            var newSelection = Selection.objects.ToList();
            newSelection.RemoveAll(element => objectToRemove.Contains(element));
            Selection.objects = newSelection.ToArray();
        }

    }
}