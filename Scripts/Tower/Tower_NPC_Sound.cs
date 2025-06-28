using UnityEngine;
using UnityEngine.Audio;

public class Tower_NPC_Sound : MonoBehaviour
{
    Tower_NPC_AI tower_npc;
    AudioClip DamageSfx, DieSfx;
    AudioSource audioSource;
    void Start()
    {
        tower_npc = GetComponentInParent<Tower_NPC_AI>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; // 3D 사운드로 설정
        audioSource.volume = 0.7f;       // 적당한 볼륨
        DamageSfx = tower_npc.DamageSfx;
        DieSfx = tower_npc.DieSfx;
    }
    public void Play_Sound(string stat)
    {
        if (stat == "Die") { audioSource.PlayOneShot(DieSfx); }
        else if (stat == "Damage") { audioSource.PlayOneShot(DamageSfx); }
    }
}
