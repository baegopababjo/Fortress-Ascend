using UnityEngine;

public class Tower_Cannon : MonoBehaviour
{
    public GameObject Shot;
    public GameObject Smoke;
    public GameObject Flame;
    public Transform tr;
    public AudioClip CannonSfx;
    GameObject Shoot_Shot;
    GameObject eff;
    public void Shoot()
    {
        Quaternion quat = tr.rotation;
        Eff_P(Flame, 0.5f, quat);
        Shoot_Shot = Instantiate(Shot, tr.position, quat);
        Shoot_Shot.GetComponent<Cannon_Shot>().att_dmg = transform.GetComponentInParent<Tower_NPC_AI>().attackPower;
        Shoot_Shot.GetComponent<Cannon_Shot>().Attack(tr.forward);
        Shoot_Shot.layer = 14;
        Shoot_Shot.SetActive(true);
        Eff_P(Smoke, 1f, quat);
    }
    public void Cannon_Sound()
    {
        AudioSource.PlayClipAtPoint(CannonSfx, transform.position);
    }
    void Eff_P(GameObject e, float t, Quaternion q)
    {
        eff = Instantiate(e, tr.position, q);
        eff.SetActive(true);
        Destroy(eff, 1f);
    }
}
