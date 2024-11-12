using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    public bool IsMyTurn = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = ECreatureType.Monster;
        CreatureState = ECreatureState.Idle;

        Managers.Game.GameStateChanged -= OnGameStateChanged;
        Managers.Game.GameStateChanged += OnGameStateChanged;

        StartCoroutine(CoUpdate());
        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);
    }

    void OnGameStateChanged(EGameState gameState)
    {
        if (gameState != EGameState.MonsterTurn)
            return;

        IsMove = false;
        IsSkill = false;
        CreatureState = ECreatureState.Idle;
    }

    protected override void UpdateIdle()
    {
        if (CreatureState == ECreatureState.Idle)
        {
            if (IsMyTurn)
                ExecuteAI();
        }
    }

    protected override void UpdateMove()
    {
        if (CreatureState == ECreatureState.Move)
        {
            Debug.Log("UpdateMove");
            FindPathAndMoveToCellPos(DestPos, Mov, findClosestPos: true);
            CreatureState = ECreatureState.Idle;
        }
    }

    protected override void UpdateSkill()
    {
        if (CreatureState == ECreatureState.Skill && CastingSkill != null)
        {
            Debug.Log("UpdateSkill");
            if (CastingSkill != null)
                CastingSkill.DoSkill();
            CreatureState = ECreatureState.Idle;
        }
    }

    protected override void UpdateDead()
    {
        if (CreatureState == ECreatureState.Dead)
        {
            Debug.Log("UpdateDead");
            // TODO
            // 보상 제공
            Managers.Object.Despawn(this);
        }
    }

    #region AI
    public struct PQTarget : IComparable<PQTarget>  // 타겟 우선순위
    {
        public float Hp;    // 체력
        public float Def;   // 방어력
        public Vector3Int CellPos;  // 위치
        public int Distance;    // 타겟과 몬스터(this)와의 거리

        public int CompareTo(PQTarget other)
        {
            // 각 가중치 설정
            float hpWeight = 0.5f;
            float defWeight = 0.5f;
            float distanceWeight = 1.0f;

            float score = (Hp * hpWeight) + (Def * defWeight) + (Distance * distanceWeight);
            float otherScore = (other.Hp * hpWeight) + (other.Def * defWeight) + (other.Distance * distanceWeight);

            if (score == otherScore)
                return 0;
            return score < otherScore ? 1 : -1; // 최소값 우선순위
        }
    }

    EMonsterBehaviorPattern _behaviorPattern;

    void ExecuteAI()
    {
        _behaviorPattern = EMonsterBehaviorPattern.Aggressive;
        if (Hp <= MaxHp * 0.3 || CreatureData.ClassDataID == MAGE_ID)   // 현재 체력이 30%이하거나 마법사라면
            _behaviorPattern = EMonsterBehaviorPattern.Defensive;

        switch (_behaviorPattern)
        {
            case EMonsterBehaviorPattern.Aggressive:
                HandleAggressivePattern();
                break;
            case EMonsterBehaviorPattern.Defensive:
                HandleDefensivePattern();
                break;
        }

        IsMyTurn = false;
    }

    void HandleAggressivePattern()
    {
        Debug.Log("AggressivePattern");

        // 타겟 우선순위 설정
        PriorityQueue<PQTarget> targets = SetTargetPriority();

        // 타겟이 없는지 확인
        if (targets.Count <= 0)
            return;

        PQSkill castingSkill = FindSkill(targets);
        PQTarget target = targets.Pop();

        // 목적지 설정, 타겟 근처로 이동
        DestPos = Managers.Map.CellToWorld(target.CellPos);
        CreatureState = ECreatureState.Move;

        // 스킬 사용
        if (castingSkill.TargetCount > 0)
        {
            CastingSkill = castingSkill.Skill;

            if (castingSkill.TargetPos != Vector3.zero)
                TargetPos = castingSkill.TargetPos;

            if (castingSkill.Dir != EDir.None)
                CastingSkill.Rotate(dir: castingSkill.Dir);

            CreatureState = ECreatureState.Skill;
        }
    }

    void HandleDefensivePattern()
    {
        Debug.Log("DefensivePattern");

        // 타겟 우선순위 설정
        PriorityQueue<PQTarget> targets = SetTargetPriority();

        // 타겟이 없는지 확인
        if (targets.Count <= 0)
            return;

        // 스킬 사용
        PQSkill castingSkill = FindSkill(targets);
        if (castingSkill.TargetCount > 0)
        {
            CastingSkill = castingSkill.Skill;

            if (castingSkill.TargetPos != Vector3.zero)
                TargetPos = castingSkill.TargetPos;

            if (castingSkill.Dir != EDir.None)
                CastingSkill.Rotate(dir: castingSkill.Dir);

            CreatureState = ECreatureState.Skill;
        }

        // 목적지 설정
        Vector3Int destCellPos = CellPos;
        int maxTotalDistance = 0; 
        SetMovementRange();

        // 주변 타겟과 최대한 거리를 유지
        foreach (var pos in MovementRange)
        {
            Vector3Int cellPos = Managers.Map.WorldToCell(pos);
            int totalDistance = 0; // 모든 유닛과의 총 거리

            // 모든 유닛과의 거리 합산
            foreach (var unit in Managers.Object.PlayerUnits)
            {
                int distance = Mathf.Abs(cellPos.x - unit.CellPos.x) + Mathf.Abs(cellPos.y - unit.CellPos.y); // 맨해튼 거리 계산
                 totalDistance += distance;
            }

            // 총 거리가 최대가 되는 위치를 찾기
            if (totalDistance > maxTotalDistance)
            {
                maxTotalDistance = totalDistance;
                destCellPos = cellPos;
            }
        }

        // 목적지로 이동
        DestPos = Managers.Map.CellToWorld(destCellPos);
        CreatureState = ECreatureState.Move;
    }

    PriorityQueue<PQTarget> SetTargetPriority()
    {
        PriorityQueue<PQTarget> units = new PriorityQueue<PQTarget>();
        foreach (var unit in Managers.Object.PlayerUnits)
        {
            int distance = Mathf.Abs(CellPos.x - unit.CellPos.x) + Mathf.Abs(CellPos.y - unit.CellPos.y);   // 맨해튼 거리 계산
            units.Push(new PQTarget() { Hp = unit.Hp, Def = unit.Def, CellPos = unit.CellPos, Distance = distance });
        }

        return units;
    }

    #region 스킬 찾기
    public struct PQSkill : IComparable<PQSkill>  // 스킬 우선순위
    {
        public SkillBase Skill;

        public int PrioritySum; // 타겟들의 우선순위 합
        public int TargetCount; // 공격 가능한 타겟 수
        public float DamageMultiplier;  // 데미지
        public int ManaCost;    // 마나량

        public EDir Dir;
        public Vector3 TargetPos;

        public int CompareTo(PQSkill other)
        {
            float score = EvaluateSkillScore(this);
            float otherScore = EvaluateSkillScore(other);

            if (score == otherScore)
                return 0;
            return score > otherScore ? 1 : -1; // 최대값 우선순위
        }

        float EvaluateSkillScore(PQSkill skill) // 스킬의 우선순위 점수 계산
        {
            float prioritySumWeight = -1.0f;
            float targetCountWeight = 2.0f;
            float damageMultiplierWeight = 1.5f;
            float manaCostWeight = -0.3f;

            return (skill.PrioritySum * prioritySumWeight) +
                   (skill.TargetCount * targetCountWeight) +
                   (skill.DamageMultiplier * damageMultiplierWeight) +
                   (skill.ManaCost * manaCostWeight);
        }
    }

    PQSkill FindSkill(PriorityQueue<PQTarget> pqTarget)
    {
        Debug.Log("FindSkill");

        List<PQTarget> targets = GetTargetListFromQueue(pqTarget);
        PriorityQueue<PQSkill> pqBestSkill = new PriorityQueue<PQSkill>();   // 가장 효율적인 스킬

        foreach (var skill in Skills.SkillList) // 사용 가능한 스킬 탐색
        {
            if (skill.IsSkillUsable() == false)
                continue;

            PQSkill pqSkill = EvaluateSkill(skill, targets);
            if (pqSkill.TargetCount > 0)
                pqBestSkill.Push(pqSkill);
        }

        return pqBestSkill.Count > 0 ? pqBestSkill.Pop() : default(PQSkill);  // 가장 효율적인 스킬을 찾으면 반환
    }

    List<PQTarget> GetTargetListFromQueue(PriorityQueue<PQTarget> pqTarget)
    {
        List<PQTarget> targets = new List<PQTarget>();
        while (pqTarget.Count > 0)
            targets.Add(pqTarget.Pop());    // 우선순위 큐를 리스트로 옮긴다

        foreach (var target in targets)
            pqTarget.Push(target);  // 복구
        
        return targets;
    }

    PQSkill EvaluateSkill(SkillBase skill, List<PQTarget> targets)
    {
        Data.SkillData data = skill.SkillData;
        
        if (data.Size == null)  // 전 영역 대상 스킬
        {
            skill.SetCastingRange();
            PQSkill skillForAllTargets = EvaluateSkillForAllTargets(skill, targets);
            return skillForAllTargets;
        }
        else if (data.CastingRange != null) // 영역 내 선택 스킬
        {
            skill.SetCastingRange();
            PQSkill skillForPos = EvaluateSkillForPos(skill, targets);
            return skillForPos;
        }
        else if (data.Size != null) // 무제한 범위 스킬
        {
            // TODO: 최적의 방향과 위치 구하기
        }

        return default(PQSkill);
    }

    #region 전 영역 대상 스킬
    PQSkill EvaluateSkillForAllTargets(SkillBase skill, List<PQTarget> targets)
    {
        int prioritySum = 0;
        int targetCount = 0;

        Data.SkillData data = skill.SkillData;

        foreach (var pos in skill.CastingRange) // 캐스팅 범위에서 탐색
        {
            PlayerUnitController target = Managers.Map.GetObject(pos) as PlayerUnitController;
            if (target == null)
                continue;

            targetCount++;

            for (int i = 0; i < targets.Count; i++) // 우선순위 합 구하기
            {
                if (target.CellPos == targets[i].CellPos)   // 타겟 위치를 찾았다
                    prioritySum += i;
            }
        }

        return (new PQSkill()
        {
            Skill = skill,
            PrioritySum = prioritySum,
            TargetCount = targetCount,
            DamageMultiplier = data.DamageMultiplier,
            ManaCost = data.ManaCost,
            Dir = EDir.None,    // 전 영역 대상 스킬이므로 방향은 필요없다
            TargetPos = Vector3.zero    // 마찬가지로 타겟 위치도 필요없다
        });
    }
    #endregion

    #region 영역 내 선택 스킬
    PQSkill EvaluateSkillForPos(SkillBase skill, List<PQTarget> targets)
    {
        PriorityQueue<PQSkill> pqBestPos = new PriorityQueue<PQSkill>();
        Data.SkillData data = skill.SkillData;

        foreach (var pos in skill.CastingRange)     // 스킬 캐스팅 범위에서 탐색
        {
            TargetPos = pos;
            skill.SetSize();

            PriorityQueue<PQSkill> pqBestRotation = new PriorityQueue<PQSkill>();   // 같은 좌표에서 가장 효율적인 스킬 방향

            for (int rotationCount = 0; rotationCount < 4; rotationCount++)   // 스킬 회전
            {
                int prioritySum = 0;    // 공격 가능한 타겟들의 우선순위 큐 인덱스 합
                int targetCount = 0;  // 공격 가능한 타겟 수

                foreach (var spos in skill.Size)    // 공격 가능한 타겟 수와 우선순위 구하기
                {
                    PlayerUnitController target = Managers.Map.GetObject(spos) as PlayerUnitController;
                    if (target == null)
                        continue;

                    targetCount++;

                    for (int i = 0; i < targets.Count; i++)
                    {
                        if (target.CellPos == targets[i].CellPos)   // 타겟 위치를 찾았다
                            prioritySum += i;
                    }
                }

                if (targetCount > 0)  // 공격 가능한 타겟을 찾았다
                {
                    pqBestRotation.Push(new PQSkill()
                    {
                        Skill = skill,
                        PrioritySum = prioritySum,
                        TargetCount = targetCount,
                        DamageMultiplier = data.DamageMultiplier,
                        ManaCost = data.ManaCost,
                        Dir = (EDir)rotationCount,
                        TargetPos = pos,
                    });
                }

                if (data.Size.Count <= 1)  // 스킬 칸이 기본적으로 한 칸이라면 회전할 필요가 없다
                    break;

                skill.Rotate();
            }

            if (pqBestRotation.Count > 0)
                pqBestPos.Push(pqBestRotation.Pop());
        }

        return pqBestPos.Count > 0 ? pqBestPos.Pop() : default(PQSkill);
    }
    #endregion

    #endregion
    #endregion
}