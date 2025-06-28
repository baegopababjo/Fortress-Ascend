using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Bow : MonoBehaviour
{
    Animator anim;
    AudioSource audioSource;
    public AudioClip stringSfx;
    void Start()
    {
        anim = transform.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    public void Attack()
    {
        StartCoroutine(Attack_P());
    }
    IEnumerator Attack_P()
    {
        anim.SetTrigger("ATT");
        yield return new WaitForSeconds(2.0f);
        anim.SetTrigger("End_AT");

    }
    public void String_Sound() { AudioSource.PlayClipAtPoint(stringSfx, transform.position); }
}
