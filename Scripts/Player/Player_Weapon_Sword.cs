using UnityEngine;
using System.Collections.Generic;

public class Player_Weapon_Sword : MonoBehaviour
{
    private Collider weaponCollider;
    private PlayerStats playerStats;
    private bool isEmpoweredAttack = false; // 🔥 강화 공격 여부
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); // ✅ 중복 피격 방지

    void Start()
    {
        weaponCollider = GetComponent<Collider>();
        weaponCollider.isTrigger = true;
        weaponCollider.enabled = false;

        playerStats = GetComponentInParent<PlayerStats>(); // 🔥 PlayerStats 가져오기
    }

    public void EnableWeapon(bool empowered = false)
    {
        weaponCollider.enabled = true; // ✅ Collider 활성화 (공격 시작)
        isEmpoweredAttack = empowered;
        hitObjects.Clear(); // ✅ 새 공격 시작 시 중복 감지 초기화
    }

    public void DisableWeapon()
    {
        weaponCollider.enabled = false; // ✅ Collider 비활성화 (공격 종료)
        isEmpoweredAttack = false;
        hitObjects.Clear(); // ✅ 중복 감지 초기화
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitObjects.Contains(other.gameObject)) return; // ✅ 중복 공격 방지
        hitObjects.Add(other.gameObject);

        //Debug.Log($"⚔️ 무기 충돌 감지! {other.gameObject.name}, 태그: {other.tag}, 레이어: {LayerMask.LayerToName(other.gameObject.layer)}");

        // 🔥 적 NPC 공격
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy_NPC"))
        {
            NPC_AI enemyAI = other.GetComponent<NPC_AI>();

            if (enemyAI != null && playerStats != null)
            {
                int totalDamage = CalculateDamage();
                enemyAI.HitEnemy(totalDamage);
            }
        }

        // 🔥 빌딩 공격 (임시로 플레이어에게 둠)
        if (other.gameObject.layer == LayerMask.NameToLayer("Building"))
        {
            BuildingDamageHandler building = other.GetComponent<BuildingDamageHandler>();

            if (building != null && playerStats != null)
            {
                int totalDamage = CalculateDamage();
                building.TakeDamage(totalDamage);
            }
        }
    }

    // ✅ 플레이어의 공격력 계산 함수
    private int CalculateDamage()
    {
        var (weaponDamage, _, _) = playerStats.gameStatsData.GetWeaponStats(playerStats.currentWeapon);
        int totalDamage = playerStats.baseAttackPower + weaponDamage;

        if (isEmpoweredAttack)
        {
            totalDamage += playerStats.gameStatsData.GetMagicStats(PlayerStats.MagicMode.EmpoweredAttack).bonusDamage;
            Debug.Log("🔥 강화 공격이 적중했습니다! 추가 데미지 적용!");
        }

        return totalDamage;
    }
}
