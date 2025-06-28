using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{
    private int damage; // 🔥 화살 데미지 (PlayerStats에서 받아옴)
    private Collider arrowCollider; // 🔥 화살의 Collider 참조
    private Rigidbody arrowRb; // 🔥 화살의 Rigidbody 참조

    void Start()
    {
        arrowCollider = GetComponent<Collider>();
        arrowRb = GetComponent<Rigidbody>();

        // 🔥 Rigidbody가 없으면 오류 출력
        if (arrowRb == null)
        {
            Debug.LogError("❌ Arrow 프리팹에 Rigidbody가 없습니다. 중력 적용이 불가능합니다.");
        }
        else
        {
            arrowRb.useGravity = true;                                                      // ✅ 중력 적용
            arrowRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;      // ✅ 충돌 감지 향상
            arrowRb.interpolation = RigidbodyInterpolation.Interpolate;                     // ✅ 부드러운 물리 움직임
        }

        // 🔥 Collider가 없을 경우 오류 출력
        if (arrowCollider == null)
        {
            Debug.LogError("❌ Arrow 프리팹에 Collider가 없습니다. 충돌 판정이 정상적으로 동작하지 않을 수 있습니다.");
        }
        else
        {
            arrowCollider.isTrigger = false; // ✅ 트리거 모드 해제하여 OnCollisionEnter 사용
        }

        // 🔥 5초 후 자동 삭제
        Destroy(gameObject, 5f);
    }

    // 🔥 데미지 설정 함수 추가
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    // 🔥 충돌 판정 (OnCollisionEnter)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy_NPC"))
        {
            NPC_AI enemyAI = collision.gameObject.GetComponent<NPC_AI>();
            if (enemyAI != null)
            {
                enemyAI.HitEnemy(damage);
                Debug.Log($"🏹 화살 명중! {collision.gameObject.name}에게 {damage} 데미지!");
            }

            Destroy(gameObject); // 명중 후 화살 삭제
        }
    }
}
