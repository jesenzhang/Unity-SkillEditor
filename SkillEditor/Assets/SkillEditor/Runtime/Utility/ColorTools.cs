using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace CySkillEditor
{
    public class ColorTools
    {
        private static float colorRip = 0.0f;
        public static Color SelectColor = Color.HSVToRGB(1, 0.5f, 0.5f);
        public static Color GetGrandientColor(float n)
        {
            Color color = Color.HSVToRGB(n, 1f, 1f);
            color.a = 1;
            return color;
        }
    }
}