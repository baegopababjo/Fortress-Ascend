using UnityEngine;

public class Cannon_Override : MonoBehaviour
{
    public Tower_Cannon cannon;
    public void Attack_C()
    {
        cannon.Shoot();
        cannon.Cannon_Sound();
    }
}
