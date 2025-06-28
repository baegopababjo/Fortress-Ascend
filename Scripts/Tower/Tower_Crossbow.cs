using UnityEngine;
using System.Collections;
using System;

public class Tower_Crossbow : MonoBehaviour
{
    Animator anim_C;
    GameObject arrow_Attach, arrow_C;
    public GameObject arrow;
    AudioSource audioSource;
    public AudioClip stringSfx, fireSfx;
    void Start()
    {

    }
    public void Ready()
    {
        arrow_Attach = transform.GetChild(4).gameObject;
        anim_C = transform.GetComponent<Animator>();
        arrow_Attach.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    public void Fire()
    {
        StartCoroutine(Fire_P());
    }
    IEnumerator Fire_P()
    {
        anim_C.SetTrigger("Fire");
        yield return new WaitForSeconds(0.07f);
        Quaternion quat = arrow_Attach.GetComponentInParent<Transform>().rotation * arrow_Attach.transform.rotation;
        arrow_C = Instantiate(arrow, arrow_Attach.transform.position, quat * Quaternion.Euler(180, 0, 0));
        arrow_C.GetComponent<Arrow_NPC>().att_dmg = transform.GetComponentInParent<Tower_NPC_AI>().attackPower;
        arrow_C.GetComponent<Arrow_NPC>().speed = 6f;
        arrow_C.GetComponent<Arrow_NPC>().extraGravityScale = 28f;
        arrow_Attach.SetActive(false);
        arrow_C.gameObject.layer = 14;
        arrow_C.SetActive(true);
        arrow_C.GetComponent<Arrow_NPC>().Attack(transform.forward);
    }
    public void Aim()
    {
        anim_C.SetTrigger("Hold");
        arrow_Attach.SetActive(true);
    }
    public void StringSound() { AudioSource.PlayClipAtPoint(stringSfx, transform.position); }
    public void FireSound() { AudioSource.PlayClipAtPoint(fireSfx, transform.position); }
}
