using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class SkillBase : InitBase
{
    public CreatureController Owner { get; private set; }

    public Data.SkillData SkillData { get; private set; }
    public HashSet<Vector3> CastingRange { get; private set; } = new HashSet<Vector3>();
    public HashSet<Vector3> Size { get; private set; } = new HashSet<Vector3>();

    EDir _dir = EDir.Up;

    public string Name { get; protected set; }
    
    int level = 0;
    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public bool IsSkillUnlocked { get { return Level > 0; } }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public virtual void SetInfo(CreatureController owner, ESkillID skillID)
    {
        Owner = owner;
        SkillData = Managers.Data.SkillDataDic[(int)skillID];
    }

    public virtual bool DoSkill()
    {
        if (IsSkillUsable())
        {
            Owner.Mp -= SkillData.ManaCost;
            return true;
        }

        return false;
    }

    public bool IsSkillUsable()
    {
        if (IsSkillUnlocked == false)
            return false;
        
        if (Owner.Mp < SkillData.ManaCost)
            return false;

        return true;
    }

    public Data.SkillData UpdateSkillData()
    {
        int skillID = SkillData.DataID;
        if (skillID == 0)
            return SkillData;

        SkillData skillData = new SkillData();
        // TODO: 레벨에 따라 스킬 강화
        if (Managers.Data.SkillDataDic.TryGetValue(skillID, out skillData) == false)    // 강화한 스킬 불러오기
            return SkillData;

        SkillData = skillData;
        return SkillData;
    }

    public virtual void OnChangedSkillData() 
    {

    }

    public virtual void LevelUp()
    {

    }

    public void SetCastingRange()
    {
        ClearCastingRange();
        if (SkillData.CastingRange == null)
            return;

        foreach (Vector3Int delta in SkillData.CastingRange)
        {
            Vector3 pos = Managers.Map.GetTilePos(Owner.transform.position, delta);
            if (Managers.Map.CanGo(pos, true) == false)
                continue;

            CastingRange.Add(pos);
            if (Owner.CreatureType == ECreatureType.PlayerUnit) // 스킬을 사용한 크리처가 플레이어 유닛이라면 범위 시각화
            {
                GameObject go = Managers.Resource.Instantiate("RangeTile", pooling: true);
                go.transform.position = pos;
            }
        }
    }

    public void SetSize()
    {
        ClearSize();

        // 캐스팅 범위 전체에 스킬 적용
        if (SkillData.Size == null && Owner.CreatureType == ECreatureType.PlayerUnit)
        {
            foreach (var pos in CastingRange)
            {
                GameObject go = Managers.Resource.Instantiate("SelectTile", pooling: true);
                go.transform.position = pos;
            }
            
            return;
        }

        // 캐스팅 범위 안에서 스킬 적용
        foreach (Vector3Int delta in SkillData.Size)
        {
            Vector3Int de;
            switch (_dir)
            {
                case EDir.Left:
                    de = new Vector3Int(-delta.y, delta.x);
                    break;
                case EDir.Down:
                    de = new Vector3Int(-delta.x, -delta.y);
                    break;
                case EDir.Right:
                    de = new Vector3Int(delta.y, -delta.x);
                    break;
                default:
                    de = new Vector3Int(delta.x, delta.y);
                    break;
            }

            Vector3 pos = Managers.Map.GetTilePos(Owner.TargetPos, de);

            if (Managers.Map.CanGo(pos, true) == false)
                continue;

            if (SkillData.CastingRange != null && CastingRange.Contains(pos) == false)
                continue;

            Size.Add(pos);
            if (Owner.CreatureType == ECreatureType.PlayerUnit) // 스킬을 사용한 크리처가 플레이어 유닛이라면 범위 시각화
            {
                GameObject go = Managers.Resource.Instantiate("SelectTile", pooling: true);
                go.transform.position = pos;
            }
        }
    }

    public void ClearCastingRange()
    {
        CastingRange.Clear();

        if (Owner.CreatureType == ECreatureType.PlayerUnit) // 스킬을 사용한 크리처가 플레이어 유닛이라면 시각화한 범위를 없앤다
        {
            GameObject go = GameObject.Find("RangeTile");
            if (go == null)
                return;

            foreach (Transform child in go.transform.parent)
            {
                if (child != null && child.gameObject.activeInHierarchy)
                    Managers.Resource.Destroy(child.gameObject);
            }
        }
    }

    public void ClearSize()
    {
        Size.Clear();

        if (Owner.CreatureType == ECreatureType.PlayerUnit) // 스킬을 사용한 크리처가 플레이어 유닛이라면 시각화한 범위를 없앤다
        {
            GameObject go = GameObject.Find("SelectTile");
            if (go == null)
                return;

            foreach (Transform child in go.transform.parent)
            {
                if (child != null && child.gameObject.activeInHierarchy)
                    Managers.Resource.Destroy(child.gameObject);
            }
        }
    }

    public void Rotate(bool isCounterClockwise = true, EDir dir = EDir.None)   // 스킬 모양을 90도씩 회전한다
    {
        if (Size == null)
            return;

        if (dir == EDir.None)
        {
            if (isCounterClockwise)
                _dir = (EDir)(((int)_dir + 1 + 4) % 4);
            else
                _dir = (EDir)(((int)_dir - 1 + 4) % 4);
        }
        else
            _dir = dir;

        SetSize();
    }
}
