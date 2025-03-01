using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Sound Effects")]
    public AudioClip coinDropSound;      // เสียงลากเหรียญ
    public AudioClip levelCompleteSound; // เสียงจบเลเวล
    public AudioClip gameOverSound;      // เสียงจบเกม
    
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayCoinDrop()
    {
        PlaySound(coinDropSound);
    }

    public void PlayLevelComplete()
    {
        PlaySound(levelCompleteSound);
    }

    public void PlayGameOver()
    {
        PlaySound(gameOverSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
} 