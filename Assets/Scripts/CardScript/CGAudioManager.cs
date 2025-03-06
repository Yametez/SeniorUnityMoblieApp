using UnityEngine;

public class CGAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip cardFlipSound;
    public AudioClip matchSound;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float defaultMusicVolume = 0.3f;  // ค่าเริ่มต้นเสียงเพลง
    [Range(0f, 1f)]
    public float defaultSFXVolume = 0.7f;    // ค่าเริ่มต้นเสียงเอฟเฟค
    
    private static CGAudioManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAudioSources()
    {
        // ตั้งค่า Music Source
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = true;
        }
        musicSource.volume = defaultMusicVolume;
        
        // ตั้งค่า SFX Source
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        sfxSource.volume = defaultSFXVolume;
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
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }
    
    public void PlayCardFlip()
    {
        if (cardFlipSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(cardFlipSound);
        }
    }
    
    public void PlayMatchSound()
    {
        if (matchSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(matchSound);
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = Mathf.Clamp01(volume);
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = Mathf.Clamp01(volume);
        }
    }

    // เพิ่มฟังก์ชันสำหรับหยุดเพลง
    public void StopBackgroundMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    // เพิ่มฟังก์ชันสำหรับ pause/unpause เพลง
    public void PauseBackgroundMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void UnpauseBackgroundMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }
} 