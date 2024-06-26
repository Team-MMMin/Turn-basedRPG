using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

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

    #region ClassData
    [Serializable]
    public class ClassData
    {
        public int DataID;
        public string Name;
        public string PrefabLabel;
        public int Mov;
        public float HpRate;
        public float MpRate;
        public float AtkRate;
        public float DefRate;
        public string IconLabel;
        public List<int> SkillIDList;
    }

    [Serializable]
    public class ClassDataLoader : ILoader<int, ClassData>
    {
        public List<ClassData> classes = new List<ClassData>();
        public Dictionary<int, ClassData> MakeDict()
        {
            Dictionary<int, ClassData> dict = new Dictionary<int, ClassData>();
            foreach (ClassData classData in classes)
                dict.Add(classData.DataID, classData);
            return dict;
        }
    }
    #endregion

    #region CreatureData
    [Serializable]
    public class CreatureData
    {
        public int DataID;
        public string Name;
        public string Description;
        public int ClassDataID;
        public int Level;
        public float Hp;
        public float Mp;
        public float Atk;
        public float Def;
    }

    [Serializable]
    public class CreatureDataLoader : ILoader<int, CreatureData>
    {
        public List<CreatureData> creatures = new List<CreatureData>();
        public Dictionary<int, CreatureData> MakeDict()
        {
            Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
            foreach (CreatureData creature in creatures)
                dict.Add(creature.DataID, creature);
            return dict;
        }
    }
    #endregion

    #region PlayerUnitData
    [Serializable]
    public class PlayerUnitData : CreatureData
    { }

    [Serializable]
    public class PlayerUnitDataLoader : ILoader<int, PlayerUnitData>
    {
        public List<PlayerUnitData> playerUnits = new List<PlayerUnitData>();
        public Dictionary<int, PlayerUnitData> MakeDict() 
        {
            Dictionary<int, PlayerUnitData> dict = new Dictionary<int, PlayerUnitData>();
            foreach (PlayerUnitData unit in playerUnits)
                dict.Add(unit.DataID, unit);
            return dict;
        }
    }
    #endregion

    #region MonsterData
    [Serializable]
    public class MonsterData : CreatureData
    { }

    [Serializable]
    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new List<MonsterData>();
        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters)
                dict.Add(monster.DataID, monster);
            return dict;
        }
    }
    #endregion

    #region SkillData
    [Serializable]
    public class SkillData
    {
        public int DataID;
        public string Name;
        public string Description;
        public string PrefabLabel;
        public string IconLabel;
        public float DamageMultiplier;    // 데미지 배율
        public int CoolTime;
        public int ManaCost;
        public List<Vector2Int> CastingRange;   // 캐스팅(시전) 범위
        public List<Vector2Int> SkillSize;  // 스킬의 크기를 나타내며, null일 때는 캐스팅(시전) 범위 내의 모든 적을 공격
    }

    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();
        public Dictionary<int, SkillData> MakeDict() 
        {
            Dictionary<int, SkillData> dic = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
                dic.Add(skill.DataID, skill);
            return dic;
        }
    }
    #endregion
}