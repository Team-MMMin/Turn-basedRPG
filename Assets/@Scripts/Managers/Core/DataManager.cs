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
    public Dictionary<int, Data.CreatureData> CreatureDataDic { get; private set; } = new Dictionary<int, Data.CreatureData>();

    public void Init()
    {
        LevelDataDic = LoadJson<Data.LevelDataLoader, int, Data.LevelData>("LevelData").MakeDict();
        CreatureDataDic = LoadJson<Data.CreatureDataLoader, int, Data.CreatureData>("CreatureData").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
