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
            return score < otherScore ? 1 : -1;
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
        if (castingSkill.AttackableTargetCount > 0)
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
        if (castingSkill.AttackableTargetCount > 0)
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

        public int PrioritySum;
        public int AttackableTargetCount;
        public float DamageMultiplier;
        public int ManaCost;

        public EDir Dir;
        public Vector3 TargetPos;

        public int CompareTo(PQSkill other)
        {
            // 각 가중치 설정
            float prioritySumWeight = -1.0f;
            float attackableTargetCountWeight = 2.0f;
            float damageMultiplierWeight = 1.5f;
            float manaCostWeight = -0.3f;

            float score = (PrioritySum * prioritySumWeight) +
                (AttackableTargetCount * attackableTargetCountWeight) +
                (DamageMultiplier * damageMultiplierWeight) + 
                (ManaCost * manaCostWeight);

            float otherScore = (other.PrioritySum * prioritySumWeight) +
                (other.AttackableTargetCount * attackableTargetCountWeight) +
                (other.DamageMultiplier * damageMultiplierWeight) + 
                (other.ManaCost * manaCostWeight);

            if (score == otherScore)
                return 0;
            return score > otherScore ? 1 : -1;
        }
    }

    PQSkill FindSkill(PriorityQueue<PQTarget> pqTarget)
    {
        Debug.Log("FindSkill");
        
        List<PQTarget> targets = new List<PQTarget>();
        while (pqTarget.Count > 0)
            targets.Add(pqTarget.Pop());

        // 우선순위 큐에 데이터 다시 삽입
        foreach (var target in targets)
            pqTarget.Push(target);  

        PriorityQueue<PQSkill> bestSkill = new PriorityQueue<PQSkill>();   // 가장 효율적인 스킬

        foreach (var skill in Skills.SkillList) // 사용 가능한 스킬 탐색
        {
            if (skill.IsSkillUsable() == false)
                continue;

            int prioritySum = 0;    // 공격 가능한 타겟들의 우선순위 큐 인덱스 합
            int attackableTargetCount = 0;  // 공격 가능한 타겟 수

            Data.SkillData data = skill.SkillData;

            #region 전 영역 대상 스킬
            if (data.Size == null)
            {
                skill.SetCastingRange();
                foreach (var pos in skill.CastingRange)
                {
                    PlayerUnitController playerUnit = Managers.Map.GetObject(pos) as PlayerUnitController;
                    if (playerUnit == null)
                        continue;

                    attackableTargetCount++;

                    for (int i = 0; i < targets.Count; i++)
                    {
                        if (targets[i].CellPos == playerUnit.CellPos)
                            prioritySum += i;
                    }
                }

                if (attackableTargetCount > 0)
                {
                    bestSkill.Push(new PQSkill()
                    {
                        Skill = skill,
                        PrioritySum = prioritySum,
                        AttackableTargetCount = attackableTargetCount,
                        DamageMultiplier = data.DamageMultiplier,
                        ManaCost = data.ManaCost,
                        Dir = EDir.None,    // 방향은 필요없다
                        TargetPos = Vector3.zero    // 타겟 위치도 필요없다
                    });
                }
            }
            #endregion

            #region 영역 내 선택 스킬
            else if (data.CastingRange != null)
            {
                PriorityQueue<PQSkill> best = new PriorityQueue<PQSkill>();
                skill.SetCastingRange();

                foreach (var pos in skill.CastingRange)
                {
                    PriorityQueue<PQSkill> pq = new PriorityQueue<PQSkill>();   // 같은 좌표에서 회전
                    prioritySum = 0;
                    attackableTargetCount = 0;

                    TargetPos = pos;
                    skill.SetSize();
                    
                    for (int rotateCount = 0; rotateCount < 4; rotateCount++)   // 스킬 회전
                    {
                        prioritySum = 0;
                        attackableTargetCount = 0;

                        foreach (var spos in skill.Size)    // 공격 가능한 타겟 수와 우선 순위 인덱스 구하기
                        {
                            PlayerUnitController playerUnit = Managers.Map.GetObject(spos) as PlayerUnitController;
                            if (playerUnit == null)
                                continue;

                            attackableTargetCount++;
                            
                            for (int i = 0; i < targets.Count; i++)
                            {
                                if (targets[i].CellPos == playerUnit.CellPos)
                                    prioritySum += i;
                            }
                        }

                        if (attackableTargetCount > 0)  // 공격 가능한 타겟을 찾았다
                        {
                            pq.Push(new PQSkill()
                            {
                                Skill = skill,
                                PrioritySum = prioritySum,
                                AttackableTargetCount = attackableTargetCount,
                                DamageMultiplier = data.DamageMultiplier,
                                ManaCost = data.ManaCost,
                                Dir = (EDir)rotateCount,
                                TargetPos = pos,
                            });
                        }

                        if (data.Size.Count <= 1)  // 스킬 칸이 기본적으로 한 칸이라면 회전할 필요가 없다
                            break;

                        skill.Rotate();
                    }

                    if (pq.Count > 0)
                        best.Push(pq.Pop());
                }

                if (best.Count > 0)
                    bestSkill.Push(best.Pop());
            }
            #endregion

            #region 무제한 범위 스킬
            else if (data.Size != null)
            {
                // TODO: 최적의 방향과 위치 구하기
            }
            #endregion
        }

        if (bestSkill.Count > 0)   // 가장 효율적인 스킬을 찾으면 반환
            return bestSkill.Pop();

        return default(PQSkill);
    }
    #endregion
    #endregion
}