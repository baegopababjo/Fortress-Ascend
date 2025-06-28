using UnityEngine;
using System.Collections;

public class Player_Attack : MonoBehaviour
{
    private PlayerStats playerStats;                 // 플레이어의 스텟
    private GameStatsData gameStatsData;             // ScriptableObject: 게임 통계 데이터
    private Animator animator;                       // Animator 컴포넌트
    private Player_Weapon_Sword swordWeapon;         // 검 무기 컴포넌트
    private Player_Weapon_Bow bowWeapon;             // 활 무기 컴포넌트
    public Player_SubWeapon_Magic P_magic;           // 보조 무기가 마법일 경우 참조
    public Player_SubWeapon_Enhanced P_empoweredMagic;  // 강화마법 프리팹


    private bool empoweredAttackActive = false;      // 강화 마법 활성화 여부
    private bool canAttack = true;                   // 기본 공격 가능 여부
    private bool canUseMagic = true;                 // 마법 공격 가능 여부

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        gameStatsData = playerStats?.gameStatsData;

        animator = GetComponent<Animator>();
        swordWeapon = GetComponentInChildren<Player_Weapon_Sword>();
        bowWeapon = GetComponentInChildren<Player_Weapon_Bow>();

        if (playerStats == null || animator == null)
            Debug.LogError("필수 컴포넌트가 누락되었습니다: PlayerStats 또는 Animator.");
    }

    void Update()
    {
        if (ShouldBlockInput()) //입력 차단 (특정 조건)
            return;

        HandleBasicAttack();    // 기본 무기 공격
        HandleMagicAttack();    // 마법 공격
    }


      /// 입력 차단 조건이 만족되면 true 반환
    private bool ShouldBlockInput()
    {
        return
            (CharacterSelection.Instance != null && !CharacterSelection.Instance.isMagicSelected) || // 마법 미선택
            ShopUI.IsShopOpen ||                                                                     // 상점 열림
            (BuildingPlacementManager.Instance?.IsPlacingBuilding() ?? false) ||                     // 건물 배치 중
            (NpcPlacementManager.Instance?.IsPlacingNpc() ?? false) ||                               // NPC 배치 중
            (SettingsMenuManager.Instance?.IsMenuOpen() ?? false);                                   // 설정 메뉴 열림
    }

    //기본 무기 공격
    private void HandleBasicAttack()
    {
        if (!canAttack || !Input.GetMouseButtonDown(0))
            return;

        switch (playerStats.currentWeapon)
        {
            case PlayerStats.WeaponMode.Sword:
                StartCoroutine(SwordAttack());
                break;

            case PlayerStats.WeaponMode.Bow:
                StartCoroutine(BowAttack());
                break;
        }
    }

    //마법 공격
    private void HandleMagicAttack()
    {
        if (canUseMagic && Input.GetMouseButtonDown(1))
        {
            StartCoroutine(MagicAttack());
        }
    }


    IEnumerator SwordAttack()
    {
            // 현재 상태가 Move 애니메이션이면 초기화
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Move")) animator.Play("Move", 0, 0f);

        
        SetAttackState(true);   // 이동 제한 + 회전 제한 걸기

        if (animator != null) animator.SetTrigger("Attack");                        // 공격 애니메이션 실행
        if (swordWeapon != null) swordWeapon.EnableWeapon(empoweredAttackActive);   // 공격 시작 시 검 Collider 활성화

        var (weaponDamage, weaponRange, cooldown) = gameStatsData.GetWeaponStats(playerStats.currentWeapon);
        int totalDamage = playerStats.baseAttackPower + weaponDamage;

        totalDamage = ApplyEmpoweredAttack(totalDamage);    // 강화 공격 적용

        PrintAttackDebug("검", totalDamage, weaponRange, cooldown);
        canAttack = false;

            // 공격 쿨타임 계산
        float attackAnimTime = animator.GetCurrentAnimatorStateInfo(0).length;
        float totalWaitTime = Mathf.Max(cooldown);
        yield return new WaitForSeconds(totalWaitTime);

            // 공격 종료 시 검 Collider 비활성화
        if (swordWeapon != null) swordWeapon.DisableWeapon();

        SetAttackState(false);   // 공격 끝나면 이동 + 회전 다시 가능
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }


    IEnumerator BowAttack()
    {
        if (animator != null) animator.SetTrigger("Shoot"); // 활 쏘는 애니메이션 실행

        
        SetAttackState(true);   // 이동 및 회전 제한 (활 공격 시)

        yield return new WaitForSeconds(0.3f); // 화살 발사 타이밍 설정 (조정하면 됨)

            // 공격력, 사거리, 쿨타임 로그 출력
        var (weaponDamage, weaponRange, cooldown) = gameStatsData.GetWeaponStats(PlayerStats.WeaponMode.Bow);
        int totalDamage = playerStats.baseAttackPower + weaponDamage;

            // 강화 공격 적용
        totalDamage = ApplyEmpoweredAttack(totalDamage);

        Debug.Log("🏹 활 공격 시작");
        PrintAttackDebug("활", totalDamage, weaponRange, cooldown);

        if (bowWeapon != null) bowWeapon.ShootArrow(); // 활 공격 실행
        canAttack = false;

        yield return new WaitForSeconds(cooldown);
        SetAttackState(false);  // 공격 끝나면 이동 및 회전 다시 가능하게
        canAttack = true;
    }


    IEnumerator MagicAttack()
    {
        var currentMagic = playerStats.currentMagic;
        var (magicDamage, magicRange, cooldown) = gameStatsData.GetMagicStats(currentMagic);

        if (currentMagic == PlayerStats.MagicMode.EmpoweredAttack)
        {
            // 강화 마법 적용
            PrepareEmpoweredAttack();
        }
        else if (currentMagic == PlayerStats.MagicMode.MeleeMagic)
        {
            // 근거리 마법 실행
            CastMeleeMagic(magicDamage, magicRange);
        }
        else
        {
            // 지원하지 않는 마법 타입
            Debug.LogWarning($"⚠️ 현재 마법 타입({currentMagic})은 처리되지 않았습니다.");
        }

        canUseMagic = false;
        yield return new WaitForSeconds(cooldown);
        canUseMagic = true;
    }


    //이동 제한 + 회전 제한 걸기 또는 해제
    private void SetAttackState(bool isAttacking)
    {
        GetComponent<Player_Move>()?.SetAttackState(isAttacking);
        FindFirstObjectByType<Player_Camera>()?.SetAttackState(isAttacking);
    }

    //강화 공격 데미지 적용 처리
    private int ApplyEmpoweredAttack(int baseDamage)
    {
        if (empoweredAttackActive)
        {
            int bonus = gameStatsData.GetMagicStats(PlayerStats.MagicMode.EmpoweredAttack).bonusDamage;
            Debug.Log("🔥 강화 공격 효과 발동! (1회 한정)");
            empoweredAttackActive = false;

            var magicEffects = FindObjectsByType<Player_SubWeapon_Enhanced>(FindObjectsSortMode.None);
            foreach (var effect in magicEffects)
            {
                effect.DestroyEffect();
            }


            return baseDamage + bonus;
        }
        return baseDamage;
    }

    //디버깅 로그 출력
    private void PrintAttackDebug(string weaponName, int damage, float range, float cooldown)
    {
        Debug.Log($"[{weaponName}] 공격력: {damage} 💥 | 사거리: {range} 🎯 | 쿨타임: {cooldown}s ⏳");
    }

    //강화 마법
    private void PrepareEmpoweredAttack()
    {
        empoweredAttackActive = true;
        Debug.Log("🔥 강화 공격 준비 완료! (다음 공격 1회 강화)");

        GameObject magicObj = Instantiate(P_empoweredMagic.gameObject, transform.position, Quaternion.identity, transform);
        Player_SubWeapon_Enhanced magicComponent = magicObj.GetComponent<Player_SubWeapon_Enhanced>();

        if (magicComponent != null)
        {
            magicComponent.Attack();
        }
        else
        {
            Debug.LogError("❌ Player_SubWeapon_Enhanced 컴포넌트를 찾을 수 없음");
        }

    }


    //근거리 마법
    private void CastMeleeMagic(int damage, float range)
    {
        // 🧙‍♀️ 먼지마법 발사
        GameObject magicObj = Instantiate(P_magic.gameObject, transform.position, Quaternion.identity);
        Player_SubWeapon_Magic magicComponent = magicObj.GetComponent<Player_SubWeapon_Magic>();

        if (magicComponent != null)
        {
            magicComponent.att_dmg = damage;
            magicComponent.explosionRadius = range * 0.01f;
            magicComponent.Attack();
        }
        else
        {
            Debug.LogError("❌ Player_SubWeapon_Magic 컴포넌트를 찾을 수 없음");
        }

        // 🎵 사운드 재생
        Player_Sound sound = GetComponent<Player_Sound>();
        sound?.PlayMagicAttackSound();

        Debug.Log($"[🪄 먼지 마법] 체력: {playerStats.health} 💟 | 공격력: {damage} 💪 | 사거리: {range} 🎯");
    }

}
