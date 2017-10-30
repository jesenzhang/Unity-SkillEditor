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
    public class EditorDataContainer
    {
        static public string SkillUnitpath = "Assets/SkillEditor/Bytes/SkillUnit.bytes";
        static public string StringIDpath = "Assets/SkillEditor/Bytes/StringID.bytes";
        static public string EffectConfigpath = "Assets/SkillEditor/Bytes/EffectConfig.bytes";

        static public string SkillUnitCopypath = "Assets/SkillEditor/Generate/SkillUnit.bytes";
        static public string StringIDCopypath = "Assets/SkillEditor/Generate/StringID.bytes";
        static public string EffectConfigCopypath = "Assets/SkillEditor/Generate/EffectConfig.bytes";

        //技能表
        static public SkillUnit.AllSkillUnits allSkillUnits = new SkillUnit.AllSkillUnits();
        //stringID 表
        static public StringID.AllStringID allStringID = new StringID.AllStringID();

        static public EffectConfig.AllEffectConfig allEffectConfig = new EffectConfig.AllEffectConfig();

        //当前技能信息
        static public SkillAssetData currentskillAssetData = ScriptableObject.CreateInstance<SkillAssetData>();

        static public SkillUnit.SkillArt CloneSkillArt(SkillUnit.SkillArt a)
        {
            SkillUnit.SkillArt b = new SkillUnit.SkillArt();
            b.beginCameraAction = a.beginCameraAction;
            List<SkillUnit.SkillArtEffect> bl = b.beginEffect;
            for (int i = 0; i < a.beginEffect.Count; i++)
            {
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
            allStringID = null;
            allEffectConfig = null;
        }

        public static void LoadSystemTables()
        {
            byte[] psubys = ReadFileAtPath(SkillUnitpath);
            MemoryStream ms = new MemoryStream(psubys);
            allSkillUnits = ProtoBuf.Serializer.Deserialize<SkillUnit.AllSkillUnits>(ms);
            byte[] pbys = ReadFileAtPath(StringIDpath);
            MemoryStream Phyms = new MemoryStream(pbys);
            allStringID = ProtoBuf.Serializer.Deserialize<StringID.AllStringID>(Phyms);

            byte[] aec = ReadFileAtPath(EffectConfigpath);
            MemoryStream aecms = new MemoryStream(aec);
            allEffectConfig = ProtoBuf.Serializer.Deserialize<EffectConfig.AllEffectConfig>(aecms);
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

        public static void SaveCurrentSkillUnitToList()
        {
            int id = currentskillAssetData.skillUnit.id;
            for (int i = 0; i < allSkillUnits.units.Count; i++)
            {
                if (allSkillUnits.units[i].id == id)
                {
                    DataConvert.ConvertSkillUnit(allSkillUnits.units[i], currentskillAssetData.skillUnit);
                    for (int j = 0; j < allSkillUnits.arts.Count; j++)
                    {
                        if (allSkillUnits.units[i].artId == allSkillUnits.arts[j].id)
                        {
                            allSkillUnits.arts[j] = DataConvert.ConvertSkillArt(EditorDataContainer.currentskillAssetData.skillArt);
                            return;
                        }
                    }
                  return;
                }
            }
           
        }

        public static void SaveSystemTables()
        {
            SaveCurrentSkillUnitToList();
            SaveBytesToPath<StringID.AllStringID>(allStringID, StringIDpath);
            SaveBytesToPath<SkillUnit.AllSkillUnits>(allSkillUnits, SkillUnitpath);
            SaveBytesToPath<EffectConfig.AllEffectConfig>(allEffectConfig, EffectConfigpath);
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
                   /* if (line.LineType() == TimeLineType.Animation)
                    {
                        JTimelineAnimation aniline = (JTimelineAnimation)line;
                        foreach (var track in aniline.AnimationTracks)
                        {
                            if (track.name == SkillNames.ActionNames[(int)SkillNames.GuideAction.GuideAction])
                            {
                                if (track.TrackClips.Count > 0)
                                {
                                    skillart.skillArt.guideAction = track.TrackClips[0].StateName;
                                }
                            }
                            else if (track.name == SkillNames.ActionNames[(int)SkillNames.GuideAction.GuidingAction])
                            {
                                if (track.TrackClips.Count > 0)
                                {
                                    skillart.skillArt.guidingAction = track.TrackClips[0].StateName;
                                }
                            }
                            else if (track.name == SkillNames.ActionNames[(int)SkillNames.GuideAction.EndAction])
                            {
                                if (track.TrackClips.Count > 0)
                                {
                                    skillart.skillArt.endAction = track.TrackClips[0].StateName;
                                }
                            }
                        }
                    }*/
                    //特效时间线 begineffect
                    if (line.LineType() == TimeLineType.Effect)
                    {
                        JTimelineEffect aniline = (JTimelineEffect)line;
                        foreach (var track in aniline.EffectTracks)
                        {

                            if (track.name == SkillNames.EffectNames[(int)SkillNames.Effect.GuideAction])
                            {
                                if (track.TrackClips.Count > 0)
                                {
                                    skillart.skillArt.guideAction = track.TrackClips[0].StateName;
                                }

                            }
                            if (track.name == SkillNames.EffectNames[(int)SkillNames.Effect.GuidingAction])
                            {
                                if (track.TrackClips.Count > 0)
                                {
                                    skillart.skillArt.guidingAction = track.TrackClips[0].StateName;
                                }

                            }
                            if (track.name == SkillNames.EffectNames[(int)SkillNames.Effect.EndAction])
                            {
                                if (track.TrackClips.Count > 0)
                                {
                                    skillart.skillArt.endAction = track.TrackClips[0].StateName;
                                }

                            }

                            if (track.name == SkillNames.EffectNames[(int)SkillNames.Effect.BeginEffect0] || track.name == SkillNames.EffectNames[(int)SkillNames.Effect.BeginEffect1])
                            {
                                if (skillart.skillArt.beginEffect == null)
                                    skillart.skillArt.beginEffect = new List<SkillEffectUnit>();
                                skillart.skillArt.beginEffect.Add(track.TrackClips[0].effectunit.Copy());
                            }
                           

                            if (track.name == SkillNames.EffectNames[(int)SkillNames.Effect.EndEffect])
                            {
                                if (skillart.skillArt.endEffect == null)
                                    skillart.skillArt.endEffect = new List<SkillEffectUnit>();
                                skillart.skillArt.endEffect.Add(track.TrackClips[0].effectunit.Copy());

                            }
                            if (track.name == SkillNames.EffectNames[(int)SkillNames.Effect.UnitEffect])
                            {
                                if (skillart.skillArt.unitEffect == null)
                                    skillart.skillArt.unitEffect = new List<SkillEffectUnit>();
                                if (skillart.skillUnit == null)
                                    skillart.skillUnit = new JSkillUnit();
                                skillart.skillArt.unitEffect.Add(track.TrackClips[0].effectunit.Copy());
                                skillart.skillUnit = track.TrackClips[0].skillunit.Copy();

                            }
                            if (track.name == SkillNames.EffectNames[(int)SkillNames.Effect.HitEffect])
                            {
                                if (skillart.skillArt.hitEffect == null)
                                    skillart.skillArt.hitEffect = new List<SkillEffectUnit>();
                                skillart.skillArt.hitEffect.Add(track.TrackClips[0].effectunit.Copy());
                            }
                            if (track.name == SkillNames.EffectNames[(int)SkillNames.Effect.TipEffect])
                            {
                                if (skillart.skillArt.tipEffect == null)
                                    skillart.skillArt.tipEffect = new List<SkillEffectUnit>();
                                skillart.skillArt.tipEffect.Add(track.TrackClips[0].effectunit.Copy());
                            }
                            if (track.name == SkillNames.EffectNames[(int)SkillNames.Effect.BeginCameraAction])
                            {
                                if (skillart.skillArt.beginCameraAction == null)
                                    skillart.skillArt.beginCameraAction = new List<SkillCameraAction>();
                                skillart.skillArt.beginCameraAction.Add(track.TrackClips[0].cameraAction.Copy());
                            }
                            if (track.name == SkillNames.EffectNames[(int)SkillNames.Effect.MoveCameraAction])
                            {
                                if (skillart.skillArt.moveCameraAction == null)
                                    skillart.skillArt.moveCameraAction = new List<SkillCameraAction>();
                                skillart.skillArt.moveCameraAction.Add(track.TrackClips[0].cameraAction.Copy());
                            }
                            if (track.name == SkillNames.EffectNames[(int)SkillNames.Effect.HitCameraAction])
                            {
                                if (skillart.skillArt.hitCameraAction == null)
                                    skillart.skillArt.hitCameraAction = new List<SkillCameraAction>();
                                skillart.skillArt.hitCameraAction.Add(track.TrackClips[0].cameraAction.Copy());
                            }

                          
                        }
                    }
                    if (skillart.skillArt.beginEffect != null)
                    {
                        skillart.skillArt.beginEffect.Sort(delegate (SkillEffectUnit x, SkillEffectUnit y)
                      {
                          return x.artEffect.beginTime.CompareTo(y.artEffect.beginTime);
                      });
                    }

                 
                    /*
                        if (line.LineType() == TimeLineType.Particle)
                        {
                            JTimelineParticle aniline = (JTimelineParticle)line;
                            foreach (var track in aniline.ParticleTracks)
                            {
                                if (track.name == SkillNames.EffectNames[(int)SkillNames.Effect.BeginEffect0]|| track.name == SkillNames.EffectNames[(int)SkillNames.Effect.BeginEffect1]|| track.name == SkillNames.EffectNames[(int)SkillNames.Effect.EndEffect])
                                {
                                    if (track.TrackClips.Count > 0)
                                    {
                                        SkillEffectUnit unit = new SkillEffectUnit();
                                        unit.configure = track.TrackClips[0].EffectConfig.Copy();
                                        unit.artEffect.beginTime = (int)(track.TrackClips[0].StartTime*1000f);
                                        unit.artEffect.phaseTime = (int)(track.TrackClips[0].PlaybackDuration * 1000f);
                                        unit.artEffect.effectObj = track.TrackClips[0].Effect;
                                        unit.artEffect.effect = EditorDataContainer.GetIdByString(track.TrackClips[0].ParticleName);
                                        begineffect.Insert(0, unit);
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
                                if (track.name == SkillNames.EffectNames[(int)SkillNames.Effect.UnitEffect])
                                {
                                    if (track.TrackClips.Count > 0)
                                    {
                                        skillart.skillUnit = track.TrackClips[0].skillunit.Copy();
                                        unitEffect.Add(track.TrackClips[0].effectunit.Copy());
                                    }
                                }
                            }
                        }
                        */
                }

            }
            Debug.Log(skillart.skillArt.beginEffect.Count);

            return skillart;
        }
        public static SkillAssetData GenerateEmptySkillData(GameObject model,ModelTargetType modelType,string animationControl)
        {
            SkillAssetData data = new SkillAssetData();
            data.skillArt = new SkillArt();
            data.skillArt.model = model;
            data.skillArt.modelName = model.name;
            data.skillArt.modelType = modelType;
            data.skillArt.animationController = animationControl;
            data.skillArt.beginEffect = new List<SkillEffectUnit>();
            data.skillArt.beginEffect.Add(new SkillEffectUnit());
            data.skillArt.beginEffect.Add(new SkillEffectUnit());
            data.skillArt.endEffect = new List<SkillEffectUnit>();
            data.skillArt.endEffect.Add(new SkillEffectUnit());
            data.skillArt.unitEffect = new List<SkillEffectUnit>();
            data.skillArt.unitEffect.Add(new SkillEffectUnit());
            data.skillArt.tipEffect = new List<SkillEffectUnit>();
            data.skillArt.tipEffect.Add(new SkillEffectUnit());
            data.skillArt.hitEffect = new List<SkillEffectUnit>();
            data.skillArt.hitEffect.Add(new SkillEffectUnit());

            data.skillArt.beginCameraAction = new List<SkillCameraAction>();
            data.skillArt.beginCameraAction.Add(new SkillCameraAction());

            data.skillArt.moveCameraAction = new List<SkillCameraAction>();
            data.skillArt.moveCameraAction.Add(new SkillCameraAction());

            data.skillArt.hitCameraAction = new List<SkillCameraAction>();
            data.skillArt.hitCameraAction.Add(new SkillCameraAction());

            data.skillUnit = new JSkillUnit();
            return data;
        }
        public static void CreateEmptySkill(string SkillId, GameObject PlayerModel, ModelTargetType modelType, RuntimeAnimatorController controll)
        {
            string ModelName = PlayerModel.name;

            GameObject ShowRoot = new GameObject("ShowRoot_" + SkillId);
            //获取模型 实例化
            GameObject Player = null;
            if (PlayerModel != null)
            {
                Player = UnityEngine.Object.Instantiate(PlayerModel) as GameObject;
                Player.name = ModelName;
                Player.transform.parent = ShowRoot.transform;
            }
            else
            {
                Player = new GameObject(ModelName);
                Player.transform.parent = ShowRoot.transform;
            }
            //获取animator 赋值
            if (controll != null)
            {
                if (Player.GetComponent<Animator>() == null)
                    Player.AddComponent<Animator>();
                Player.GetComponent<Animator>().runtimeAnimatorController = controll;
            }

            GameObject newSequence = new GameObject("Sequence_" + ModelName);
            JSequencer sequence = newSequence.AddComponent<JSequencer>();
            JTimelineContainer ccontainer = sequence.CreateNewTimelineContainer(Player.transform);
            JTimelineAnimation aniline = (JTimelineAnimation)ccontainer.AddNewTimeline(TimeLineType.Animation);

         /*   foreach (SkillNames.GuideAction gui in Enum.GetValues(typeof(SkillNames.GuideAction)))
            {
                JAnimationTrack track = aniline.AddNewTrack();
                track.name = SkillNames.ActionNames[(int)gui];
            }*/
            JTimelineEffect effectline = (JTimelineEffect)ccontainer.AddNewTimeline(TimeLineType.Effect);
            foreach (SkillNames.Effect gui in Enum.GetValues(typeof(SkillNames.Effect)))
            {
                JEffectTrack track = effectline.AddNewTrack();
                track.name = SkillNames.EffectNames[(int)gui];
            }
           /* JTimelineSound soundline = (JTimelineSound)ccontainer.AddNewTimeline(TimeLineType.Sound);
            foreach (SkillNames.Sound gui in Enum.GetValues(typeof(SkillNames.Sound)))
            {
                JSoundTrack track = soundline.AddNewTrack();
                track.name = SkillNames.SoundNames[(int)gui];
            }*/
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
            RuntimeAnimatorController animationControllerObj = skillart.skillArt.animationControllerObj;
            //模型类型 主角  NPC 
            ModelTargetType modelType = skillart.skillArt.modelType;
            //技能列表
            List<SkillEffectUnit> beginEffect = skillart.skillArt.beginEffect;
            //技能列表
            List<SkillEffectUnit> unitEffect = skillart.skillArt.unitEffect;
            //技能列表
            List<SkillEffectUnit> tipEffect = skillart.skillArt.tipEffect;
            //技能列表
            List<SkillEffectUnit> hitEffect = skillart.skillArt.hitEffect;

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
            else
            {
                Player = new GameObject(ModelName);
                Player.transform.parent = ShowRoot.transform;
            }
            //获取animator 赋值
            RuntimeAnimatorController controll = animationControllerObj;// AssetUtility.GetAnimationCtl(modelType, ModelName, animatorName);
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

            JTimelineEffect effectline = (JTimelineEffect)ccontainer.AddNewTimeline(TimeLineType.Effect);

            if (guideAction != "-1")
            {
                JEffectTrack track = effectline.AddNewTrack();
                track.name = SkillNames.EffectNames[(int)SkillNames.Effect.GuideAction];
                JEffectClipData clip = ScriptableObject.CreateInstance<JEffectClipData>();
                clip.effectType = EffectType.Animation;
                clip.TargetObject = effectline.AffectedObject.gameObject;
                clip.animController = controll;
                clip.StateName = guideAction;
                clip.PlaybackDuration = MecanimAnimationUtility.GetStateDurationWithAnimatorController(guideAction, controll);
                clip.StartTime =0;
                track.AddClip(clip);

            }
            if (guidingAction != "-1")
            {
                JEffectTrack track = effectline.AddNewTrack();
                track.name = SkillNames.EffectNames[(int)SkillNames.Effect.GuidingAction];
                JEffectClipData clip = ScriptableObject.CreateInstance<JEffectClipData>();
                clip.effectType = EffectType.Animation;
                clip.TargetObject = effectline.AffectedObject.gameObject;
                clip.animController = controll;
                clip.StateName = guidingAction;
                clip.PlaybackDuration = MecanimAnimationUtility.GetStateDurationWithAnimatorController(guidingAction, controll);
                float t = guidingAction == "-1" ? 0 : MecanimAnimationUtility.GetStateDurationWithAnimatorController(guideAction, controll);
                clip.StartTime = t;
                track.AddClip(clip);
            }
            if (endAction != "-1")
            {
                JEffectTrack track = effectline.AddNewTrack();
                track.name = SkillNames.EffectNames[(int)SkillNames.Effect.EndAction];
                float t1 = guideAction == "-1" ? 0 : MecanimAnimationUtility.GetStateDurationWithAnimatorController(guideAction, controll);
                float t = guidingAction == "-1" ? 0 : MecanimAnimationUtility.GetStateDurationWithAnimatorController(guidingAction, controll);
                JEffectClipData clip = ScriptableObject.CreateInstance<JEffectClipData>();
                clip.effectType = EffectType.Animation;
                clip.TargetObject = effectline.AffectedObject.gameObject;
                clip.animController = controll;
                clip.StateName = endAction;
                clip.PlaybackDuration = MecanimAnimationUtility.GetStateDurationWithAnimatorController(endAction, controll);
                clip.StartTime = t1+t;
                track.AddClip(clip);
                 
            }

            if (beginEffect != null)
            {
                //一个特效 一条轨道
                foreach (SkillEffectUnit effect in beginEffect)
                {
                    //特效名称
                    JEffectTrack track = effectline.AddNewTrack();
                    int index = beginEffect.IndexOf(effect);
                    if (index > 2)
                        index = 2;
                    track.name = SkillNames.EffectNames[(int)SkillNames.Effect.BeginEffect0 + index];
                    JEffectClipData clip = ScriptableObject.CreateInstance<JEffectClipData>();
                    clip.effectType = EffectType.Particle;
                    clip.effectunit = effect.Copy();
                    clip.TargetObject = effectline.AffectedObject.gameObject;
                    clip.PlaybackDuration = (float)effect.artEffect.phaseTime / 1000.0f;
                    clip.StartTime = (float)effect.artEffect.beginTime / 1000.0f;
                    track.AddClip(clip);
                }
            }
            if (unitEffect != null)
            {
                //一个特效 一条轨道
                foreach (SkillEffectUnit effect in unitEffect)
                {
                    //特效名称
                    JEffectTrack track = effectline.AddNewTrack();
                    track.name = SkillNames.EffectNames[(int)SkillNames.Effect.UnitEffect];
                    JEffectClipData clip = ScriptableObject.CreateInstance<JEffectClipData>();
                    clip.effectType = EffectType.Trajectory;
                    clip.effectunit = effect.Copy();
                    clip.skillunit = skillart.skillUnit.Copy();
                    clip.TargetObject = effectline.AffectedObject.gameObject;
                    clip.PlaybackDuration = (float)effect.artEffect.phaseTime / 1000.0f;
                    clip.StartTime = (float)effect.artEffect.beginTime / 1000.0f;
                    track.AddClip(clip);
                }
            }
            if (tipEffect != null)
            {
                //一个特效 一条轨道
                foreach (SkillEffectUnit effect in tipEffect)
                {
                    //特效名称
                    JEffectTrack track = effectline.AddNewTrack();
                    track.name = SkillNames.EffectNames[(int)SkillNames.Effect.TipEffect];
                    JEffectClipData clip = ScriptableObject.CreateInstance<JEffectClipData>();
                    clip.effectType = EffectType.Particle;
                    clip.effectunit = effect.Copy();
                    clip.TargetObject = effectline.AffectedObject.gameObject;
                    clip.PlaybackDuration = (float)effect.artEffect.phaseTime / 1000.0f;
                    clip.StartTime = (float)effect.artEffect.beginTime / 1000.0f;
                    track.AddClip(clip);
                }
            }
            if (hitEffect != null)
            {
                //一个特效 一条轨道
                foreach (SkillEffectUnit effect in hitEffect)
                {
                    //特效名称
                    JEffectTrack track = effectline.AddNewTrack();
                    track.name = SkillNames.EffectNames[(int)SkillNames.Effect.HitEffect];
                    JEffectClipData clip = ScriptableObject.CreateInstance<JEffectClipData>();
                    clip.effectType = EffectType.Particle;
                    clip.effectunit = effect.Copy();
                    clip.TargetObject = effectline.AffectedObject.gameObject;
                    clip.PlaybackDuration = (float)effect.artEffect.phaseTime / 1000.0f;
                    clip.StartTime = (float)effect.artEffect.beginTime / 1000.0f;
                    track.AddClip(clip);
                    //    track.AddClip(effectName, (float)effect.artEffect.beginTime / 1000.0f, (float)effect.artEffect.phaseTime / 1000.0f, effectPrefab, effect.configure);
                }
            }
            if (skillart.skillArt.beginCameraAction != null)
            {
                //一个特效 一条轨道
                foreach (SkillCameraAction effect in skillart.skillArt.beginCameraAction)
                {
                    JEffectTrack track = effectline.AddNewTrack();
                    track.name = SkillNames.EffectNames[(int)SkillNames.Effect.BeginCameraAction];
                    JEffectClipData clip = ScriptableObject.CreateInstance<JEffectClipData>();
                    clip.effectType = EffectType.Camera;
                    clip.cameraAction = effect.Copy();
                    clip.TargetCamera = Camera.main;
                    clip.TargetObject = effectline.AffectedObject.gameObject;
                    clip.PlaybackDuration = (float)effect.phaseTime;
                    clip.StartTime = (float)effect.delay;
                    track.AddClip(clip);
                }

            }
            if (skillart.skillArt.moveCameraAction != null)
            {
                foreach (SkillCameraAction effect in skillart.skillArt.moveCameraAction)
                {
                    JEffectTrack track = effectline.AddNewTrack();
                    track.name = SkillNames.EffectNames[(int)SkillNames.Effect.MoveCameraAction];
                    JEffectClipData clip = ScriptableObject.CreateInstance<JEffectClipData>();
                    clip.effectType = EffectType.Camera;
                    clip.cameraAction = effect.Copy();
                    clip.TargetCamera = Camera.main;
                    clip.TargetObject = effectline.AffectedObject.gameObject;
                    clip.PlaybackDuration = (float)effect.phaseTime;
                    clip.StartTime = (float)effect.delay;
                    track.AddClip(clip);
                }

            }
            if (skillart.skillArt.hitCameraAction != null)
            {
                foreach (SkillCameraAction effect in skillart.skillArt.hitCameraAction)
                {
                    JEffectTrack track = effectline.AddNewTrack();
                    track.name = SkillNames.EffectNames[(int)SkillNames.Effect.HitCameraAction];
                    JEffectClipData clip = ScriptableObject.CreateInstance<JEffectClipData>();
                    clip.effectType = EffectType.Camera;
                    clip.cameraAction = effect.Copy();
                    clip.TargetCamera = Camera.main;
                    clip.TargetObject = effectline.AffectedObject.gameObject;
                    clip.PlaybackDuration = (float)effect.phaseTime;
                    clip.StartTime = (float)effect.delay;
                    track.AddClip(clip);
                }
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

}