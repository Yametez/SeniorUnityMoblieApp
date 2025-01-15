using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CoinGame;

public class ResultPanel : MonoBehaviour
{
    public Text timeResultText;
    public Text coin10ResultText;
    public Text coin5ResultText;
    public Text coin1ResultText;
    public Text totalScoreText;
    
    public void ShowResults(float time, int coin10Count, int coin5Count, int coin1Count, int totalScore)
    {
        Debug.Log("ShowResults called");
        
        if (timeResultText == null || coin10ResultText == null || 
            coin5ResultText == null || coin1ResultText == null || 
            totalScoreText == null)
        {
            Debug.LogError("Some Text components are not assigned!");
            return;
        }

        int calculatedScore = (coin10Count * 10) + (coin5Count * 5) + (coin1Count * 1);
        
        gameObject.SetActive(true);
        
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timeResultText.text = $"เวลาที่ใช้: {minutes}:{seconds:D2} นาที";
        coin10ResultText.text = $"เหรียญ 10 บาท: {coin10Count} เหรียญ";
        coin5ResultText.text = $"เหรียญ 5 บาท: {coin5Count} เหรียญ";
        coin1ResultText.text = $"เหรียญ 1 บาท: {coin1Count} เหรียญ";
        totalScoreText.text = $"คะแนนรวม: {calculatedScore} บาท";

        Debug.Log($"Result Panel - Coins: 10B={coin10Count}, 5B={coin5Count}, 1B={coin1Count}");
        Debug.Log($"Result Panel - Calculated Score: {calculatedScore}, Received Score: {totalScore}");
    }

    public void OnBackHomeButtonClick()
    {
        SceneManager.LoadScene(2);
    }

    public void OnRestartButtonClick()
    {
        gameObject.SetActive(false);
        FindObjectOfType<CoinManager>().StartNewGame();
    }
} 