using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public event System.Action OnStatsInitialized;

    public GameStatsData gameStatsData; // ScriptableObject 연결

    public enum WeaponMode { Sword, Bow }
    public enum MagicMode { EmpoweredAttack, MeleeMagic }

    public int MaxLevel = 5;
    public int gold = 1000;
    public int level = 1;
    public int health;
    public int baseAttackPower;
    public WeaponMode currentWeapon;
    public MagicMode currentMagic;

    void Awake()
    {
        gameStatsData.Initialize();
    }

    void Start()
    {
        SetLevelStats(level);

        // ✅ 객체 이름에 따라 무기 자동 설정
        if (gameObject.name.Contains("Warrior"))
        {
            ChangeWeapon(WeaponMode.Sword);
        }
        else if (gameObject.name.Contains("Archer"))
        {
            ChangeWeapon(WeaponMode.Bow);
        }

        currentMagic = MagicMode.EmpoweredAttack;
    }

    public void SetLevelStats(int newLevel)
    {
        (this.health, baseAttackPower) = gameStatsData.GetPlayerLevelStats(newLevel);
        level = newLevel;
        Debug.Log($"레벨 {level} 설정됨! 💟체력: {health}, 💪공격력: {baseAttackPower}");

        OnStatsInitialized?.Invoke(); // ✅ 초기화 완료 알림
    }

    public void ChangeWeapon(WeaponMode newWeapon)
    {
        currentWeapon = newWeapon;
        //Debug.Log($"🔄 무기 변경: {newWeapon}");
    }

    public void ChangeMagic(MagicMode newMagic)
    {
        currentMagic = newMagic;
        Debug.Log($"✨ 마법 변경: {newMagic}");
    }

    public void AddGold(int amount)
    {
        gold += amount;
        //Debug.Log($"💰 {amount} 골드 추가됨! 현재 골드: {gold}");
    }

    public void Damaged(int att_damage)
    {
        health -= att_damage;
    }
}
