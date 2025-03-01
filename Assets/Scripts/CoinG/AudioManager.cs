using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Sound Effects")]
    public AudioClip coinDropSound;      // เสียงลากเหรียญ
    public AudioClip levelCompleteSound; // เสียงจบเลเวล
    public AudioClip gameOverSound;      // เสียงจบเกม

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float coinDropVolume = 0.5f;    // ค่าเริ่มต้น 50%
    [Range(0f, 1f)]
    public float levelCompleteVolume = 0.7f;  // ค่าเริ่มต้น 70%
    [Range(0f, 1f)]
    public float gameOverVolume = 0.7f;     // ค่าเริ่มต้น 70%
    
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
        PlaySound(coinDropSound, coinDropVolume);
    }

    public void PlayLevelComplete()
    {
        PlaySound(levelCompleteSound, levelCompleteVolume);
    }

    public void PlayGameOver()
    {
        PlaySound(gameOverSound, gameOverVolume);
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
} 