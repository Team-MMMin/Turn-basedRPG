using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[Serializable]
public class PlayerStat
{
    public int DataId;
    public string Name;
    public float Hp;
    public float Mp;
    public float Akt;
    public float Def;
    public int Mov;
}

public class PlayerController : CreatureController
{
    [SerializeField]
    public PlayerStat Stat = new PlayerStat();
}
