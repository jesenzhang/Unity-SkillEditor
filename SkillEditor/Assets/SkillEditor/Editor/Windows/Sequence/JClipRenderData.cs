
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
namespace CySkillEditor
{
    [Serializable]
    [CanEditMultipleObjects]
    public class JClipRenderData : ScriptableObject
    {
        [SerializeField]
        public Rect renderRect;
        [SerializeField]
        public Rect labelRect;
        [SerializeField]
        public Rect transitionRect;
        [SerializeField]
        public Rect leftHandle;
        [SerializeField]
        public Rect rightHandle;
        [SerializeField]
        public Vector2 renderPosition;
        [SerializeField]
        public ScriptableObject ClipData;
        [SerializeField]
        public int index;

    }
}