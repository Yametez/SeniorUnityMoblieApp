using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using static CurrentUser;  // เพิ่มบรรทัดนี้

public class CGResultScreen : MonoBehaviour
{
    public GameObject resultScreenUI;
    public Text timeText;
    public Text matchesText;
    private CGTimer timer;
    private CGGameManager gameManager;
    private CGApiManager apiManager;
    private DateTime gameStartTime;
    
    void Start()
    {
        timer = GetComponent<CGTimer>();
        gameManager = GetComponent<CGGameManager>();
        apiManager = GetComponent<CGApiManager>();
        resultScreenUI.SetActive(false);
        gameStartTime = DateTime.Now;

        // ตรวจสอบการ login
        UserData currentUser = CurrentUser.GetCurrentUser();
        if (currentUser == null || currentUser.userId <= 0)
        {
            Debug.LogWarning("No user logged in! Redirecting to login page...");
            SceneManager.LoadScene("FistHomePage");
            return;
        }
    }

    public void ShowResult()
    {
        if (!resultScreenUI.activeSelf)
        {
            resultScreenUI.SetActive(true);
            
            float totalTime = timer.GetCurrentTime();
            int minutes = Mathf.FloorToInt(totalTime / 60f);
            int seconds = Mathf.FloorToInt(totalTime % 60f);
            timeText.text = string.Format("เวลา: {0:00}:{1:00}", minutes, seconds);
            
            int totalMatches = gameManager.GetTotalMatches();
            matchesText.text = "จับคู่สำเร็จ: " + totalMatches.ToString() + " คู่";

            // ดึง user ID จาก CurrentUser
            UserData currentUser = CurrentUser.GetCurrentUser();
            string userId = currentUser.userId.ToString();

            Debug.Log($"Sending result - Time: {totalTime}, Matches: {totalMatches}, UserID: {userId}");
            
            // ส่งข้อมูลไปยัง API
            StartCoroutine(apiManager.SendGameResult(
                userId,          // User ID จาก CurrentUser
                totalTime,       // เวลาที่ใช้
                totalMatches    // จำนวนคู่ที่จับได้
            ));
        }
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