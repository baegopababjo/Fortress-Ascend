using UnityEngine;

public class Player_SubWeapon_Enhanced : MonoBehaviour
{
    public GameObject MagicEff; // ¿Ã∆Â∆Æ «¡∏Æ∆’
    GameObject eff;

    public void Attack()
    {
        if (MagicEff != null)
        {
            eff = Instantiate(MagicEff, transform.position, Quaternion.identity);
            eff.transform.SetParent(this.transform);
        }
    }

    public void DestroyEffect()
    {
        if (eff != null)
        {
            Destroy(eff);
        }

        Destroy(gameObject);
    }
}
