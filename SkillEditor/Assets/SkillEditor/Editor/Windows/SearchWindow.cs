using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CySkillEditor
{
    public class SearchWindow : EditorWindow
    {
        public static Vector2 minWindowSize = new Vector2(750.0f, 250.0f);
        int id = -1;
        string idName = "";
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("输入技能id数字或id名称搜索");
            id = EditorGUILayout.IntField("id数字：",id);
            idName = EditorGUILayout.TextField("id名称：", idName);
            if (GUILayout.Button("id数字搜索并应用", GUILayout.Height(40)))
            {
                if (id > 0 && EditorDataContainer.allSkillUnits.units.Count > id)
                {
                    idName = EditorDataContainer.GetSkillStringById(id);
                    DataConvert.ConvertSkillUnit(EditorDataContainer.currentskillAssetData.skillUnit, EditorDataContainer.allSkillUnits.units[id]);
                    int curart = EditorDataContainer.currentskillAssetData.skillUnit.artId;
                    SkillUnit.SkillArt oart = EditorDataContainer.allSkillUnits.arts[curart];
                    DataConvert.ConvertSkillArt(EditorDataContainer.currentskillAssetData.skillArt, oart);
                    EditorDataContainer.UseSkillArt(oart);
                }
            }
            if (GUILayout.Button("id名字搜索并应用", GUILayout.Height(40)))
            {
                id= EditorDataContainer.GetSkillIdByString(idName);
                if (id > 0 && EditorDataContainer.allSkillUnits.units.Count > id)
                {
                    DataConvert.ConvertSkillUnit(EditorDataContainer.currentskillAssetData.skillUnit, EditorDataContainer.allSkillUnits.units[id]);
                    int curart = EditorDataContainer.currentskillAssetData.skillUnit.artId;
                    SkillUnit.SkillArt oart = EditorDataContainer.allSkillUnits.arts[curart];
                    DataConvert.ConvertSkillArt(EditorDataContainer.currentskillAssetData.skillArt, oart);
                    EditorDataContainer.UseSkillArt(oart);
                }
            }
            if (GUILayout.Button("Close", GUILayout.Height(40)))
            {
                Close();
            }
            EditorGUILayout.EndVertical();
        }
           
    }
}