using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour
{
    public AudioClip SwordSfm;
    private Collider weaponCollider;
    private AudioSource audioSource;
    bool IsActive = false;

    void Awake()
    {
        weaponCollider = GetComponent<Collider>();
        weaponCollider.enabled = false; // 초기 비활성화
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void EnableCollider()
    {
        weaponCollider.enabled = true;
    }

    public void DisableCollider()
    {
        weaponCollider.enabled = false;
    }

    IEnumerator PlaySwordSound()
    {
        IsActive = true;
        audioSource.PlayOneShot(SwordSfm);
        yield return new WaitForSeconds(1f);
        IsActive = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!weaponCollider.enabled) return;

        int att_dmg = transform.GetComponentInParent<NPC_AI>().attackPower;
        // Ally Sword
        if (this.CompareTag("Enemy") && col.gameObject.layer == 17 && IsActive) { StartCoroutine(PlaySwordSound()); }
        if (col.gameObject.CompareTag("Tower") && col.gameObject.layer == 9) { col.gameObject.GetComponent<Tower_NPC_AI>().HitEnemy(att_dmg); }
        //8,9 = Enemy, Ally NPC
        else if (col.gameObject.layer == 8 || col.gameObject.layer == 9)
        {
            col.gameObject.GetComponent<NPC_AI>().HitEnemy(att_dmg);
        }
        //10 = player
        else if (col.gameObject.layer == 10)
        {
            col.gameObject.GetComponent<Player_Move>().Damaged(att_dmg);
            Debug.Log("🧍‍♂️ 플레이어가 NPC의 공격을 받음! 공격자: " + transform.root.name + ", 데미지: " + att_dmg);
        }
        //13, 14 = Anemy, Ally Projectile
        else if (col.gameObject.layer == 13 || col.gameObject.layer == 14)
        {
            PlaySwordSound();
            Destroy(col.gameObject);
        }
        else if (col.gameObject.layer == 11) // ✅ 빌딩 공격 처리
        {
            BuildingDamageHandler building = col.GetComponent<BuildingDamageHandler>();
            if (building != null)
            {
                building.TakeDamage(att_dmg);
                Debug.Log("🏰 빌딩이 NPC의 공격을 받음! 데미지: " + att_dmg);
            }
        }
    }
}
