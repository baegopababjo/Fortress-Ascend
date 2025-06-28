using UnityEngine;

public class Player_Sound : MonoBehaviour
{
    [Header("👣 걷기 사운드")]
    public AudioClip footstepSound;

    [Header("💥 강화 마법 사운드")]
    public AudioClip enhancedSound;

    [Header("💥 활 시위 사운드")]
    public AudioClip ReleaseStringSound;
    public AudioClip PullStringSound;

    [Header("💥 검 사운드")]
    public AudioClip SwordSound;

    [Header("💥 마법 충격파 사운드")]
    public AudioClip magicSound;

    [Header("😵 피격 사운드")]
    public AudioClip hitSound;

    [Header("😵 사망 사운드")]
    public AudioClip dieSound;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; // 3D 사운드 적용
        audioSource.volume = 0.7f;
    }

    // 👣 걷기 소리 (애니메이션 이벤트용)
    public void PlayFootstepSound()
    {
        if (footstepSound != null)
        {
            audioSource.PlayOneShot(footstepSound);
        }
    }

    // 💥 충격파 사운드
    public void PlayMagicAttackSound()
    {
        if (magicSound != null)
        {
            audioSource.PlayOneShot(magicSound);
        }
    }

    public void PlayEnhancedAttackSound()
    {
        if (enhancedSound != null)
        {
            audioSource.PlayOneShot(enhancedSound);
        }
    }

    public void PlayStringSound(string str)
    {
        if(str == "Release")
        {
            if (ReleaseStringSound != null)
            {
                audioSource.PlayOneShot(ReleaseStringSound);
            }
        }

        else if (str == "Pull")
        {
            if (PullStringSound != null)
            {
                audioSource.PlayOneShot(PullStringSound);
            }
        }
        
    }

    public void PlaySwordSound()
    {
        if (SwordSound != null)
        {
            audioSource.PlayOneShot(SwordSound);
        }
    }
    

    // 😵 피격 사운드
    public void PlayHitSound()
    {
        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    // 😵 사망 사운드
    public void PlayDieSound()
    {
        if (dieSound != null)
        {
            audioSource.PlayOneShot(dieSound);
        }
    }
}
