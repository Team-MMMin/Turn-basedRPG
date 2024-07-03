using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.LevelData> LevelDataDic { get; private set; } = new Dictionary<int, Data.LevelData>();
    public Dictionary<int, Data.ClassData> ClassDataDic { get; private set; } = new Dictionary<int, Data.ClassData>();
    public Dictionary<int, Data.PlayerUnitData> PlayerUnitDataDic { get; private set; } = new Dictionary<int, Data.PlayerUnitData>();
    public Dictionary<int, Data.MonsterData> MonsterDataDic { get; private set; } = new Dictionary<int, Data.MonsterData>();
    public Dictionary<int, Data.SkillData> SkillDataDic { get; private set; } = new Dictionary<int, Data.SkillData>();
    public Dictionary<int, Data.TileData> TileDataDic { get; private set; } = new Dictionary<int, Data.TileData>();

    public void Init()
    {
        LevelDataDic = LoadJson<Data.LevelDataLoader, int, Data.LevelData>("LevelData").MakeDict();
        ClassDataDic = LoadJson<Data.ClassDataLoader, int, Data.ClassData>("ClassData").MakeDict();
        PlayerUnitDataDic = LoadJson<Data.PlayerUnitDataLoader, int, Data.PlayerUnitData>("PlayerUnitData").MakeDict();
        MonsterDataDic = LoadJson<Data.MonsterDataLoader, int, Data.MonsterData>("MonsterData").MakeDict();
        SkillDataDic = LoadJson<Data.SkillDataLoader, int, Data.SkillData>("SkillData").MakeDict();
        TileDataDic = LoadJson<Data.TileDataLoader, int, Data.TileData>("TileData").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
