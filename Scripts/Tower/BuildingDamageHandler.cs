using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using NUnit.Framework;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

public class BuildingDamageHandler : MonoBehaviour
{
    public GameStatsData gameStatsData; // 인스펙터에서 연결해줄 것

    private string buildingName;
    private int buildingLevel = 1;
    private int currentHealth;

    private Rigidbody rb;
    private HashSet<GameObject> recentAttackers = new HashSet<GameObject>(); // 🔥 일정 시간 동안 같은 무기 중복 방지

    public GameObject RubblePrefab; // ✅ 잔해물 프리팹을 인스펙터에서 할당
    public GameObject ExplodeEff;
    private GameObject Eff_E;
    private GameObject Rub_R;

    public AudioClip DestroySfx, DamageSfx;

    private bool isDestroyed = false;

    void Start()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.detectCollisions = true;
    }

    public void InitializeBuilding(string name, int level, int health)
    {
        buildingName = name;
        buildingLevel = level;
        currentHealth = health;
        //Debug.Log($"🏗️ {buildingName} 초기화 완료! 레벨: {buildingLevel}, 체력: {currentHealth}");
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"💥 {buildingName}이(가) 공격받음! 받은 데미지: {damage} | 남은 체력: {currentHealth}");
        if (isDestroyed) return; // ✅ 중복 파괴 방지

        currentHealth -= damage;
        //AudioSource.PlayClipAtPoint(DamageSfx, transform.position);

        if (currentHealth <= 0)
        {
            isDestroyed = true;
            DestroyBuilding();
        }
    }

    private void DestroyBuilding()
    {
        // 폭발 이펙트 생성 및 사운드 재생
        Eff_E = Instantiate(ExplodeEff, transform.position, Quaternion.identity);
        //AudioSource.PlayClipAtPoint(DestroySfx, transform.position);

        // ✅ 잔해물 생성
        if (RubblePrefab != null)
        {
            Rub_R = Instantiate(RubblePrefab, transform.position, RubblePrefab.transform.rotation);

            var rubbleHandler = Rub_R.GetComponent<Rubble_RepairHandler>();
            if (rubbleHandler != null)
            {
                rubbleHandler.InitializeRubble(buildingName, buildingLevel, transform.position, transform.rotation);
            }
            else
            {
                Debug.LogWarning("⚠️ Rubble_RepairHandler 컴포넌트를 찾을 수 없습니다.");
            }
        }

        Debug.Log($"💥 {buildingName} 파괴됨!");
        Destroy(gameObject);
        Destroy(Eff_E, 1f);
    }
}
