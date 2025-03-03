using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using CoinGame; // เพิ่ม namespace นี้

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool isPaused = false;
    public GameObject pauseMenuPanel; // อ้างอิงถึง Panel ของ Pause Menu
    public CoinManager coinManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // เก็บ GameManager ไว้ระหว่าง Scene
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        ResetGameState();
    }

    public void ResetGameState()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        if (coinManager != null)
        {
            coinManager.ResetGame();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(isPaused);
            
            // ตรวจสอบและปรับ sorting order ของ Canvas
            Canvas pauseCanvas = pauseMenuPanel.GetComponentInParent<Canvas>();
            if (pauseCanvas != null)
            {
                pauseCanvas.sortingOrder = 10;  // ค่าสูงกว่า game objects อื่นๆ
            }
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
    }

    public void QuitGame()
    {
        ResetGameState();
        SceneManager.LoadScene(2); // หรือใช้ Application.Quit(); ถ้าต้องการออกจากเกมเลย
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGameState();
    }

    public void StartNextLevel()
    {
        coinManager.StartLevel(coinManager.currentLevel + 1);
    }
} 