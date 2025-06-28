using UnityEngine;

public class Cannon_Shot : MonoBehaviour
{
    public int att_dmg;
    public GameObject bombEffect;
    public float explosionRadius = 3f;
    public AudioClip ShotSfx;
    GameObject eff;
    float speed;
    Rigidbody rb;
    Vector3 Force;
    public void Attack(Vector3 dir)
    {
        speed = 10f;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        Force = (dir * speed);
        rb.AddForce(Force, ForceMode.Impulse);
    }
    private void OnTriggerEnter(Collider col)
    {
        AudioSource.PlayClipAtPoint(ShotSfx, transform.position);
        Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius);
        for (int i = 0; i < cols.Length; i++) { if (cols[i].tag == "Enemy") { cols[i].GetComponent<NPC_AI>().HitEnemy(att_dmg); } }
        eff = Instantiate(bombEffect);
        eff.transform.position = transform.position;
        Destroy(gameObject);
        Destroy(eff, 1.5f);
    }
}
