using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Sound Effects")]
    public AudioClip coinDropSound;      // เสียงลากเหรียญ
    public AudioClip levelCompleteSound; // เสียงจบเลเวล
    public AudioClip gameOverSound;      // เสียงจบเกม
    public AudioClip wrongPlaceSound;    // เสียงเตือนวางผิดที่
    public AudioClip backgroundMusic;    // เพิ่มเสียงเพลงพื้นหลัง

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float coinDropVolume = 0.5f;    // ค่าเริ่มต้น 50%
    [Range(0f, 1f)]
    public float levelCompleteVolume = 0.7f;  // ค่าเริ่มต้น 70%
    [Range(0f, 1f)]
    public float gameOverVolume = 0.7f;     // ค่าเริ่มต้น 70%
    [Range(0f, 1f)]
    public float wrongPlaceVolume = 0.6f;  // ความดังเสียงเตือน
    [Range(0f, 1f)]
    public float backgroundMusicVolume = 0.3f;  // ความดังเพลงพื้นหลัง
    
    private AudioSource audioSource;
    private AudioSource musicSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            // แยก AudioSource สำหรับ effects และ music
            audioSource = gameObject.AddComponent<AudioSource>();
            musicSource = gameObject.AddComponent<AudioSource>();
            
            // ตั้งค่า music source
            musicSource.loop = true;
            musicSource.volume = backgroundMusicVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
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

    public void PlayWrongPlace()
    {
        PlaySound(wrongPlaceSound, wrongPlaceVolume);
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    // เพิ่มเมธอดสำหรับ fade out เพลง
    public void FadeOutBackgroundMusic()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float startVolume = musicSource.volume;
        float fadeTime = 2f;  // ระยะเวลาในการ fade out (2 วินาที)

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;  // รีเซ็ตความดังกลับค่าเดิม
    }
} 