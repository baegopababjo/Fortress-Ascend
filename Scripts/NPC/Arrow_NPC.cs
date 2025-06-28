using NUnit.Framework;
using System.Collections;
using System.Runtime.InteropServices;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class Arrow_NPC : MonoBehaviour
{
    public int att_dmg;
    public float speed, extraGravityScale;
    Rigidbody rb;
    Vector3 Force;
    public void Attack(Vector3 dir)
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        Force = (dir * speed);
        rb.AddForce(Force, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider col)
    {
        if(gameObject.layer == col.gameObject.layer) { return; }
        else if (col.gameObject.CompareTag("Tower") && col.gameObject.layer == 9) { col.gameObject.GetComponent<Tower_NPC_AI>().HitEnemy(att_dmg); }
        else if (col.gameObject.layer == 8 || col.gameObject.layer == 9)
        {
            col.gameObject.GetComponent<NPC_AI>().HitEnemy(att_dmg);
        }
        else if (col.gameObject.layer == 10)
        {
            col.gameObject.GetComponent<Player_Move>().Damaged(att_dmg);

        }
        Destroy(gameObject, 3f);
    }
    void FixedUpdate()
    {
        // 추가적인 아래쪽 힘을 가하여 중력 효과 증가
        if (rb != null) { rb.AddForce(Physics.gravity * (extraGravityScale - 1f), ForceMode.Acceleration); }
    }
}

