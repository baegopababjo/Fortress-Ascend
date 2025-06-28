using UnityEngine;
using System.Collections;
//using Unity.Android.Gradle.Manifest;
using UnityEngine.EventSystems;

public class Player_Move : MonoBehaviour
{
    public float Player_Move_Speed = 3.0f;  // 이동 속도 조절 변수
    private Animator animator;  // 애니메이터 변수 추가
    CharacterController cc;
    private float gravity = -99f; // 중력 적용
    private bool isAttacking = false; // 🛑 공격 중인지 체크
    private bool IsDamage = false;
    private bool isDead = false; // ⬅️ 상태 변수 추가

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();  // Animator 컴포넌트 가져오기

        // ✅ 사망 상태 초기화
        isDead = false;
        isAttacking = false;
        IsDamage = false;

        // ✅ 캐릭터 컨트롤러 활성화 (혹시 이전 씬에서 비활성화 되었을 수 있음)
        if (!cc.enabled)
        {
            cc.enabled = true;
            Debug.Log("✅ CharacterController 재활성화됨");
        }
    }

    void Update()
    {
        if (isDead) return; // ✅ 죽은 상태면 이동 등 처리 안 함

             // 🔒 마법 선택 중이면 이동 차단
        if (CharacterSelection.Instance != null && !CharacterSelection.Instance.isMagicSelected)
            return;

        // 🔥 상점이 열려 있거나 환경설정 창 열었다면 공격 입력을 차단
        if (ShopUI.IsShopOpen || (SettingsMenuManager.Instance != null && SettingsMenuManager.Instance.IsMenuOpen())) return;

        // 🛑 공격 중에는 이동하지 않음
        if (isAttacking)
        {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 플레이어가 바라보는 방향 기준으로 이동
        Vector3 dir = new Vector3(h, 0, v);
        dir = transform.TransformDirection(dir);  // **카메라가 아닌 플레이어 기준**
        dir = dir.normalized;

        if (!cc.isGrounded) { dir.y += gravity * Time.deltaTime; }
        cc.Move(dir * Player_Move_Speed * Time.deltaTime);

        // 애니메이션 트리거 설정
        HandleAnimation(h, v);
    }

    void HandleAnimation(float h, float v)
    {
        bool isMoving = (h != 0 || v != 0);  // 움직이면 true, 아니면 false

        animator.SetBool("isWalking", isMoving); // 움직이는 동안 걷기 애니메이션 실행
        animator.SetFloat("MoveX", h);
        animator.SetFloat("MoveY", v);
    }

    // 🏹 공격 중 상태 설정 함수 추가
    public void SetAttackState(bool attacking)
    {
        isAttacking = attacking;
    }

    public void Damaged(int att_damage)
    {
        if (!IsDamage) { IsDamage = true; StartCoroutine(Damage_Process(att_damage)); }
    }

    IEnumerator Damage_Process(int att_damage)
    {
        if (GetComponent<PlayerStats>().health > 0)
        {
            animator.SetTrigger("Damaged");
            GetComponent<PlayerStats>().Damaged(att_damage);

            // ✅ 체력 UI 갱신
            HpUIManager hpUI = FindAnyObjectByType<HpUIManager>();
            if (hpUI != null)
            {
                hpUI.UpdateHpUI();
            }
        }
        else
        {
            Die();
        }

        yield return new WaitForSeconds(1f);
        IsDamage = false;
    }

    void Die()
    {
        StopAllCoroutines();
        animator.SetTrigger("Die");
        isDead = true; // ⬅️ 죽었음을 표시

        // 🔒 카메라 회전 막기
        Player_Camera cam = FindFirstObjectByType<Player_Camera>();
        if (cam != null)
        {
            cam.SetRotationEnabled(false);
            //Debug.Log("📷 카메라 회전 차단됨 (플레이어 사망)");
        }

        StartCoroutine(Die_Process());
    }


    IEnumerator Die_Process()
    {
        cc.enabled = false;
        yield return new WaitForSeconds(4f); // 죽음 애니메이션 대기

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.GameOver();
        }
        else
        {
            Debug.LogError("❌ GameManager를 찾을 수 없습니다. GameOver 실행 실패");
        }
    }
}
