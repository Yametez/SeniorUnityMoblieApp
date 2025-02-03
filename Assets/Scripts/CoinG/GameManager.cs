using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isPaused = false;
    public GameObject pauseMenuPanel; // อ้างอิงถึง Panel ของ Pause Menu

    void Awake()
    {
        Instance = this;
        pauseMenuPanel.SetActive(false); // ซ่อน Panel ตอนเริ่มเกม
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenuPanel.SetActive(isPaused);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(2); // หรือใช้ Application.Quit(); ถ้าต้องการออกจากเกมเลย
    }
} 