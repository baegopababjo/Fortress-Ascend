using UnityEngine;

public class GameSound : MonoBehaviour
{
    [Header("Ŭ�� ����")]
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
        audioSource.spatialBlend = 0f; // 2D ����� ���� (UI�� ����)
        audioSource.volume = 0.7f;
    }

    //Ŭ�� �Ҹ�
    public void PlayClickSound()
    {
        if (clicksound != null)
        {
            audioSource.PlayOneShot(clicksound);
        }
    }
}
