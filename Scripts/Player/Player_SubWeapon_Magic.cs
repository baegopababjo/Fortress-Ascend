using System.Collections;
using UnityEngine;

public class Player_SubWeapon_Magic : MonoBehaviour
{
    public float explosionRadius = 6.0f;
    public GameObject MagicEff;
    public int att_dmg;
    GameObject eff;

    public void Attack() // 공격 활성화시 실행
    {
        eff = Instantiate(MagicEff, transform.position, Quaternion.identity);

        // ✅ 여기서 바로 데미지 적용
        DealDamage();

        StartCoroutine(DestroyAfterDelay());
    }

    void DealDamage()
    {
        //Debug.Log($"Damage = {att_dmg}, 범위 = {explosionRadius}");
        Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].CompareTag("Enemy"))
            {
                NPC_AI enemy = cols[i].GetComponent<NPC_AI>();
                if (enemy != null)
                {
                    enemy.IsFall = true;
                    enemy.HitEnemy(att_dmg);
                }
            }
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        // 🧹 이펙트도 파괴
        if (eff != null)
            Destroy(eff);

        // 🧹 마법 오브젝트 파괴
        Destroy(gameObject);
    }
}
