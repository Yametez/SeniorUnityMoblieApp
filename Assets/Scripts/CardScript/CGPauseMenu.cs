using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CGPauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public CGGameManager gameManager;
    public CGTimer timer;
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        timer.ResumeTimer();
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        timer.PauseTimer();
        isPaused = true;
    }

    public void RestartGame()
    {
        Resume();
        gameManager.RestartGame();
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("FistHomePage");
    }
} 