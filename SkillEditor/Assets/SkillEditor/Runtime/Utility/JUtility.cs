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
            else
            {
                Debug.LogError("AnimatorController Type not supported");
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
            else
            {
                controller.animationClips.ToList().ForEach(animationClip => availableStateNames.Add(animationClip.name));
            }
            // else
            //  {
            //     Debug.LogError("AnimatorController Type not supported");
            //  }

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
            else
            {
                for (int i = 0; i < controller.animationClips.Count(); i++)
                {
                    var clipName = controller.animationClips[i].name;
                    if (clipName == stateName)
                        return controller.animationClips[i].averageDuration;
                }

                //  Debug.LogError("AnimatorController Type not supported");
            }

            return 0;
            throw new System.Exception(string.Format("StateName {0} not found", stateName));
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
                    return list[i].duration;
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
    public enum ModelTargetType
    {
        Player = 0,
        NPC = 1
    }
    public class EditorDataContainer
    {
        static public string SkillUnitpath = "Assets/Bytes/SkillUnit.bytes";
        static public string StringIDpath = "Assets/Bytes/StringID.bytes";
        //技能表
        static public SkillUnit.AllSkillUnits allSkillUnits = new SkillUnit.AllSkillUnits();
        //stringID 表
        static public StringID.AllStringID allStringID = new StringID.AllStringID();

        //当前技能信息
        static public SkillAssetData currentskillAssetData = new SkillAssetData();

        static public SkillUnit.SkillArt CloneSkillArt(SkillUnit.SkillArt a)
        {
            SkillUnit.SkillArt b = new SkillUnit.SkillArt();
            b.beginCameraAction = a.beginCameraAction;
            List<SkillUnit.SkillArtEffect> bl = b.beginEffect;
            for (int i = 0; i < a.beginEffect.Count; i++) {
                SkillUnit.SkillArtEffect se = a.beginEffect[i];
                bl.Add(se);
            }

            b.endAction = a.endAction;
            b.endEffect = a.endEffect;

            b.guideAction = a.guideAction;
            b.guideFadeTime = a.guideFadeTime;
            b.guidingAction = a.guidingAction;
            b.hitCameraAction = a.hitCameraAction;
            b.id = a.id;
            b.tipReferPoint = a.tipReferPoint;
            b.moveCameraAction = a.moveCameraAction;
            b.tipEffect = a.tipEffect;
            b.unitEffect = a.unitEffect;
            return b;
        }
        static public SkillUnit.SkillUnit CloneSkillUnit(SkillUnit.SkillUnit a)
        {
            SkillUnit.SkillUnit b = new SkillUnit.SkillUnit();
            b.artId = a.artId;
            b.cd = a.cd;
            b.distance = a.distance;
            b.id = a.id;
            b.launchType = a.launchType;
            b.referId = a.referId;
            b.skillTime = a.skillTime;
            b.targetType = a.targetType;
            b.guidePolicy = new SkillUnit.SkillGuidePolicy();
            b.guidePolicy.guideTime = a.guidePolicy.guideTime;
            b.guidePolicy.guidingTime = a.guidePolicy.guidingTime;
            b.guidePolicy.endTime = a.guidePolicy.endTime;
            b.guidePolicy.type = a.guidePolicy.type;
            return b;
        }
        public static void Clear()
        {
            //技能表
            allSkillUnits = null;
            //形体表
            allStringID = null;
        }

        public static void LoadSystemTables()
        {
            // TextAsset systemdata = AssetDatabase.LoadAssetAtPath(SkillUnitpath, typeof(TextAsset)) as TextAsset;
            byte[] psubys = ReadFileAtPath(SkillUnitpath);
            MemoryStream ms = new MemoryStream(psubys);
            allSkillUnits = ProtoBuf.Serializer.Deserialize<SkillUnit.AllSkillUnits>(ms);

            // TextAsset Physiquedata = AssetDatabase.LoadAssetAtPath(Physiquepath, typeof(TextAsset)) as TextAsset;
            byte[] pbys = ReadFileAtPath(StringIDpath);
            MemoryStream Phyms = new MemoryStream(pbys);
            allStringID = ProtoBuf.Serializer.Deserialize<StringID.AllStringID>(Phyms);
        }
        public static byte[] ReadFileAtPath(string Path)
        {
            FileStream file = new FileStream(Path, FileMode.Open);
            byte[] bts = new byte[file.Length];
            file.Read(bts, 0, bts.Length);
            if (file != null)
            {
                file.Flush();
                file.Close();
                file.Dispose();
                Debug.Log("load file");
            }
            return bts;
        }

        public static void SaveSystemTables()
        {
            SaveBytesToPath<StringID.AllStringID>(allStringID, StringIDpath);
            SaveBytesToPath<SkillUnit.AllSkillUnits>(allSkillUnits, SkillUnitpath);
        }

        public static void SaveBytesToPath<T>(System.Object data, string Path)
        {
            MemoryStream outPhy = new MemoryStream();
            ProtoBuf.Serializer.Serialize<T>(outPhy, (T)data);

            FileStream file = new FileStream(Path, FileMode.Create);
            byte[] bts = outPhy.ToArray();
            file.Write(bts, 0, bts.Length);
            if (file != null)
            {
                file.Flush();
                file.Close();
                file.Dispose();
                Debug.Log("save to file");
            }
        }

        //保存当前技能为Asset
        public static void SaveSkillArt(string Path)
        {
           // MemoryStream outPhy = new MemoryStream();
           // ProtoBuf.Serializer.Serialize<SkillUnit.SkillArt>(outPhy, currentskillAssetData.skillart);
           // currentskillAssetData.skillartString = outPhy.ToArray();
           // MemoryStream outPhy1 = new MemoryStream();
           // ProtoBuf.Serializer.Serialize<SkillUnit.SkillUnit>(outPhy1, currentskillAssetData.skillunit);
           // currentskillAssetData.skillunitString = outPhy1.ToArray();
            SkillAssetData copydata = currentskillAssetData.Copy();
            AssetDatabase.CreateAsset(copydata, Path);
            AssetDatabase.Refresh();
        }
        //加载保存的技能asset
        public static void LoadSkillArt(string Path)
        {
            SkillAssetData copydata = AssetDatabase.LoadAssetAtPath(Path, typeof(SkillAssetData)) as SkillAssetData;
            currentskillAssetData = copydata.Copy();
            //MemoryStream outPhy = new MemoryStream(currentskillAssetData.skillartString);
            //currentskillAssetData.skillart = CloneSkillArt(ProtoBuf.Serializer.Deserialize<SkillUnit.SkillArt>(outPhy));
            //MemoryStream outPhy1 = new MemoryStream(currentskillAssetData.skillunitString);
            //currentskillAssetData.skillunit = CloneSkillUnit(ProtoBuf.Serializer.Deserialize<SkillUnit.SkillUnit>(outPhy1));
            AssetDatabase.Refresh();
        }

        static public SkillUnit.SkillArt GetSkillArtById(int artid)
        {
            for (int i = 0; i < allSkillUnits.arts.Count; i++)
            {
                SkillUnit.SkillArt art = allSkillUnits.arts[i];
                Debug.Log("art id " + art.id + "   " + art.guideAction);
                if (art.id == artid)
                {
                    return art;
                }

            }
            return null;
        }

        //特效名称 模型名称
        static public string GetStringById(int id)
        {
            if (id >= 0 && id < allStringID.ids.Count)
                return allStringID.ids[id].id;
            return "None";
        }
        //特效名称 模型名称
        static public int GetIdByString(string name)
        {
            for (int i = 0; i < allStringID.ids.Count; i++)
            {
                if (allStringID.ids[i].id == name)
                    return i;
            }
            return -1;
        }
        //技能id 名称
        static public int GetSkillIdByString(string name)
        {
            for (int i = 0; i < allSkillUnits.stringID.ids.Count; i++)
            {
                if (allSkillUnits.stringID.ids[i].id == name)
                    return i;
            }
            return -1;
        }
        //技能id 名称
        static public string GetSkillStringById(int id)
        {
            if (id >= 0 && id < allSkillUnits.stringID.ids.Count)
                return allSkillUnits.stringID.ids[id].id;
            return "";
        }

        public static void SaveSkillAssetData()
        {
            string path = EditorUtility.SaveFilePanel("Save SkillArt", Application.dataPath + "/" + "Asset", "", "asset");
            if (path.Length != 0)
            {
                path = "Assets" + path.Replace(Application.dataPath, "");
                EditorDataContainer.SaveSkillArt(path);
            }
        }
        public static void LoadSkillAssetData()
        {
            string path = EditorUtility.OpenFilePanel("Load SkillArt", Application.dataPath + "/" + "Asset", "asset");
            if (path.Length != 0)
            {
                path = "Assets" + path.Replace(Application.dataPath, ""); 
                EditorDataContainer.LoadSkillArt(path);
            }
        }
        public static SkillAssetData MakeSkillAssetData(JSequencer sequenceer)
        {
            SkillAssetData skillart = new SkillAssetData();
            Transform AffectedObject = sequenceer.TimelineContainers[0].AffectedObject;
            string ModelName = AffectedObject.name;
            List<GameObject> plist = AssetUtility.GetAllFBXWithType(ModelTargetType.Player);
            List<GameObject> nlist = AssetUtility.GetAllFBXWithType(ModelTargetType.NPC);
            skillart.skillArt.modelType = ModelTargetType.Player;
            bool findtype = false;
            for (int i = 0; i < plist.Count && !findtype; i++)
            {
                if (plist[i].name == ModelName)
                {
                    skillart.skillArt.modelType = ModelTargetType.Player;
                    findtype = true;
                }
            }
            for (int i = 0; i < nlist.Count && !findtype; i++)
            {
                if (nlist[i].name == ModelName)
                {
                    skillart.skillArt.modelType = ModelTargetType.NPC;
                    findtype = true;
                }
            }
            skillart.skillArt.modelName = ModelName;
            skillart.skillArt.model = AssetUtility.GetFBXWithName(ModelName);
            Animator Animator = AffectedObject.GetComponent<Animator>();
            skillart.skillArt.animationController = Animator.runtimeAnimatorController.name;
            skillart.skillArt.id = EditorDataContainer.GetSkillIdByString(AffectedObject.parent.name.Split('_')[1]);
            if (sequenceer.TimelineContainers.Length > 0)
            {
                JTimelineBase[] lines = sequenceer.TimelineContainers[0].Timelines;
                for (int i = 0; i < lines.Length; i++)
                {
                    JTimelineBase line = lines[i];
                    //动作时间线 guideaction
                    if (line.LineType() == TimeLineType.Animation)
                    {
                        JTimelineAnimation aniline = (JTimelineAnimation)line;
                        foreach (var track in aniline.AnimationTracks)
                        {
                            if (track.name == "guideAction")
                            {
                                if (track.TrackClips.Count > 0)
                                {
                                    skillart.skillArt.guideAction = track.TrackClips[0].StateName;
                                }
                            } else if (track.name == "guidingAction")
                            {
                                if (track.TrackClips.Count > 0)
                                {
                                    skillart.skillArt.guidingAction = track.TrackClips[0].StateName;
                                }
                            } else if (track.name == "endAction")
                            {
                                if (track.TrackClips.Count > 0)
                                {
                                    skillart.skillArt.endAction = track.TrackClips[0].StateName;
                                }
                            }
                        }
                    }
                    if (skillart.skillArt.beginEffect == null)
                        skillart.skillArt.beginEffect = new List<SkillEffectUnit>();
                    List<SkillEffectUnit> begineffect = skillart.skillArt.beginEffect;
                    begineffect.Clear();
                    //特效时间线 begineffect
                    if (line.LineType() == TimeLineType.Effect)
                    {
                        JTimelineParticle aniline = (JTimelineParticle)line;

                        foreach (var track in aniline.ParticleTracks)
                        {
                            if (track.name == "beginEffect")
                            {
                                if (track.TrackClips.Count > 0)
                                {
                                    SkillEffectUnit unit = new SkillEffectUnit();
                                    unit.configure = track.TrackClips[0].EffectConfig.Copy();
                                    unit.artEffect.beginTime = (int)(track.TrackClips[0].StartTime*1000f);
                                    unit.artEffect.phaseTime = (int)(track.TrackClips[0].PlaybackDuration * 1000f);
                                    unit.artEffect.effectObj = track.TrackClips[0].Effect;
                                    unit.artEffect.effect = EditorDataContainer.GetIdByString(track.TrackClips[0].ParticleName);
                                    begineffect.Add(unit);
                                }
                            }
                        }
                    }
                    //弹道时间线
                    if (skillart.skillArt.unitEffect == null)
                        skillart.skillArt.unitEffect = new List<SkillEffectUnit>();
                    List<SkillEffectUnit> unitEffect = skillart.skillArt.unitEffect;
                    unitEffect.Clear();
                    if (line.LineType() == TimeLineType.Trajectory)
                    {
                        JTimelineTrajectory aniline = (JTimelineTrajectory)line;

                        foreach (var track in aniline.TrajectoryTracks)
                        {
                            if (track.name == "unitEffect")
                            {
                                if (track.TrackClips.Count > 0)
                                {
                                    skillart.skillUnit = track.TrackClips[0].skillunit.Copy();
                                    unitEffect.Add(track.TrackClips[0].effectunit.Copy());
                                }
                            }
                        }
                    }
                }

            }
            return skillart;
        }
        //生成序列
        public static JSequencer GenerateSequence(SkillAssetData skillart)
        {
            //id 名称
            string SkillId = EditorDataContainer.GetSkillStringById(skillart.skillArt.id);
            //模型名称
            string ModelName = skillart.skillArt.modelName;
            //起始动作
            string guideAction = skillart.skillArt.guideAction;
            //持续动作
            string guidingAction = skillart.skillArt.guidingAction;
            //结束动作
            string endAction = skillart.skillArt.endAction;
            //animator名称
            string animatorName = skillart.skillArt.animationController;
            //模型类型 主角  NPC 
            ModelTargetType modelType = skillart.skillArt.modelType;
            //技能列表
            List<SkillEffectUnit> beginEffect = skillart.skillArt.beginEffect;

            GameObject ShowRoot = new GameObject("ShowRoot_" + SkillId);
            //获取模型 实例化
            GameObject PlayerModel = AssetUtility.GetFBXAsset(modelType, ModelName);
            GameObject Player = null;
            if (PlayerModel != null)
            {
                Player = UnityEngine.Object.Instantiate(PlayerModel) as GameObject;
                Player.name = ModelName;
                Player.transform.parent = ShowRoot.transform;
            }
            else {
                Player = new GameObject(ModelName);
                Player.transform.parent = ShowRoot.transform;
            }
            //获取animator 赋值
            RuntimeAnimatorController controll = AssetUtility.GetAnimationCtl(modelType, ModelName, animatorName);
            if (controll != null)
            {
                if (Player.GetComponent<Animator>() == null)
                    Player.AddComponent<Animator>();
                Player.GetComponent<Animator>().runtimeAnimatorController = controll;
            }

            //新建队列
            GameObject newSequence = new GameObject("Sequence_" + ModelName);
            JSequencer sequence = newSequence.AddComponent<JSequencer>();

            JTimelineContainer ccontainer = sequence.CreateNewTimelineContainer(Player.transform);
            JTimelineAnimation aniline = (JTimelineAnimation)ccontainer.AddNewTimeline(TimeLineType.Animation);
            if (guideAction != "-1")
            {
                JAnimationTrack track = aniline.AddNewTrack();
                track.name = "guideAction";
                track.AddClipWithState(guideAction, 0f);
            }
            if (guidingAction != "-1")
            {
                JAnimationTrack track1 = aniline.AddNewTrack();
                track1.name = "guidingAction";
                float t = guideAction == "-1" ? 0 : MecanimAnimationUtility.GetStateDurationWithAnimatorController(guideAction, controll);
                track1.AddClipWithState(guidingAction, t);
            }
            if (endAction != "-1")
            {
                JAnimationTrack track2 = aniline.AddNewTrack();
                track2.name = "endAction";
                float t1 = guideAction == "-1" ? 0 : MecanimAnimationUtility.GetStateDurationWithAnimatorController(guideAction, controll);
                float t = guidingAction == "-1" ? 0 : MecanimAnimationUtility.GetStateDurationWithAnimatorController(guidingAction, controll);
                track2.AddClipWithState(endAction, t1 + t);
            }

            JTimelineParticle parline = (JTimelineParticle)ccontainer.AddNewTimeline(TimeLineType.Effect);
            if (beginEffect != null)
            {
                //一个特效 一条轨道
                foreach (SkillEffectUnit effect in beginEffect)
                {
                    //特效名称
                    string effectName = EditorDataContainer.GetStringById(effect.artEffect.effect);
                    GameObject effectPrefab = AssetUtility.GetSkillEffect(modelType, ModelName, effectName);
                    // GameObject effectobj = UnityEngine.Object.Instantiate(effectPrefab) as GameObject;
                    // effectobj.name = effectName;
                    effect.configure.effectName = effectName;
                    // effectobj.transform.parent = Player.transform;
                    JParticleTrack ptrack = parline.AddNewTrack();
                    ptrack.name = "beginEffect";
                    ptrack.AddClipWithName(effectName, (float)effect.artEffect.beginTime / 1000.0f, (float)effect.artEffect.phaseTime / 1000.0f, effectPrefab, effect.configure);
                }
            }
            if (skillart.skillArt.unitEffect != null)
            {
                //一个特效 一条轨道
                foreach (SkillEffectUnit uniteffect in skillart.skillArt.unitEffect)
                {
                    JTimelineTrajectory trajectory = (JTimelineTrajectory)ccontainer.AddNewTimeline(TimeLineType.Trajectory);
                    JTrajectoryTrack track3 = trajectory.AddNewTrack();
                    track3.name = "unitEffect";
                    track3.AddClipWithName(uniteffect.artEffect.effectObj.name, uniteffect.artEffect.beginTime / 1000.0f, uniteffect.artEffect.phaseTime / 1000.0f, skillart.skillUnit.Copy(), uniteffect.Copy());
                }
            }
            if(skillart.skillArt.beginCameraAction!=null)
            {
                JTimelineCamera cameraline = (JTimelineCamera)ccontainer.AddNewTimeline(TimeLineType.CameraAction);
                JCameraTrack track = cameraline.AddNewTrack();
                track.name = "beginCameraAction";
                string name = Enum.GetName(typeof(SkillCameraAction.CameraAction), skillart.skillArt.beginCameraAction.action);
                track.AddClipWithName(skillart.skillArt.beginCameraAction.Copy());
            }
            if (skillart.skillArt.moveCameraAction != null)
            {
                JTimelineCamera cameraline = (JTimelineCamera)ccontainer.AddNewTimeline(TimeLineType.CameraAction);
                JCameraTrack track = cameraline.AddNewTrack();
                track.name = "moveCameraAction";
                string name = Enum.GetName(typeof(SkillCameraAction.CameraAction), skillart.skillArt.moveCameraAction.action);
                track.AddClipWithName(skillart.skillArt.moveCameraAction.Copy());
            }
            if (skillart.skillArt.hitCameraAction != null)
            {
                JTimelineCamera cameraline = (JTimelineCamera)ccontainer.AddNewTimeline(TimeLineType.CameraAction);
                JCameraTrack track = cameraline.AddNewTrack();
                track.name = "hitCameraAction";
                string name = Enum.GetName(typeof(SkillCameraAction.CameraAction), skillart.skillArt.hitCameraAction.action);
                track.AddClipWithName(skillart.skillArt.hitCameraAction.Copy());
            }

            return sequence;
        }

        public static void UseSkillArt(SkillUnit.SkillArt oart)
        {
            SkillArt art = EditorDataContainer.currentskillAssetData.skillArt;
            DataConvert.ConvertSkillArt(art, oart);

            string effectName = EditorDataContainer.GetStringById(oart.unitEffect.effect);
            if (art.unitEffect == null)
            {
                art.unitEffect = new List<SkillEffectUnit>();
            }
            if (art.unitEffect.Count == 0)
                art.unitEffect.Add(new SkillEffectUnit());
            art.unitEffect[0].artEffect.effectObj = AssetUtility.GetSkillEffect(art.modelType, art.modelName, effectName);

            string hitEffectName = EditorDataContainer.GetStringById(oart.hitEffect.effect);
            if (art.hitEffect == null)
            {
                art.hitEffect = new List<SkillEffectUnit>();
            }
            if (art.hitEffect.Count == 0)
                art.hitEffect.Add(new SkillEffectUnit());
            art.hitEffect[0].artEffect.effectObj = AssetUtility.GetSkillEffect(art.modelType, art.modelName, hitEffectName);

            string tipEffectName = EditorDataContainer.GetStringById(oart.tipEffect.effect);
            if (art.tipEffect == null)
            {
                art.tipEffect = new List<SkillEffectUnit>();
            }
            if (art.tipEffect.Count == 0)
                art.tipEffect.Add(new SkillEffectUnit());
            art.tipEffect[0].artEffect.effectObj = AssetUtility.GetSkillEffect(art.modelType, art.modelName, hitEffectName);

            for (int i = 0; i < oart.beginEffect.Count; i++)
            {
                string beginEffectName = EditorDataContainer.GetStringById(oart.beginEffect[i].effect);
                art.beginEffect[i].artEffect.effectObj = AssetUtility.GetSkillEffect(art.modelType, art.modelName, beginEffectName);
            }
             

        }
    }

    public class DataConvert
    {
        public static SkillShape ConvertSkillShape(SkillUnit.SkillShape unit)
        {
            SkillShape oUnit = new SkillShape();
            oUnit.area = (SkillShape.Area)(int)unit.area;
            oUnit.param1 = unit.param1;
            oUnit.param2= unit.param2;
            oUnit.param3 = unit.param3;
            return oUnit;
        }
        public static void ConvertSkillShape(SkillShape oUnit,SkillUnit.SkillShape unit)
        {
            oUnit.area = (SkillShape.Area)(int)unit.area;
            oUnit.param1 = unit.param1;
            oUnit.param2 = unit.param2;
            oUnit.param3 = unit.param3;
        }

        public static SkillGuidePolicy ConvertSkillGuidePolicy(SkillUnit.SkillGuidePolicy unit)
        {
            SkillGuidePolicy oUnit = new SkillGuidePolicy();
            oUnit.endTime = unit.endTime;
            oUnit.guideTime = unit.guideTime;
            oUnit.guidingTime = unit.guidingTime;
            oUnit.type = (SkillGuidePolicy.GuideType)(int)unit.type;
            return oUnit;
        }
        public static void ConvertSkillGuidePolicy(SkillGuidePolicy oUnit,SkillUnit.SkillGuidePolicy unit)
        {
            oUnit.endTime = unit.endTime;
            oUnit.guideTime = unit.guideTime;
            oUnit.guidingTime = unit.guidingTime;
            oUnit.type = (SkillGuidePolicy.GuideType)(int)unit.type;
        }
        public static SkillLine ConvertSkillLine(SkillUnit.SkillLine unit)
        {
            SkillLine oUnit = new SkillLine();
            oUnit.id = unit.id;
            oUnit.moveTime = unit.moveTime;
            oUnit.canPierce = unit.canPierce;
            oUnit.offset = Vector3.zero;
            if (unit.offset!=null)
                oUnit.offset = new Vector3(unit.offset.x, unit.offset.y, unit.offset.z);
            oUnit.speed = unit.speed;
            oUnit.waves = unit.waves;
            oUnit.waveDelay = unit.waveDelay;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillMultiLine ConvertSkillMultiLine(SkillUnit.SkillMultiLine unit)
        {
            SkillMultiLine oUnit = new SkillMultiLine();
            oUnit.id = unit.id;
            oUnit.moveTime = unit.moveTime;
            oUnit.canPierce = unit.canPierce;
            oUnit.offset = Vector3.zero;
            if (unit.offset != null)
                oUnit.offset = new Vector3(unit.offset.x, unit.offset.y, unit.offset.z);
            oUnit.speed = unit.speed;
            oUnit.waves = unit.waves;
            oUnit.unitCount = unit.unitCount;
            oUnit.waveDelay = unit.waveDelay;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            oUnit.shape = ConvertSkillShape(unit.shape);
            return oUnit;
        }
        public static SkillArea ConvertSkillArea(SkillUnit.SkillArea unit)
        {
            SkillArea oUnit = new SkillArea();
            oUnit.id = unit.id;
            oUnit.referPoint = (JSkillUnit.ReferPoint)(int)unit.referPoint;
            oUnit.basePoint = (JSkillUnit.BasePoint)(int)unit.basePoint;
            oUnit.waves = unit.waves;
            oUnit.waveDelay = unit.waveDelay;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillHelix ConvertSkillHelix(SkillUnit.SkillHelix unit)
        {
            SkillHelix oUnit = new SkillHelix();
            oUnit.id = unit.id;
            oUnit.moveTime = unit.moveTime;
            oUnit.canPierce = unit.canPierce;
            oUnit.offset = Vector3.zero;
            if (unit.offset != null)
                oUnit.offset = new Vector3(unit.offset.x, unit.offset.y, unit.offset.z);
            oUnit.maxRadius = unit.maxRadius;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillAreaRand ConvertSkillAreaRand(SkillUnit.SkillAreaRand unit)
        {
            SkillAreaRand oUnit = new SkillAreaRand();
            oUnit.id = unit.id;
            oUnit.referPoint = (JSkillUnit.ReferPoint)(int)unit.referPoint;
            oUnit.basePoint = (JSkillUnit.BasePoint)(int)unit.basePoint;
            oUnit.unitID = unit.unitID;
            oUnit.unitCount = unit.unitCount;
            oUnit.area = ConvertSkillShape(unit.area);
            return oUnit;
        }
        public static SkillFollow ConvertSkillFollow(SkillUnit.SkillFollow unit)
        {
            SkillFollow oUnit = new SkillFollow();
            oUnit.id = unit.id;
            oUnit.maxFollowTime = unit.maxFollowTime;
            oUnit.offset = Vector3.zero;
            if (unit.offset != null)
                oUnit.offset = new Vector3(unit.offset.x, unit.offset.y, unit.offset.z);
            oUnit.speed = unit.speed;
            oUnit.waves = unit.waves;
            oUnit.waveDelay = unit.waveDelay;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillJump ConvertSkillJump(SkillUnit.SkillJump unit)
        {
            SkillJump oUnit = new SkillJump();
            oUnit.id = unit.id;
            oUnit.height = unit.height;
            oUnit.speed = unit.speed;
            oUnit.moveTime = unit.moveTime;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.jumpType = (SkillJump.JumpType)(int)unit.jumpType;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillBackStab ConvertSkillBackStab(SkillUnit.SkillBackStab unit)
        {
            SkillBackStab oUnit = new SkillBackStab();
            oUnit.id = unit.id;
            oUnit.referPoint = (JSkillUnit.ReferPoint)(int)unit.referPoint;
            oUnit.basePoint = (JSkillUnit.BasePoint)(int)unit.basePoint;
            oUnit.maxInfluence = unit.maxInfluence;
            oUnit.moveDelay = unit.moveDelay;
            oUnit.hitArea = ConvertSkillShape(unit.hitArea);
            return oUnit;
        }
        public static SkillArtEffect ConvertSkillArtEffect(SkillUnit.SkillArtEffect unit)
        {
            SkillArtEffect oUnit = new SkillArtEffect();
            oUnit.effect = unit.effect;
            oUnit.effPos = (SkillArtEffect.EffPos)(int)unit.effPos;
            oUnit.height = unit.height;
            oUnit.phaseTime = unit.phaseTime;
            return oUnit;
        }
        public static void ConvertSkillArtEffect(SkillArtEffect oUnit, SkillUnit.SkillArtEffect unit)
        {
            oUnit.effect = unit.effect;
            oUnit.effPos = (SkillArtEffect.EffPos)(int)unit.effPos;
            oUnit.height = unit.height;
            oUnit.phaseTime = unit.phaseTime;
        }
        public static JSkillUnit ConvertSkillUnit(SkillUnit.SkillUnit unit)
        {
            JSkillUnit oUnit = new JSkillUnit();
            oUnit.id = unit.id;
            oUnit.artId = unit.artId;
            oUnit.launchType = (JSkillUnit.LaunchType)(int)unit.launchType;
            oUnit.targetType = (JSkillUnit.TargetType)(int)unit.targetType;
            oUnit.skillTime = unit.skillTime;
            oUnit.cd = unit.cd;
            oUnit.distance = unit.distance;
            oUnit.referId = unit.referId;
            oUnit.guidePolicy = ConvertSkillGuidePolicy(unit.guidePolicy);

            if (oUnit.launchType == JSkillUnit.LaunchType.SINGLELINE)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.singeLines.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillLine line = EditorDataContainer.allSkillUnits.singeLines[unit.referId];
                    oUnit.skillObj = ConvertSkillLine(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.MULLINE)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.multLines.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillMultiLine line = EditorDataContainer.allSkillUnits.multLines[unit.referId];
                    oUnit.skillObj = ConvertSkillMultiLine(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.AREA)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.areas.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillArea line = EditorDataContainer.allSkillUnits.areas[unit.referId];
                    oUnit.skillObj = ConvertSkillArea(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.JUMP)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.jumps.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillJump line = EditorDataContainer.allSkillUnits.jumps[unit.referId];
                    oUnit.skillObj = ConvertSkillJump(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.AREA_RANDSKILL)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.areaRands.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillAreaRand line = EditorDataContainer.allSkillUnits.areaRands[unit.referId];
                    oUnit.skillObj = ConvertSkillAreaRand(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.HELIX)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.helixes.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillHelix line = EditorDataContainer.allSkillUnits.helixes[unit.referId];
                    oUnit.skillObj = ConvertSkillHelix(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.FOLLOW)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.follows.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillFollow line = EditorDataContainer.allSkillUnits.follows[unit.referId];
                    oUnit.skillObj = ConvertSkillFollow(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.BACK_STAB)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.backStabs.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillBackStab line = EditorDataContainer.allSkillUnits.backStabs[unit.referId];
                    oUnit.skillObj = ConvertSkillBackStab(line);
                }
            }
            return oUnit;
        }
        public static void ConvertSkillUnit(JSkillUnit oUnit,SkillUnit.SkillUnit unit)
        {
            oUnit.id = unit.id;
            oUnit.artId = unit.artId;
            oUnit.launchType = (JSkillUnit.LaunchType)(int)unit.launchType;
            oUnit.targetType = (JSkillUnit.TargetType)(int)unit.targetType;
            oUnit.skillTime = unit.skillTime;
            oUnit.cd = unit.cd;
            oUnit.distance = unit.distance;
            oUnit.referId = unit.referId;
            oUnit.guidePolicy=ConvertSkillGuidePolicy(unit.guidePolicy);

            if (oUnit.launchType == JSkillUnit.LaunchType.SINGLELINE)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.singeLines.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillLine line = EditorDataContainer.allSkillUnits.singeLines[unit.referId];
                    oUnit.skillObj = ConvertSkillLine(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.MULLINE)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.multLines.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillMultiLine line = EditorDataContainer.allSkillUnits.multLines[unit.referId];
                    oUnit.skillObj = ConvertSkillMultiLine(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.AREA)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.areas.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillArea line = EditorDataContainer.allSkillUnits.areas[unit.referId];
                    oUnit.skillObj = ConvertSkillArea(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.JUMP)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.jumps.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillJump line = EditorDataContainer.allSkillUnits.jumps[unit.referId];
                    oUnit.skillObj = ConvertSkillJump(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.AREA_RANDSKILL)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.areaRands.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillAreaRand line = EditorDataContainer.allSkillUnits.areaRands[unit.referId];
                    oUnit.skillObj = ConvertSkillAreaRand(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.HELIX)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.helixes.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillHelix line = EditorDataContainer.allSkillUnits.helixes[unit.referId];
                    oUnit.skillObj = ConvertSkillHelix(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.FOLLOW)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.follows.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillFollow line = EditorDataContainer.allSkillUnits.follows[unit.referId];
                    oUnit.skillObj = ConvertSkillFollow(line);
                }
            }
            if (oUnit.launchType == JSkillUnit.LaunchType.BACK_STAB)
            {
                if (unit.referId < EditorDataContainer.allSkillUnits.backStabs.Count && unit.referId >= 0)
                {
                    SkillUnit.SkillBackStab line = EditorDataContainer.allSkillUnits.backStabs[unit.referId];
                    oUnit.skillObj = ConvertSkillBackStab(line);
                }
            }
        }
        public static SkillArt ConvertSkillArt(SkillUnit.SkillArt unit)
        {
            SkillArt oUnit = new SkillArt();
            oUnit.id = unit.id;
            oUnit.guideAction = unit.guideAction;
            oUnit.guideFadeTime = unit.guideFadeTime;
            oUnit.guidingAction = unit.guidingAction;
            oUnit.endAction = unit.endAction;

            oUnit.unitEffect = new List<SkillEffectUnit>();
            oUnit.unitEffect.Add(new SkillEffectUnit());
            oUnit.unitEffect[0].artEffect = ConvertSkillArtEffect(unit.unitEffect);

            oUnit.tipEffect = new List<SkillEffectUnit>();
            oUnit.tipEffect.Add(new SkillEffectUnit());
            oUnit.tipEffect[0].artEffect = ConvertSkillArtEffect(unit.tipEffect);

            oUnit.hitEffect = new List<SkillEffectUnit>();
            oUnit.hitEffect.Add(new SkillEffectUnit());
            oUnit.hitEffect[0].artEffect = ConvertSkillArtEffect(unit.hitEffect);
            

            oUnit.beginEffect = new List<SkillEffectUnit>();
            for (int i = 0; i < unit.beginEffect.Count; i++)
            {
                SkillEffectUnit Effect = new SkillEffectUnit();
                Effect.artEffect = ConvertSkillArtEffect(unit.beginEffect[i]);
                oUnit.beginEffect.Add(Effect);
            }

            return oUnit;
        }

        public static void ConvertSkillArt(SkillArt oUnit, SkillUnit.SkillArt unit)
        {
            oUnit.id = unit.id;
            oUnit.guideAction = unit.guideAction;
            oUnit.guideFadeTime = unit.guideFadeTime;
            oUnit.guidingAction = unit.guidingAction;
            oUnit.endAction = unit.endAction;
            if (oUnit.unitEffect == null)
            {
                oUnit.unitEffect = new List<SkillEffectUnit>();
            }
            if (oUnit.tipEffect == null)
            {
                oUnit.tipEffect = new List<SkillEffectUnit>();
            }
            if (oUnit.hitEffect == null)
            {
                oUnit.hitEffect = new List<SkillEffectUnit>();
            }

            if (oUnit.unitEffect.Count == 0)
            {
                oUnit.unitEffect.Add(new SkillEffectUnit());
            }
            if (oUnit.tipEffect.Count == 0)
            {
                oUnit.tipEffect.Add(new SkillEffectUnit());
            }
            if (oUnit.hitEffect.Count == 0)
            {
                oUnit.hitEffect.Add(new SkillEffectUnit());
            }
            ConvertSkillArtEffect(oUnit.unitEffect[0].artEffect,unit.unitEffect);
            ConvertSkillArtEffect(oUnit.tipEffect[0].artEffect,unit.tipEffect);
            ConvertSkillArtEffect(oUnit.hitEffect[0].artEffect, unit.hitEffect);
            if (oUnit.beginEffect == null)
                oUnit.beginEffect = new List<SkillEffectUnit>();
            oUnit.beginEffect.Clear();
            for (int i = 0; i < unit.beginEffect.Count; i++)
            {
                SkillEffectUnit Effect = new SkillEffectUnit();
                ConvertSkillArtEffect(Effect.artEffect, unit.beginEffect[i]);
                oUnit.beginEffect.Add(Effect);
            }
            
        }
    }

    public class EditorDrawUtility
    {
        //绘制一个popup区域
        public static void DrawActionRect(string title, string inaction, out string content, List<string> stateNamelist)
        {
            EditorGUILayout.BeginHorizontal();
            string guidestr = EditorGUILayout.TextField(title, inaction);
            int gaindex = -1;
            if (stateNamelist.Contains(guidestr))
            {
                content = guidestr;
            }
            else
            {
                content = "-1";
            }
            if (stateNamelist != null && stateNamelist.Contains(content))
                gaindex = stateNamelist.IndexOf(content);
            int newgaindex = EditorGUILayout.Popup("", gaindex, stateNamelist.ToArray());
            if (gaindex != newgaindex)
            {
                content = stateNamelist[newgaindex];
            }
            EditorGUILayout.EndHorizontal();
        }
        //绘制特效的配置
        public static void DrawEffectConfigure(EffectConfigure effect)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("  特效配置：");
            effect.posType = (CySkillEditor.EffectConfigure.PosType)EditorGUILayout.Popup("  posType", (int)effect.posType, Enum.GetNames(typeof(CySkillEditor.EffectConfigure.PosType)));
            effect.effectName = EditorGUILayout.TextField("  effectName", effect.effectName);
            if (effect.posType == CySkillEditor.EffectConfigure.PosType.BODY)
            {
                effect.bodyHeight =  EditorGUILayout.FloatField("  bodyHeight",effect.bodyHeight);
            }
            else
            if (effect.posType == CySkillEditor.EffectConfigure.PosType.HEAD)
            {
                effect.headHeight =  EditorGUILayout.FloatField("  headHeight",  effect.headHeight);
            }
            else
            if (effect.posType == CySkillEditor.EffectConfigure.PosType.BONE)
            {
                effect.boneName = EditorGUILayout.TextField("  boneName", "" + effect.boneName);
            }
            else
            if (effect.posType == CySkillEditor.EffectConfigure.PosType.BODY)
            {
                effect.bodyHeight =  EditorGUILayout.FloatField("  bodyHeight",effect.bodyHeight);
            }
            else
            if (effect.posType == CySkillEditor.EffectConfigure.PosType.FEET)
            {
                effect.feetWidth =  EditorGUILayout.FloatField("  feetWidth",effect.feetWidth);
            }
            effect.position = EditorGUILayout.Vector3Field("  position", effect.position);
            effect.rotation = EditorGUILayout.Vector3Field("  rotation", effect.rotation);
            effect.lifeTime = (CySkillEditor.EffectConfigure.LifeTime)EditorGUILayout.Popup("  lifeTime", (int)effect.lifeTime, Enum.GetNames(typeof(CySkillEditor.EffectConfigure.LifeTime)));
            EditorGUILayout.EndVertical();
        }
        //绘制特效单位
        public static void DrawSkillArtEffect(SkillArtEffect Art)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("  特效：");
            GameObject effect = null;
            if (Art.effectObj != null)
                effect = Art.effectObj;
            Art.effectObj = (GameObject)EditorGUILayout.ObjectField("  unitEffect:", effect, typeof(GameObject), true);
            Art.beginTime = EditorGUILayout.IntField("  beginTime:",  Art.beginTime);
            Art.phaseTime = EditorGUILayout.IntField("  phaseTime:",  Art.phaseTime);
            Art.height = EditorGUILayout.FloatField("  height:", Art.height);
            Art.effPos = (SkillArtEffect.EffPos)EditorGUILayout.Popup("  effPos:", (int)Art.effPos, Enum.GetNames(typeof(SkillArtEffect.EffPos)));
            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillEffectUnit(SkillEffectUnit unit)
        {
            DrawSkillArtEffect(unit.artEffect);
            DrawEffectConfigure(unit.configure);
        }
        public static void DrawSkillShape(string title, SkillShape hitarea)
        {
            int newarea = EditorGUILayout.Popup(title, (int)hitarea.area, Enum.GetNames(typeof(SkillShape.Area)));
            if (newarea != (int)hitarea.area)
            {
                hitarea.area = (SkillShape.Area)newarea;
            }
            string p1 = "";
            string p2 = "";
            string p3 = "";
            if (hitarea.area == SkillShape.Area.QUADRATE)
            {
                p1 = "长"; p2 = "宽";
            }
            if (hitarea.area == SkillShape.Area.CIRCLE)
            {
                p1 = "半径";
            }
            if (hitarea.area == SkillShape.Area.SECTOR)
            {
                p1 = "半径"; p3 = "弧度";
            }
            if (hitarea.area == SkillShape.Area.TRIANGLE)
            {
                p1 = "高"; p2 = "底"; p3 = "1正2反";
            }

            hitarea.param1 = EditorGUILayout.FloatField("  param1:" + p1, hitarea.param1);
            hitarea.param2 = EditorGUILayout.FloatField("  param2:" + p2, hitarea.param2);
            hitarea.param3 = EditorGUILayout.FloatField("  param3:" + p3, hitarea.param3);
        }
        public static void DrawCameraAction(SkillCameraAction action)
        {
            EditorGUILayout.BeginVertical("Box");
            string inType = Enum.GetName(typeof(SkillCameraAction.CameraAction),action.action) ;
            DrawActionRect("类型", inType, out inType, new List<string>(Enum.GetNames(typeof(SkillCameraAction.CameraAction))));
            action.action = (SkillCameraAction.CameraAction)Enum.Parse(typeof(SkillCameraAction.CameraAction), inType);
            if (action.action == SkillCameraAction.CameraAction.CAMERAACTION_SHAKE)
            {
                action.shakeRange = EditorGUILayout.FloatField("  shakeRange:", action.shakeRange);
                action.shakeInterval = EditorGUILayout.IntField("  shakeInterval:", action.shakeInterval);
            }
           
            action.param = EditorGUILayout.FloatField("  param:", action.param);
            action.delay = EditorGUILayout.FloatField("  delay:", action.delay);
            action.phaseTime = EditorGUILayout.FloatField("  phaseTime:", action.phaseTime);

            EditorGUILayout.EndVertical();
        }
        //绘制弹道技能
        public static void DrawSkillSingleLine(SkillLine line)
        {
            EditorGUILayout.BeginVertical();
            line.id = EditorGUILayout.IntField("  id",line.id);
            line.moveTime = EditorGUILayout.IntField("  moveTime", line.moveTime);
            line.speed =EditorGUILayout.FloatField("  speed:", line.speed);
            line.waves = EditorGUILayout.IntField("  waves:",line.waves);
            line.waveDelay = EditorGUILayout.IntField("  waveDelay:", line.waveDelay);
            line.maxInfluence =EditorGUILayout.IntField("  maxInfluence:",line.maxInfluence);
            line.canPierce = EditorGUILayout.Toggle("  canPierce:", line.canPierce);
            line.offset = EditorGUILayout.Vector3Field("  offset:", line.offset);
            SkillShape hitarea = line.hitArea;
            DrawSkillShape("  hitArea:",hitarea);
            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillMultiLine(SkillMultiLine line)
        {
            EditorGUILayout.BeginVertical();
            line.id = EditorGUILayout.IntField("  id", line.id);
            line.moveTime = EditorGUILayout.IntField("  moveTime", line.moveTime);
            line.unitCount =EditorGUILayout.IntField("  unitCount:",line.unitCount);
            line.speed = EditorGUILayout.FloatField("  speed:",line.speed);
            line.waves = EditorGUILayout.IntField("  waves:", line.waves);
            line.waveDelay = EditorGUILayout.IntField("  waveDelay:",  line.waveDelay);
            line.maxInfluence = EditorGUILayout.IntField("  maxInfluence:", line.maxInfluence);
            line.canPierce = EditorGUILayout.Toggle("  canPierce:",line.canPierce);
            line.offset = EditorGUILayout.Vector3Field("  offset:", line.offset);
            SkillShape hitarea = line.hitArea;
            DrawSkillShape("  hitArea:", hitarea);
            SkillShape shape = line.shape;
            DrawSkillShape("  shape:", shape);

            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillArea(SkillArea line)
        {
            EditorGUILayout.BeginVertical();
            line.id = EditorGUILayout.IntField("  id",line.id);
            line.moveDelay = EditorGUILayout.IntField("  moveDelay", line.moveDelay);
            line.waves = EditorGUILayout.IntField("  waves:", line.waves);
            line.waveDelay =  EditorGUILayout.IntField("  waveDelay:",  line.waveDelay);
            line.maxInfluence =  EditorGUILayout.IntField("  maxInfluence:", line.maxInfluence);
            JSkillUnit.BasePoint bp = line.basePoint;
            int newbp = EditorGUILayout.Popup("  basePoint:", (int)bp, Enum.GetNames(typeof(JSkillUnit.BasePoint)));
            if (newbp != (int)bp)
            {
                bp = (JSkillUnit.BasePoint)newbp;
            }
            JSkillUnit.ReferPoint shape = line.referPoint;
            int newshape = EditorGUILayout.Popup("  referPoint:", (int)shape, Enum.GetNames(typeof(JSkillUnit.ReferPoint)));
            if (newshape != (int)shape)
            {
                shape = (JSkillUnit.ReferPoint)newshape;
            }
            SkillShape hitarea = line.hitArea;
            DrawSkillShape("  hitArea:", hitarea);
          
            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillJump(SkillJump line)
        {
            EditorGUILayout.BeginVertical();
            line.id =  EditorGUILayout.IntField("  id",  line.id);
            line.moveTime =  EditorGUILayout.IntField("  moveTime", line.moveTime);
            line.speed =  EditorGUILayout.FloatField("  speed:",  line.speed);
            line.height =  EditorGUILayout.FloatField("  height:",line.height);
            line.maxInfluence =  EditorGUILayout.IntField("  maxInfluence:", line.maxInfluence);
            SkillJump.JumpType bp = line.jumpType;
            int newbp = EditorGUILayout.Popup("  jumpType:", (int)bp, Enum.GetNames(typeof(SkillJump.JumpType)));
            if (newbp != (int)bp)
            {
                bp = (SkillJump.JumpType)newbp;
            }
            SkillShape hitarea = line.hitArea;
            DrawSkillShape("  hitArea:", hitarea);
            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillAreaRandom(SkillAreaRand line)
        {
            EditorGUILayout.BeginVertical();
            line.id =  EditorGUILayout.IntField("  id",line.id);
            line.unitID =  EditorGUILayout.IntField("  unitID", line.unitID);
            line.unitCount =  EditorGUILayout.IntField("  unitCount:",line.unitCount);
            JSkillUnit.BasePoint bp = line.basePoint;
            int newbp = EditorGUILayout.Popup("  basePoint:", (int)bp, Enum.GetNames(typeof(JSkillUnit.BasePoint)));
            if (newbp != (int)bp)
            {
                bp = (JSkillUnit.BasePoint)newbp;
            }
            JSkillUnit.ReferPoint shape = line.referPoint;
            int newshape = EditorGUILayout.Popup("  referPoint:", (int)shape, Enum.GetNames(typeof(JSkillUnit.ReferPoint)));
            if (newshape != (int)shape)
            {
                shape = (JSkillUnit.ReferPoint)newshape;
            }
            SkillShape hitarea = line.area;
            DrawSkillShape("  area:", hitarea);
            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillHelix(SkillHelix line)
        {
            EditorGUILayout.BeginVertical();
            line.id =  EditorGUILayout.IntField("  id", line.id);
            line.moveTime =  EditorGUILayout.IntField("  moveTime",  line.moveTime);
            line.maxRadius =  EditorGUILayout.FloatField("  maxRadius:", line.maxRadius);
            line.maxInfluence =  EditorGUILayout.IntField("  maxInfluence:",line.maxInfluence);
            line.canPierce = EditorGUILayout.Toggle("  canPierce:",  line.canPierce);
            line.offset = EditorGUILayout.Vector3Field("  offset:", line.offset);
            SkillShape hitarea = line.hitArea;
            DrawSkillShape("  hitArea:", hitarea);
            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillFollow(SkillFollow line)
        {
            EditorGUILayout.BeginVertical();
            line.id =  EditorGUILayout.IntField("  id", line.id);
            line.maxFollowTime =  EditorGUILayout.IntField("  maxFollowTime", line.maxFollowTime);
            line.speed =  EditorGUILayout.FloatField("  speed:", line.speed);
            line.waves =  EditorGUILayout.IntField("  waves:",  line.waves);
            line.waveDelay =  EditorGUILayout.IntField("  waveDelay:",line.waveDelay);
            SkillShape hitarea = line.hitArea;
            DrawSkillShape("  hitArea:", hitarea);
            EditorGUILayout.EndVertical();
        }
        public static void DrawSkillBackStab(SkillBackStab line)
        {
            EditorGUILayout.BeginVertical();
            line.id =  EditorGUILayout.IntField("  id", line.id);
            line.moveDelay =  EditorGUILayout.IntField("  moveDelay", line.moveDelay);
            line.maxInfluence =  EditorGUILayout.IntField("  maxInfluence:", line.maxInfluence);
            JSkillUnit.BasePoint bp = line.basePoint;
            int newbp = EditorGUILayout.Popup("  basePoint:", (int)bp, Enum.GetNames(typeof(JSkillUnit.BasePoint)));
            if (newbp != (int)bp)
            {
                bp = (JSkillUnit.BasePoint)newbp;
            }
            JSkillUnit.ReferPoint shape = line.referPoint;
            int newshape = EditorGUILayout.Popup("  referPoint:", (int)shape, Enum.GetNames(typeof(JSkillUnit.ReferPoint)));
            if (newshape != (int)shape)
            {
                shape = (JSkillUnit.ReferPoint)newshape;
            }
            SkillShape hitarea = line.hitArea;
            DrawSkillShape("  hitArea:", hitarea);
            EditorGUILayout.EndVertical();
        }
        //绘制技能信息区域
        public static void DrawSkillUnit(JSkillUnit unit)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Skill unit: 技能信息");
            JSkillUnit.LaunchType lt = unit.launchType;
            int newlt = EditorGUILayout.Popup("  LaunchType:", (int)lt, Enum.GetNames(typeof(JSkillUnit.LaunchType)));
            if (newlt != (int)lt)
            {
                unit.launchType = (JSkillUnit.LaunchType)newlt;
            }
            JSkillUnit.TargetType tt = unit.targetType;
            int newtt = EditorGUILayout.Popup("  TargetType:", (int)tt, Enum.GetNames(typeof(JSkillUnit.TargetType)));
            if (newtt != (int)tt)
            {
                unit.targetType = (JSkillUnit.TargetType)newtt;
            }
            unit.skillTime =  EditorGUILayout.IntField("  skillTime:",  unit.skillTime);
            unit.cd =  EditorGUILayout.IntField("  cd:",  unit.cd);
            unit.distance =  EditorGUILayout.FloatField("  distance:", unit.distance);
            unit.referId =  EditorGUILayout.IntField("  referId:",  unit.referId);
            EditorGUILayout.LabelField("弹道信息:");
            if (unit.skillObj != null)
            {
                if (unit.launchType == JSkillUnit.LaunchType.SINGLELINE)
                {
                    if (!(unit.skillObj is SkillLine))
                    {
                        unit.skillObj = new SkillLine();
                    }
                    DrawSkillSingleLine((SkillLine)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.MULLINE)
                {
                    if (!(unit.skillObj is SkillMultiLine))
                    {
                        unit.skillObj = new SkillMultiLine();
                    }
                    DrawSkillMultiLine((SkillMultiLine)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.AREA)
                {
                    if (!(unit.skillObj is SkillArea))
                    {
                        unit.skillObj = new SkillArea();
                    }
                    DrawSkillArea((SkillArea)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.JUMP)
                {
                    if (!(unit.skillObj is SkillJump))
                    {
                        unit.skillObj = new SkillJump();
                    }
                    DrawSkillJump((SkillJump)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.AREA_RANDSKILL)
                {
                    if (!(unit.skillObj is SkillAreaRand))
                    {
                        unit.skillObj = new SkillAreaRand();
                    }
                    DrawSkillAreaRandom((SkillAreaRand)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.HELIX)
                {
                    if (!(unit.skillObj is SkillHelix))
                    {
                        unit.skillObj = new SkillHelix();
                    }
                    DrawSkillHelix((SkillHelix)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.FOLLOW)
                {
                    if (!(unit.skillObj is SkillFollow))
                    {
                        unit.skillObj = new SkillFollow();
                    }
                    DrawSkillFollow((SkillFollow)unit.skillObj);
                }
                if (unit.launchType == JSkillUnit.LaunchType.BACK_STAB)
                {
                    if (!(unit.skillObj is SkillBackStab))
                    {
                        unit.skillObj = new SkillBackStab();
                    }
                    DrawSkillBackStab((SkillBackStab)unit.skillObj);
                }
            }
            else
            {
                if (GUILayout.Button("Add New Unit:添加弹道"))
                {
                    unit.skillObj = new SkillObj();
                }
            }
            EditorGUILayout.EndVertical();
        }

    }
}