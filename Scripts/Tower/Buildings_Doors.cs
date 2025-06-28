using UnityEngine;
using System.Collections;

public class Buildings_Doors : MonoBehaviour
{
    Animator anim;
    public AudioClip DoorOpen, DoorClose;
    AudioSource audioSource;
    bool IsOpen, IsActive;
    public float interactDistance = 7f;
    public LayerMask DoorLayer;
    void Start()
    {
        anim = GetComponent<Animator>();
        IsOpen = true; IsActive = false;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactDistance, DoorLayer) && Input.GetKey(KeyCode.E) && !IsActive)
        {
            IsActive = true; 
            Door_Control();
        }
    }
    void Door_Control()
    {
        StartCoroutine(DoorProcess());
    }
    IEnumerator DoorProcess()
    {
        if (IsOpen) { anim.SetTrigger("Close"); IsOpen = false; }
        else { anim.SetTrigger("Open"); IsOpen = true; }
        yield return new WaitForSeconds(0.6f);
        IsActive = false;
    }
    public void Door_Sound(string str)
    {
        if (str == "Open") { audioSource.PlayOneShot(DoorOpen); }
        else if (str == "Close") { audioSource.PlayOneShot(DoorClose); }
    }
}
