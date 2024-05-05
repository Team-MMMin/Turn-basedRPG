using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region LevelData
    [Serializable]
    public class LevelData
    {
        public int Level;
        public int TotalExp;
        public int RequiredExp;
    }

    [Serializable]
    public class LevelDataLoader : ILoader<int, LevelData>
    {
        public List<LevelData> levels = new List<LevelData>();
        public Dictionary<int, LevelData> MakeDict()
        {
            Dictionary<int, LevelData> dict = new Dictionary<int, LevelData>();
            foreach (LevelData levelData in levels)
                dict.Add(levelData.Level, levelData);
            return dict;
        }
    }
    #endregion

    #region CreatureData
    [Serializable]
    public class CreatureData
    {
        public int DataID;
        public string DescriptionTextID;
        public string PrefabLabel;
        public float MaxHp;
        public float MaxMp;
        public float Atk;
        public float Def;
        public int Mov;
        public float HpRate;
        public float MpRate;
        public float AtkRate;
        public float DefRate;
        public string IconLabel;
        public List<int> SkillTypeList;
    }

    [Serializable]
    public class CreatureDataLoader : ILoader<int, CreatureData> 
    {
        public List<CreatureData> creatures = new List<CreatureData>();
        public Dictionary<int, CreatureData> MakeDict()
        {
            Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
            foreach(CreatureData creature in creatures)
                dict.Add(creature.DataID, creature);
            return dict;
        }
    }
    #endregion
}
