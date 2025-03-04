using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CGResultScreen : MonoBehaviour
{
    public GameObject resultScreenUI;
    public Text timeText;
    public Text matchesText;
    private CGTimer timer;
    private CGGameManager gameManager;

    void Start()
    {
        timer = GetComponent<CGTimer>();
        gameManager = GetComponent<CGGameManager>();
        resultScreenUI.SetActive(false);
    }

    public void ShowResult()
    {
        resultScreenUI.SetActive(true);
        
        // แสดงเวลาที่ใช้
        float totalTime = timer.GetCurrentTime();
        int minutes = Mathf.FloorToInt(totalTime / 60f);
        int seconds = Mathf.FloorToInt(totalTime % 60f);
        timeText.text = string.Format("เวลา: {0:00}:{1:00}", minutes, seconds);
        
        // แสดงจำนวนการจับคู่
        matchesText.text = "จับคู่สำเร็จ: " + gameManager.GetTotalMatches().ToString() + " คู่";
    }

    public void RestartGame()
    {
        resultScreenUI.SetActive(false);
        gameManager.RestartGame();
    }

    public void BackToHome()
    {
        SceneManager.LoadScene("FistHomePage");
    }
} 