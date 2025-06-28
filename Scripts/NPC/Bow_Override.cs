using UnityEngine;
using System.Collections;
//using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using Unity.Burst.Intrinsics;
//using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;

public class Bow_Override : MonoBehaviour
{
    public Bow bow;
    public GameObject arrow;
    public Transform arr_T;
    GameObject arrow_p;
    private void Start()
    {

    }
    public void Attack_B()
    {
        bow.Attack();
    }
    public void Arrow_Q()
    {
        arrow_p = Instantiate(arrow, arr_T);
        arrow_p.gameObject.tag = gameObject.tag;
        arrow_p.gameObject.layer = gameObject.layer;
        arrow_p.SetActive(true);
    }
    public void Arrow_Att()
    {
        transform.RotateAround(transform.position, Vector3.up, -5f);
        Quaternion quat = arrow_p.transform.rotation.normalized;
        Destroy(arrow_p);
        arrow_p = Instantiate(arrow, arr_T.transform.position, quat);
        arrow_p.GetComponent<Animator>().enabled = false;
        if (this.gameObject.layer == 8) { arrow_p.gameObject.layer = 13; } // ���� ���� ȭ�� ���̾ �ٸ��� �����Ͽ� ���� ���� NPC�� �ݶ��̴� ���� X
        else if (this.gameObject.layer == 9) { arrow_p.gameObject.layer = 14; }
        arrow_p.SetActive(true);
        arrow_p.GetComponent<Arrow_NPC>().att_dmg = transform.GetComponentInParent<NPC_AI>().attackPower;
        arrow_p.GetComponent<Arrow_NPC>().speed = 2f;
        arrow_p.GetComponent<Arrow_NPC>().extraGravityScale = 1f;
        arrow_p.GetComponent<Arrow_NPC>().Attack(transform.forward.normalized); // �Ű������� Ȱ ���� ��������(��Ȯ�� ��ġ�� ���� ��ǥ ����)
    }
}
