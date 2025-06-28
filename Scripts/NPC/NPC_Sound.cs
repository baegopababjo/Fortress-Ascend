using UnityEngine;

public class NPC_Sound : MonoBehaviour
{
    [Header("👣 걷기 사운드")]
    public AudioClip footstepSound;

    [Header("💥 활 시위 놓는 타이밍 사운드")]
    public AudioClip ReleaseStringSound;

    [Header("💥 칼 사운드")]
    public AudioClip SwordSound;

    [Header("😵 피격 사운드")]
    public AudioClip hitSound;

    [Header("😵 낙하 사운드")]
    public AudioClip fallSound;

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
        audioSource.spatialBlend = 1.0f; // 3D 사운드로 설정
        audioSource.volume = 0.7f;       // 적당한 볼륨
    }

    // 👣 애니메이션 이벤트에서 호출될 함수
    public void PlayFootstepSound()
    {
        if (footstepSound != null)
        {
            audioSource.PlayOneShot(footstepSound);
        }
    }

    // 💥 활 시위 놓는 타이밍 사운드
    public void PlayStringSound()
    {
        if (ReleaseStringSound != null)
        {
            audioSource.PlayOneShot(ReleaseStringSound);
        }
    }

    // 💥 칼 사운드
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

    // 😵 낙하 사운드
    public void PlayFallSound()
    {
        if (fallSound != null)
        {
            audioSource.PlayOneShot(fallSound);
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
