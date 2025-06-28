using UnityEngine;

public class Tower_Crossbow_Override : MonoBehaviour
{
    public Tower_Crossbow t_crossbow;
    void Start()
    {
        t_crossbow.Ready();
        Aim();
    }
    public void Fire()
    {
        t_crossbow.Fire();
        t_crossbow.Aim();
    }
    public void Aim()
    {
        t_crossbow.Aim();
    }
}
