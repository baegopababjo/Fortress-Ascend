using UnityEngine;
using System.Collections;

public class Player_Weapon_Bow : MonoBehaviour
{
    public GameObject arrowPrefab;          // 🔥 화살 Prefab
    public Transform arrowSpawnPoint;       // 🔥 화살 생성 위치
    public float arrowSpeed = 20f;          // 🔥 화살 기본 속도
    public float delayBeforeShoot = 1.3f;   // 🔥 화살 발사 딜레이
    public float upwardForce = 3f;          // 🔥 위쪽 방향으로 추가 힘을 가하는 값

    private PlayerStats playerStats;        // 🔥 플레이어 스탯 참조

    void Start()
    {
        playerStats = GetComponentInParent<PlayerStats>(); // 🔥 플레이어 스탯 가져오기
    }

    public void ShootArrow()
    {
        if (arrowPrefab == null || arrowSpawnPoint == null || playerStats == null) return;
        StartCoroutine(ShootArrowWithDelay()); // 🔥 1.3초 뒤에 화살 발사
    }

    IEnumerator ShootArrowWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeShoot); // ⏳ 1.3초 기다림

        // 🔥 플레이어 스탯에서 활 공격력 가져오기
        var (bowDamage, _, _) = playerStats.gameStatsData.GetWeaponStats(PlayerStats.WeaponMode.Bow);
        int totalDamage = playerStats.baseAttackPower + bowDamage;

        // 🔥 화살 생성
        GameObject arrowInstance = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

        // 🔥 화살의 데미지 설정
        Arrow arrowComponent = arrowInstance.GetComponent<Arrow>();
        if (arrowComponent != null)
        {
            arrowComponent.SetDamage(totalDamage); // 플레이어 공격력 반영
        }

        // 🔥 Rigidbody 가져오기
        Rigidbody arrowRb = arrowInstance.GetComponent<Rigidbody>();
        if (arrowRb != null)
        {
            Vector3 shootDirection = arrowSpawnPoint.forward; // 현재 활의 방향으로 발사
            Vector3 finalForce = (shootDirection * arrowSpeed) + (Vector3.up * upwardForce); // 🔥 Y축 방향으로 추가 힘 적용

            arrowRb.AddForce(finalForce, ForceMode.Impulse); // 🔥 즉각적인 힘 적용 (포물선 궤적)

            // 🔥 화살 방향을 조정하여 올바르게 정렬
            arrowInstance.transform.rotation = Quaternion.LookRotation(finalForce) * Quaternion.Euler(90, 0, 0);
        }
    }
}
