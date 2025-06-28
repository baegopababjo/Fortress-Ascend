using UnityEngine;

public class NPC_Stats : MonoBehaviour
{
    public GameStatsData gameStatsData; // ScriptableObject 연결

    public enum NPC_Class
    {
        Warrior, // 🔪 전사
        Archer   // 🏹 궁수
    }

    public float rot_r;

    public int level { get; private set; } = 1;
    public int health { get; private set; }
    public int baseAttackPower { get; private set; }
    public int upgradecost { get; private set; }

    public NPC_Class npcClass { get; private set; }
    public int attackPower { get; private set; }
    public float attackRange { get; private set; }
    public float AttackCooldown { get; private set; }

    private string npcTag; // NPC가 아군인지 적군인지 구별하는 태그

    void Awake()
    {
        gameStatsData.Initialize();
    }

    public void SetLevel(int lev)
    {
        level = lev;
    }

    public void SetNPC(Transform tr, string cw)
    {
        npcTag = tr.tag; // NPC가 아군인지 적군인지 태그 저장

        if (cw.ToLower() == "warrior") { npcClass = NPC_Class.Warrior; rot_r = -0.1f; }
        else if (cw.ToLower() == "archer") { npcClass = NPC_Class.Archer; rot_r = 0.1f; }

        SetLevelStats(level);
        ApplyClassStats();
        // 🎯 스탯 출력 통합 로그
        Debug.Log($"🧩 {npcTag} {npcClass} 설정됨 (Lv.{level})\n💟 체력: {health} | 💪 공격력: {attackPower} (기본: {baseAttackPower}) | 🎯 사거리: {attackRange} | ⏳ 쿨타임: {AttackCooldown}");
    }

    public void SetLevelStats(int newLevel)
    {
        if (npcTag == "Ally")
        {
            (health, baseAttackPower, upgradecost) = gameStatsData.GetAllyLevelStats(newLevel);
        }else if(npcTag == "preview_Ally")
        {
            (health, baseAttackPower, upgradecost) = gameStatsData.GetAllyLevelStats(newLevel);
        }
        else if (npcTag == "Enemy")
        {
            (health, baseAttackPower) = gameStatsData.GetEnemyLevelStats(newLevel);
        }

        level = newLevel;
        //Debug.Log($"🎚️ {npcTag} NPC 레벨 {level}로 설정됨! 💟체력: {health}, 💪기본 공격력: {baseAttackPower}");
    }

    public void ApplyClassStats()
    {
        var (attackBonus, range, cooldown) = gameStatsData.GetNPCClassStats((GameStatsData.NPCClass)npcClass);
        attackPower = baseAttackPower + attackBonus;
        attackRange = range;
        AttackCooldown = cooldown;

        //Debug.Log($"🆕 {npcTag} {npcClass} 설정됨! 💟체력: {health} | 💪공격력: {attackPower} | 🎯 사거리: {attackRange} | ⏳ 쿨타임: {AttackCooldown}");
    }
}
