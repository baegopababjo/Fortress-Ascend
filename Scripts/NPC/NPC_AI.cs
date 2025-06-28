using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_AI : MonoBehaviour // NPC 움직임 제어
{
    // 🔍 타겟 탐색 캐시 (태그별 GameObject 목록 저장)
    private Dictionary<string, List<GameObject>> cachedTaggedObjects;

    // 📦 상태/활성 여부
    public bool IsFall = false;
    public bool IsActive = true;

    // 📊 전투 관련 수치
    public int attackPower;
    public string cw;                      // NPC 클래스 문자열
    public float rot_range;               // 회전 보정 값
    private float findDistance;           // 타겟 감지 거리
    private float distance;               // 타겟과의 거리
    private float gravity = -9.81f;       // 중력 값
    private float verticalVelocity = 0f;  // 중력 가속 누적값

    private int MaxHP;
    private int hp;

    // ⚙️ 컴포넌트 참조
    private CharacterController cc;
    private NavMeshAgent smith;
    private Animator anim;
    private GameObject target;

    // ⚔️ 전투 스탯
    private float attackRange;
    private float attackcooldown;
    private Vector3 targetlook;
    //public Vector3 returnPos;
    //private readonly Vector3 nullPos = Vector3.zero;

    // 🎯 타겟 탐색용 태그 목록
    private string[] tags;

    // 🔁 상태 머신 정의
    private enum State
    {
        Preview,    // 배치 상태
        Idle,       // 대기
        Chase,      // 추격
        Attack,     // 공격
        Damaged,    // 피해 입음
        Fall,       // 넘어짐
        Die         // 사망
    }
    private State N_state;
    //State N_state;

    // 📚 외부 데이터
    private NPC_Stats npcStats;
    private GameStatsData gameStatsData;

    void Start()
    {
        // 기본 위치 및 컴포넌트 초기화
        //returnPos = nullPos;
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        smith = GetComponent<NavMeshAgent>();
        npcStats = GetComponent<NPC_Stats>();
        gameStatsData = npcStats?.gameStatsData;

        // NavMeshAgent 설정
        smith.updatePosition = false;
        smith.updateRotation = true;
        gravity = -9.81f;

        // 🔥 적군일 경우 클래스 정보로 NPC 세팅
        if (CompareTag("Enemy") && npcStats != null)
        {
            npcStats.SetNPC(this.transform, cw);
        }

        // 🔥 스탯 데이터 적용
        if (npcStats != null)
        {
            MaxHP = npcStats.health;
            hp = MaxHP;
            attackPower = npcStats.attackPower;
            attackRange = npcStats.attackRange * 0.01f;
            attackcooldown = npcStats.AttackCooldown;
            findDistance = 8f;
            rot_range = npcStats.rot_r;
        }

        // 초기 상태 설정
        if (CompareTag("Enemy"))
            N_state = State.Idle;

        // 🔍 탐색 태그 설정 및 캐시 초기화
        BuildTagCache();
    }

    void Update()
    {
        // 전투 중 타겟이 사라졌으면 상태 Idle로 복귀
        if ((N_state == State.Chase || N_state == State.Attack) && target == null)
        {
            N_state = State.Idle;
            return;
        }

        // NavMeshAgent 이동 로직 적용
        if (!CompareTag("preview_Ally") && cc.enabled)
        {
            Idle_Smith();
        }

        // 상태 머신 실행
        switch (N_state)
        {
            case State.Preview:
                anim.Play("Idle");
                break;
            case State.Idle:
                Idle();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Damaged:
                break;
            case State.Fall:
                Fall();
                break;
            case State.Die:
                break;
        }
    }

    void BuildTagCache()
    {
        FindTag(); // 자동으로 태그 설정

        cachedTaggedObjects = new Dictionary<string, List<GameObject>>();
        foreach (string tag in tags)
        {
            cachedTaggedObjects[tag] = new List<GameObject>(GameObject.FindGameObjectsWithTag(tag));
        }
    }



    void Idle()
    {
        FindTarget();

        if (target == null)
        {
            //Debug.LogWarning("⚠️ [Idle] target이 null입니다. 적절한 대상이 있는지 확인하세요.");
            return; // target이 없으면 오류 방지
        }
        //anim.Play("Idle"); // ❗ Idle 애니메이션만 반복
        if (Vector3.Distance(transform.position, target.transform.position) < findDistance || transform.CompareTag("Enemy"))
        {
            smith.stoppingDistance = attackRange;
            smith.SetDestination(target.transform.position);
            N_state = State.Chase;
            anim.SetTrigger("ID_T_C");
            //Debug.Log($"👁️ [{gameObject.name}] 대상 발견 → 추격 시작! 🎯 타겟: {target.name}");

        }
    }
    void FindTarget() // 관련 태그를 가진 오브젝트들을 찾아 배열로 변환, 이후 본인과 가장 가까운 오브젝트를 타깃으로 설정
    {
        FindTag(); // tags 설정

        float targetDist = Mathf.Infinity;
        GameObject R_target = null;

        foreach (string t_tag in tags)
        {
            if (!cachedTaggedObjects.ContainsKey(t_tag)) continue;

            foreach (GameObject t_target in cachedTaggedObjects[t_tag])
            {
                if (t_target == null || !t_target.activeSelf) continue;

                float dist = (t_target.transform.position - transform.position).sqrMagnitude;
                if (dist < targetDist)
                {
                    targetDist = dist;
                    R_target = t_target;
                }
            }
        }

        target = R_target;
    }
    void CalDistance() // 공격하려는 오브젝트 크기를 고려하여 공격 사거리 계산
    {
        Collider targetCollider = target.GetComponent<Collider>();
        if (targetCollider == null) { targetCollider = target.GetComponentInParent<Collider>(); }
        Vector3 closestPoint = targetCollider.bounds.ClosestPoint(transform.position);
        distance = Vector3.Distance(transform.position, closestPoint);
    }
    void FindTag() // 소속 별 목표 종류들의 태그 설정
    {
        if (transform.CompareTag("Ally") || transform.CompareTag("preview_Ally")) { tags = new string[] { "Enemy" }; } 
        else if (transform.CompareTag("Enemy")) { tags = new string[] { "Player", "Ally", "Tower", "Wall", "Building" }; }
        else { Debug.Log("해당 캐릭터의 소속을 알 수 없습니다."); Debug.Log($"소속 = {transform.tag.ToString()}"); return; }
    }
    void Chase()
    {
        if (target == null) // ✅ target 파괴 여부 체크!
        {
            N_state = State.Idle;
            return;
        }

        CalDistance();

        if (transform.CompareTag("Enemy")) // 적 NPC의 경우, 해당 상황에서 가장 가까운 오브젝트로 타깃을 변경 후 이동
        {
            if (distance <= attackRange + 0.1f)
            {
                N_state = State.Attack;
                anim.SetTrigger("C_T_AD");
            }
            else
            {
                FindTarget();
                NPC_ResetPath(); 
            }
        }
        else if (transform.CompareTag("Ally"))
        {
            if (distance > attackRange + 0.1f){ 
                NPC_ResetPath(); 
            }else if (distance <= attackRange + 0.1f)
            {
                N_state = State.Attack;
                anim.SetTrigger("C_T_AD");
            }
        }
    }
    void NPC_ResetPath() // 추적 경로 재설정
    {
        Vector3 pos = target.transform.position;
        NavMeshPath N_Pos = new NavMeshPath();
        smith.isStopped = true;
        smith.ResetPath();
        smith.stoppingDistance = attackRange;
        smith.CalculatePath(pos, N_Pos);
        smith.SetPath(N_Pos);
        //smith.SetDestination(pos);
    }
    void Attack()
    {
        if (target == null || !target.activeSelf)
        {
            N_state = State.Idle;
            anim.SetTrigger("AT_T_ID");
            return;
        }

        CalDistance();

        if (distance <= attackRange + 0.1f)
        {
            StartCoroutine(Attack_P());
        }
        else
        {
            if (distance <= findDistance)
            {
                N_state = State.Chase;
                anim.SetTrigger("AT_T_C");

                //if (tag == "Enemy")
                    //Debug.Log($"👣 [Enemy Chase] {gameObject.name} 사거리 밖, 추격 시작! 대상 거리: {distance:F2}");
            }
            else
            {
                N_state = State.Idle;
                anim.SetTrigger("AT_T_ID");

                if (tag == "Enemy")
                    Debug.Log($"🛑 [Enemy Idle] {gameObject.name} 대상 거리 멀어짐, 대기 상태 전환.");
            }
        }
    }
    IEnumerator Attack_P()
    {
        targetlook = new Vector3(target.transform.position.x + rot_range, transform.position.y, target.transform.position.z);
        transform.LookAt(targetlook);
        anim.SetTrigger("Start_AT");

        // ✅ 콜라이더 일정 시간 활성화
        StartCoroutine(EnableWeaponColliderTemporarily(1f)); // 1초 동안 활성화

        yield return new WaitForSeconds(attackcooldown);
    }

    void Damaged()
    {
        StartCoroutine(DamageProcess());
    }
    IEnumerator DamageProcess()
    {
        yield return new WaitForSeconds(1.0f);
        N_state = State.Idle;
    }
    public void HitEnemy(int hitPower)
    {
        if (N_state == State.Damaged || N_state == State.Die)
        { 
            return; 
        }
        hp -= hitPower;
        smith.isStopped = true;
        smith.ResetPath();

        Debug.Log($"💥 {gameObject.name}가 {hitPower}만큼의 피해를 입음! 남은 체력: {hp}");

        if (hp > 0)
        {
            if (IsFall)
            {
                N_state = State.Fall;
                IsFall = false;
                anim.SetTrigger("Fall");
            }
            else
            {
                N_state = State.Damaged;
                anim.SetTrigger("Dm");
                Damaged();
            }
        }
        else
        {
            N_state = State.Die;
            anim.SetTrigger("Die");
            Die();
        }
    }
    void Fall()
    {
        StartCoroutine(FallProcess());
    }
    IEnumerator FallProcess()
    {
        N_state = State.Fall;  // 🔒 상태 고정
        yield return new WaitForSeconds(1.0f);
        anim.SetTrigger("F_T_WU");
        StartCoroutine(WakeUpProcess());
    }
    IEnumerator WakeUpProcess()
    {
        yield return new WaitForSeconds(2.0f);
        N_state = State.Idle;
    }
    void Die()
    {
        StopAllCoroutines();

        // ✅ 죽으면 태그를 무력화 (AI 타겟 대상에서 제외)
        gameObject.tag = "Untagged";

        StartCoroutine(DieProcess());
    }
    IEnumerator DieProcess()
    {
        cc.enabled = false;
        yield return new WaitForSeconds(1f);
        AddGoldToPlayer();

        ResetNPC();               // 초기화 함수 호출
        gameObject.SetActive(false); // 풀에 반환
    }

    void ResetNPC()
    {
        // 기본 전투 스탯
        hp = MaxHP;
        target = null;
        N_state = State.Idle;

        // 태그 복원 (죽을 때 Untagged 됐으니까)
        gameObject.tag = "Enemy";

        // 컴포넌트 복구
        if (cc != null) cc.enabled = true;
        if (smith != null)
        {
            smith.isStopped = false;
            smith.ResetPath();
        }

        // 애니메이터 초기화
        if (anim != null)
        {
            anim.Rebind(); // 애니메이션 상태 초기화
            anim.Update(0f); // 프레임 갱신
        }

        // 무기 콜라이더 꺼놓기
        DisableWeaponCollider();
    }






    public void SetPreviewState()
    {
        var sword = GetComponentInChildren<Sword>();
        if (sword != null) sword.DisableCollider();
        N_state = State.Preview;

        // 🏷️ 배치 중엔 preview_Ally로 변경해서 타겟팅 되지 않도록 처리
        gameObject.tag = "preview_Ally";

        if (smith != null)
        {
            smith.isStopped = true;
            smith.ResetPath();
        }
    }

    public void batch()
    {
        gameObject.tag = "Ally"; // 🏷️ 배치 완료 후 Ally로 복구
        N_state = State.Idle;
    }

    void Idle_Smith()
    {
        if (smith.enabled && smith.hasPath)
        {
            // NavMeshAgent의 desiredVelocity를 사용하여 수평 이동
            if (smith.desiredVelocity.sqrMagnitude > Mathf.Epsilon)
            {
                Vector3 horizontalVelocity = smith.desiredVelocity;
                horizontalVelocity.y = 0f; // y축 속도 제거

                // 수직 속도 적용
                Vector3 verticalMovement = Vector3.up * verticalVelocity * Time.deltaTime;

                // 최종 이동 벡터 (수평 + 수직)
                cc.Move(horizontalVelocity * Time.deltaTime + verticalMovement);

                // 회전
                if (smith.remainingDistance > smith.stoppingDistance)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(smith.desiredVelocity.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * smith.angularSpeed);
                }
            }
            else
            {
                // 목표 지점에 도착했거나 desiredVelocity가 없으면 수직 이동만 적용
                Vector3 verticalMovement = Vector3.up * verticalVelocity * Time.deltaTime;
                cc.Move(verticalMovement);
            }
        }
        else
        {
            // NavMeshAgent가 비활성화되었거나 경로가 없으면 수직 이동만 적용
            Vector3 verticalMovement = Vector3.up * verticalVelocity * Time.deltaTime;
            cc.Move(verticalMovement);
        }

        // 중력 적용 및 접지 판정 (매 프레임)
        if (!cc.isGrounded) { verticalVelocity += gravity * Time.deltaTime; }
        else { verticalVelocity = 0f; } // 땅에 닿으면 수직 속도 초기화

        // CharacterController의 위치를 NavMeshAgent에 동기화 (매 프레임)
        smith.Warp(transform.position);
    }

    void AddGoldToPlayer()
    {
        // 🎯 "Player" 레이어를 가진 오브젝트 찾기
        int playerLayer = LayerMask.NameToLayer("Player");
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        PlayerStats playerStats = null;
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == playerLayer)
            {
                playerStats = obj.GetComponent<PlayerStats>();
                if (playerStats != null) break;
            }
        }

        if (playerStats == null)
        {
            Debug.LogWarning("⚠️ PlayerStats를 찾을 수 없습니다! 골드 지급 실패");
            return;
        }

        // 🎯 NPC 클래스 문자열 변환 후 Enum 비교
        int goldReward = 0;
        GameStatsData.NPCClass npcClassType;
        if (System.Enum.TryParse(npcStats.npcClass.ToString(), out npcClassType))
        {
            if (npcClassType == GameStatsData.NPCClass.Warrior)
            {
                goldReward = 30;
            }
            else if (npcClassType == GameStatsData.NPCClass.Archer)
            {
                goldReward = 50;
            }
        }

        // 💰 골드 추가
        playerStats.AddGold(goldReward);
    }

    IEnumerator EnableWeaponColliderTemporarily(float duration)
    {
        EnableWeaponCollider();
        yield return new WaitForSeconds(duration);
        DisableWeaponCollider();
    }
    public void EnableWeaponCollider()
    {
        var sword = GetComponentInChildren<Sword>();
        if (sword != null)
        {
            sword.EnableCollider();
            //Debug.Log($"🟢 [Collider ON] {gameObject.name} 무기 콜라이더 활성화됨");
        }
    }

    public void DisableWeaponCollider()
    {
        var sword = GetComponentInChildren<Sword>();
        if (sword != null)
        {
            sword.DisableCollider();
            //Debug.Log($"🔴 [Collider OFF] {gameObject.name} 무기 콜라이더 비활성화됨");
        }
    }
}
