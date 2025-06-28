using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Tower_NPC_AI : MonoBehaviour
{
    private Tower_NPC_Stats TnpcStats;  // 🔥 NPC 스탯 가져오기
    private GameStatsData gameStatsData; // 🔥 GameStatsData 연결
    public AudioClip DamageSfx, DieSfx;
    private AudioSource Sound;
    Animator anim;
    CharacterController cc;
    GameObject target;
    float currentTime, attackRange, attackcooldown, minAttackRange, rotRange = 0f;
    public int attackPower;
    int MaxHP, hp;
    public string cw;
    string[] tags;
    private Vector3 targetlook;
    public int overrideLevel = 1; // 🆕 외부에서 설정 가능하도록 public 변수 추가

    enum State
    {
        Idle,
        Attack,
        Damaged,
        Die
    }
    State T_State;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        cc = GetComponent<CharacterController>();
        TnpcStats = GetComponent<Tower_NPC_Stats>();                                                // 🔥 NPC_Status 가져오기
        TnpcStats.gameStatsData = GameObject.FindAnyObjectByType<GameManager>()?.gameStatsData;     // 🔥 gameStatsData 미리 연결
        Sound = GetComponent<AudioSource>();

        // ✅ gameStatsData가 연결됐는지 확인
        if (TnpcStats.gameStatsData == null)
        {
            Debug.LogError("❌ Tower_NPC_AI에서 gameStatsData 연결 실패! GameManager 또는 ScriptableObject 누락 확인");
            return;
        }

        
        
        TnpcStats.SetTowerNPC(this.transform, cw, overrideLevel);
        gameStatsData = TnpcStats.gameStatsData; // 🔥 GameStatsData 가져오기
        if (TnpcStats != null)
        {
            // 🔥 NPC 스탯 가져와서 적용
            MaxHP = TnpcStats.health;
            hp = MaxHP;
            attackPower = TnpcStats.attackPower;
            attackRange = TnpcStats.attackRange * 0.01f;
            minAttackRange = 1200 * 0.01f;
            attackcooldown = TnpcStats.AttackCooldown;
        }
        currentTime = attackcooldown;
    }
    public void batch() // NPC를 배치할 때 참조하는 함수
    {
        T_State = State.Idle;
    }
    void Idle()
    {
        FindTarget();
        if (target == null)
        {
            //Debug.LogWarning("⚠️ [Idle] target이 null입니다. 적절한 대상이 있는지 확인하세요.");
            return; // target이 없으면 오류 방지
        }
        else
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if(dist < attackRange && dist >= minAttackRange)
            {
                T_State = State.Attack;
                anim.SetTrigger("Attack");
            }

        }
    }
    void FindTarget() // 관련 태그를 가진 오브젝트들을 찾아 배열로 변환, 이후 본인과 가장 가까운 오브젝트를 타깃으로 설정
    {
        GameObject[] targets;
        targets = null;
        tags = new string[] { "Enemy" };
        float targetDist = Mathf.Infinity;
        GameObject R_target = null;
        foreach (string t_tag in tags)
        {
            targets = GameObject.FindGameObjectsWithTag(t_tag);
            foreach (GameObject t_target in targets)
            {
                Vector3 objectPos = t_target.transform.position;
                if ((objectPos - transform.position).sqrMagnitude < targetDist)
                {

                    targetDist = (objectPos - transform.position).sqrMagnitude;
                    R_target = t_target;
                }
            }
            target = R_target;
        }
    }
    void Attack()
    {
        if (Vector3.Distance(transform.position, target.transform.position) <= attackRange)
        {
            currentTime += Time.deltaTime;
            if (cw == "crossbow") { rotRange = -180f; }
            targetlook = new Vector3(target.transform.position.x, transform.position.y + rotRange, target.transform.position.z).normalized;
            transform.LookAt(targetlook, Vector3.up);
            if (currentTime > attackcooldown)
            {
                currentTime = 0;
                anim.SetTrigger("Attack");
            }
        }
        else
        {
            T_State = State.Idle;
            currentTime = 0;
            anim.SetTrigger("AT_T_ID");
        }
    }
    public void HitEnemy(int hitPower)
    {
        if (T_State == State.Damaged || T_State == State.Die) { return; }
        hp -= hitPower;

        //Debug.Log($"💥 {gameObject.name}가 {hitPower}만큼의 피해를 입음! 남은 체력: {hp}");

        if (hp > 0)
        {
            T_State = State.Damaged;
            anim.SetTrigger("Damaged");
            Damaged();
        }
        else
        {
            T_State = State.Die;
            anim.SetTrigger("Die");
            Die();
        }
    }
    void Damaged()
    {
        StartCoroutine(DamageProcess());
    }
    IEnumerator DamageProcess()
    {
        yield return new WaitForSeconds(1.0f);
        T_State = State.Idle;
    }
    void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DieProcess());
    }
    IEnumerator DieProcess()
    {
        cc.enabled = false;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    void Update()
    {
        switch (T_State)
        {
            case State.Idle:
                Idle();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Damaged:
                break;
            case State.Die:
                break;
        }
    }
}
