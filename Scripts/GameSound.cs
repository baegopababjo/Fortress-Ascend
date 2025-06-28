using UnityEngine;

public class GameSound : MonoBehaviour
{
    [Header("클릭 사운드")]
    public AudioClip clicksound;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D 사운드로 설정 (UI에 적합)
        audioSource.volume = 0.7f;
    }

    //클릭 소리
    public void PlayClickSound()
    {
        if (clicksound != null)
        {
            audioSource.PlayOneShot(clicksound);
        }
    }
}
