using UnityEngine;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using ProtoBuf;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CySkillEditor
{
    public static class TransformExtensions
    {
        public static string GetFullHierarchyPath(this Transform transform)
        {
            string path = "/" + transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = "/" + transform.name + path;
            }
            return path;
        }
    }

    public static class MecanimAnimationUtility
    {
        public static List<string> GetAllStateNames(GameObject gameObject, int layer)
        {
            var animator = (gameObject as GameObject).GetComponent<Animator>();
            var availableStateNames = new List<string>();

            if (animator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController)
            {
                var ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
                var sm = ac.layers[layer].stateMachine;
                for (int i = 0; i < sm.states.Length; i++)
                {
                    UnityEditor.Animations.AnimatorState state = sm.states[i].state;
                    var stateName = state.name;
                    availableStateNames.Add(stateName);
                }
            }
            else if (animator.runtimeAnimatorController is UnityEngine.AnimatorOverrideController)
            {
                var aoc = animator.runtimeAnimatorController as UnityEngine.AnimatorOverrideController;
                aoc.animationClips.ToList().ForEach(animationClip => availableStateNames.Add(animationClip.name));
            }
            else //if(animator.runtimeAnimatorController!=null)
            {
                animator.runtimeAnimatorController.animationClips.ToList().ForEach(animationClip => availableStateNames.Add(animationClip.name));
            }
           // else
            {
            //    Debug.LogError("AnimatorController Type not supported");
            }

            return availableStateNames;
        }

        public static List<string> GetAllStateNamesWithController(RuntimeAnimatorController controller)
        {
            var availableStateNames = new List<string>();
            if (controller == null)
                return availableStateNames;

            if (controller is UnityEditor.Animations.AnimatorController)
            {
                var ac = controller as UnityEditor.Animations.AnimatorController;
                for (int j = 0; j < ac.layers.Length; j++)
                {
                    var sm = ac.layers[j].stateMachine;
                    for (int i = 0; i < sm.states.Length; i++)
                    {
                        UnityEditor.Animations.AnimatorState state = sm.states[i].state;
                        var stateName = state.name;
                        availableStateNames.Add(stateName);
                    }
                }

            }
            else if (controller is UnityEngine.AnimatorOverrideController)
            {
                var aoc = controller as UnityEngine.AnimatorOverrideController;
                aoc.animationClips.ToList().ForEach(animationClip => availableStateNames.Add(animationClip.name));
            }
            else if (controller!=null )
            {
                controller.animationClips.ToList().ForEach(animationClip => availableStateNames.Add(animationClip.name));
            }
             else
              {
                 Debug.LogError("AnimatorController Type not supported");
              }

            return availableStateNames;
        }

        public static List<string> GetAllLayerNames(GameObject gameObject)
        {
            var animator = (gameObject as GameObject).GetComponent<Animator>();
            var availableLayerNames = new List<string>();
            for (int layerIndex = 0; layerIndex < animator.layerCount; layerIndex++)
                availableLayerNames.Add(animator.GetLayerName(layerIndex));

            return availableLayerNames;
        }

        public static int LayerNameToIndex(GameObject gameObject, string layerName)
        {
            var animator = (gameObject as GameObject).GetComponent<Animator>();

            for (int layerIndex = 0; layerIndex < animator.layerCount; layerIndex++)
            {
                if (animator.GetLayerName(layerIndex) == layerName)
                    return layerIndex;
            }

            return 0;
        }

        public static string LayerIndexToName(GameObject gameObject, int requestedLayerIndex)
        {
            var animator = (gameObject as GameObject).GetComponent<Animator>();
            return animator.GetLayerName(requestedLayerIndex);
        }

        public static float GetStateDuration(string stateName, GameObject gameObject)
        {
            var animator = (gameObject as GameObject).GetComponent<Animator>();

            if (animator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController)
            {
                var ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
                for (var layerIndex = 0; layerIndex < animator.layerCount; layerIndex++)
                {
                    var sm = ac.layers[layerIndex].stateMachine;
                    for (int i = 0; i < sm.states.Length; i++)
                    {
                        var state = sm.states[i].state;
                        if (state.name == stateName)
                            return state.motion.averageDuration;
                    }
                }
            }
            else if (animator.runtimeAnimatorController is UnityEngine.AnimatorOverrideController)
            {
                var aoc = animator.runtimeAnimatorController as UnityEngine.AnimatorOverrideController;
                for (int i = 0; i < aoc.animationClips.Count(); i++)
                {
                    var clipName = aoc.animationClips[i].name;
                    if (clipName == stateName)
                        return aoc.animationClips[i].averageDuration;
                }
            }
            else if (animator.runtimeAnimatorController!=null)
            {
                var aoc = animator.runtimeAnimatorController;
                for (int i = 0; i < aoc.animationClips.Count(); i++)
                {
                    var clipName = aoc.animationClips[i].name;
                    if (clipName == stateName)
                        return aoc.animationClips[i].averageDuration;
                }
            }
            else
            {
                Debug.LogError("AnimatorController Type not supported");
            }

            throw new System.Exception(string.Format("StateName {0} not found", stateName));
        }

        public static float GetStateDurationWithAnimatorController(string stateName, RuntimeAnimatorController controller)
        {

            if (controller is UnityEditor.Animations.AnimatorController)
            {
                var ac = controller as UnityEditor.Animations.AnimatorController;
                for (var layerIndex = 0; layerIndex < ac.layers.Length; layerIndex++)
                {
                    var sm = ac.layers[layerIndex].stateMachine;
                    for (int i = 0; i < sm.states.Length; i++)
                    {
                        var state = sm.states[i].state;
                        if (state.name == stateName)
                            return state.motion.averageDuration;
                    }
                }
            }
            else if (controller is UnityEngine.AnimatorOverrideController)
            {
                var aoc = controller as UnityEngine.AnimatorOverrideController;
                for (int i = 0; i < aoc.animationClips.Count(); i++)
                {
                    var clipName = aoc.animationClips[i].name;
                    if (clipName == stateName)
                        return aoc.animationClips[i].averageDuration;
                }
            }
            else if (controller !=null)
            {
                for (int i = 0; i < controller.animationClips.Count(); i++)
                {
                    var clipName = controller.animationClips[i].name;
                    if (clipName == stateName)
                        return controller.animationClips[i].averageDuration;
                }


            }
            else
            {
                Debug.LogError("AnimatorController Type not supported");
            }
           // throw new System.Exception(string.Format("StateName {0} not found", stateName));
            return 0;
           
        }

        public static UnityEditor.Animations.AnimatorState GetState(string stateName, GameObject gameObject)
        {
            var animator = (gameObject as GameObject).GetComponent<Animator>();
            UnityEditor.Animations.AnimatorController ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            var sm = ac.layers[0].stateMachine;
            for (int i = 0; i < sm.states.Length; i++)
            {
                var state = sm.states[i].state;
                if (state.name == stateName)
                    return state;
            }

            throw new System.Exception(string.Format("StateName {0} not found", stateName));
        }
    }

    public static class ParticleSystemUtility
    {
        public static bool IsRoot(ParticleSystem ps)
        {
            if (ps == null)
            {
                return false;
            }

            var parent = ps.transform.parent;

            if (parent == null)
                return true;
            if (parent.GetComponent<ParticleSystem>() != null)
                return false;
            else
                return true;

        }
        public static float GetParticleDuration(string stateName, GameObject gameObject)
        {
            List<ParticleSystem> list = new List<ParticleSystem>();
            ParticleSystem[] pchild = gameObject.GetComponentsInChildren<ParticleSystem>();

            if (pchild != null)
            {
                list.AddRange(pchild);
            }
            int len = list.Count;
            for (int i = 0; i < len; i++)
            {
                if (list[i].name == stateName)
                {
                    return list[i].main.duration;
                }
            }
            return 0;
        }


    }

    public class AssetUtility
    {
        //资源根路径
        public static string ResPath = "Res/";
        //主角资源路径
        public static string PlayerPath = "Player/";
        //NPC资源路径
        public static string NPCPath = "NPC/";
        //技能特效资源路径
        public static string SkillEffectPath = "Effect/particle/Skill/";
        //声音资源路径
        public static string AudioPath = "Audio/";


        // 目录是否存在
        public static void CheckAssetPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public static void inputFiles(string path, Dictionary<string, int> files, string regex)
        {
            string[] filesList = Directory.GetFiles(path, regex, SearchOption.AllDirectories);
            for (int i = 0; i < filesList.Length; i++)
            {
                string filePath = filesList[i].Replace(@"\", @"/");
                // if (Regex.IsMatch(filePath, regex) == true)
                {
                    if (files.ContainsKey(filePath) == false)
                    {
                        files.Add(filePath, 0);
                    }
                    else
                    {
                        files[filePath]++;
                    }
                }
            }
        }
        public static List<GameObject> GetAllFBX()
        {
            string Path = "Assets/" + ResPath;
            Dictionary<string, int> files = new Dictionary<string, int>();
            inputFiles(Path, files, "*.fbx");
            List<GameObject> allfbx = new List<GameObject>();
            foreach (var p in files.Keys)
            {
                GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath<GameObject>(p);
                allfbx.Add(prefab);
            }

            return allfbx;
        }

        public static List<GameObject> GetAllFBXWithType(ModelTargetType mtype)
        {
            string Path = "Assets/" + ResPath;
            if (mtype == ModelTargetType.Player)
                Path = "Assets/" + ResPath + PlayerPath;
            if (mtype == ModelTargetType.NPC)
                Path = "Assets/" + ResPath + NPCPath;
            Dictionary<string, int> files = new Dictionary<string, int>();
            inputFiles(Path, files, "*.fbx");
            List<GameObject> allfbx = new List<GameObject>();
            foreach (var p in files.Keys)
            {
                GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath<GameObject>(p);
                allfbx.Add(prefab);
            }

            return allfbx;
        }
        public static GameObject GetFBXWithName(string Name)
        {
            string Path = "Assets/" + ResPath;
            Dictionary<string, int> files = new Dictionary<string, int>();
            inputFiles(Path, files, Name + ".fbx");
            List<GameObject> allfbx = new List<GameObject>();
            foreach (var p in files.Keys)
            {
                GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath<GameObject>(p);
                allfbx.Add(prefab);
            }
            if (allfbx.Count >= 1)
                return allfbx[0];
            else
                return null;
        }
        public static List<GameObject> GetAllSkillEffectPrefabs()
        {
            string Path = "Assets/" + ResPath + SkillEffectPath;
            Dictionary<string, int> files = new Dictionary<string, int>();
            inputFiles(Path, files, "*.prefab");
            List<GameObject> allfbx = new List<GameObject>();
            foreach (var p in files.Keys)
            {
                GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath<GameObject>(p);
                allfbx.Add(prefab);
            }
            return allfbx;
        }

        public static List<GameObject> GetModelSkillEffectPrefabs(string modelName)
        {
            List<GameObject> allfbx = new List<GameObject>();
            string Path = "Assets/" + ResPath + SkillEffectPath;
            Dictionary<string, int> files = new Dictionary<string, int>();
            inputFiles(Path, files, modelName + "*.prefab");

            foreach (var p in files.Keys)
            {
                GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath<GameObject>(p);
                allfbx.Add(prefab);
            }

            return allfbx;
        }
        public static GameObject GetAsset(string Path)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/" + Path);
            return prefab;
        }
        public static string GetPlayerPath(string PlayerName)
        {
            string path = "Assets/" + ResPath + PlayerPath + PlayerName + "/" + PlayerName + ".FBX";

            return path;
        }

        public static string GetNPCPath(string NPCName)
        {
            string path = "Assets/" + ResPath + NPCPath + NPCName + "/" + NPCName + ".FBX";

            return path;
        }
        public static string GetPlayerEffectPath(string PlayerName, string EffectName)
        {
            string path = "Assets/" + ResPath + SkillEffectPath + PlayerName + "/" + EffectName + ".prefab";
            return path;
        }

        public static string GetNPCEffectPath(string NPCName, string EffectName)
        {
            string path = "Assets/" + ResPath + SkillEffectPath + NPCName + "/" + EffectName + ".prefab";
            return path;
        }

        public static GameObject GetPlayerAsset(string PlayerName)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GetPlayerPath(PlayerName));
            if (prefab == null)
            {
                throw new System.Exception(string.Format("PlayerAsset {0} not found", PlayerName));
            }
            return prefab;
        }

        public static GameObject GetNPCAsset(string NPCName)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GetNPCPath(NPCName));
            if (prefab == null)
            {
                throw new System.Exception(string.Format("PlayerAsset {0} not found", NPCName));
            }
            return prefab;
        }

        public static GameObject GetFBXAsset(ModelTargetType tartype, string Name)
        {
            GameObject fbx = null;
            if (tartype == ModelTargetType.Player)
                fbx = GetPlayerAsset(Name);
            if (tartype == ModelTargetType.NPC)
                fbx = GetNPCAsset(Name);
            return fbx;
        }

        //获取人物的技能特效
        public static GameObject GetPlayerSkillEffect(string PlayerName, string EffectName)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GetPlayerEffectPath(PlayerName, EffectName));
            if (prefab == null)
            {
                throw new System.Exception(string.Format(PlayerName + " SkillEffect {0} not found", EffectName));
            }
            return prefab;
        }

        public static GameObject GetNPCSkillEffect(string NPCName, string EffectName)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GetNPCEffectPath(NPCName, EffectName));
            if (prefab == null)
            {
                throw new System.Exception(string.Format(NPCName + " SkillEffect {0} not found", EffectName));
            }
            return prefab;
        }

        //获取技能特效
        public static GameObject GetSkillEffect(ModelTargetType tartype, string PlayerName, string EffectName)
        {
            string path = "Assets/" + ResPath + SkillEffectPath + PlayerName + "/" + EffectName + ".prefab";
            if (tartype == ModelTargetType.Player)
                path = "Assets/" + ResPath + SkillEffectPath + PlayerName + "/" + EffectName + ".prefab";
            if (tartype == ModelTargetType.NPC)
                path = "Assets/" + ResPath + SkillEffectPath + PlayerName + "/" + EffectName + ".prefab";

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                throw new System.Exception(string.Format(PlayerName + " SkillEffect {0} not found", EffectName));
            }
            return prefab;
        }

        public static RuntimeAnimatorController GetPlayerAnimationCtl(string PlayerName, string CtlName)
        {
            string path = "Assets/" + ResPath + PlayerPath + PlayerName + "/Animation/" + CtlName + ".controller";
            RuntimeAnimatorController prefab = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
            return prefab;
        }
        public static RuntimeAnimatorController GetAnimationCtl(ModelTargetType tartype, string Name, string CtlName)
        {
            string path = "Assets/" + ResPath + PlayerPath + Name + "/Animation/" + CtlName + ".controller";
            if (tartype == ModelTargetType.Player)
                path = "Assets/" + ResPath + PlayerPath + Name + "/Animation/" + CtlName + ".controller";
            if (tartype == ModelTargetType.NPC)
                path = "Assets/" + ResPath + NPCPath + Name + "/Animation/" + CtlName + ".controller";
            RuntimeAnimatorController prefab = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
            return prefab;
        }

        public static void SaveDataAsAsset(UnityEngine.Object data, string Path)
        {
            AssetDatabase.CreateAsset(data, Path);
        }
        public static UnityEngine.Object LoadDataAsAssetFrom(string Path)
        {
            return AssetDatabase.LoadAssetAtPath(Path, typeof(UnityEngine.Object));
        }

        public static void LoadFile(SkillAssetData data)
        {
            // 导入
            string path = EditorUtility.OpenFilePanel("Load SkillAssetData", Application.dataPath + "/", "asset");
            if (path.Length != 0)
            {
                path = "Assets" + path.Replace(Application.dataPath, "");

                data = AssetDatabase.LoadAssetAtPath(path, typeof(SkillAssetData)) as SkillAssetData;
            }
        }
        public static void SaveSkill(SkillAssetData data)
        {
            // 保存
            string path = EditorUtility.SaveFilePanel("Save SkillAssetData", Application.dataPath + "/", "", "asset");
            SaveDataAsAsset(data, path);
        }

    }
    
  
}