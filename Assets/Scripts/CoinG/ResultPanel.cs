using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CoinGame;
using System;

public class ResultPanel : MonoBehaviour
{
    public Text timeResultText;
    public Text coin10ResultText;
    public Text coin5ResultText;
    public Text coin1ResultText;
    public Text totalScoreText;
    
    [SerializeField] private ResultAnalysis resultAnalysis;
    [SerializeField] private ExamManager examManager;

    private string finalEvaluationResult;
    private string finalAdviceText;
    private bool isResultFinalized = false;

    public void ShowResults(float time, int coin10Count, int coin5Count, int coin1Count, int totalScore)
    {
        if (isResultFinalized) return; // ถ้าผลถูกสรุปแล้ว ไม่ต้องคำนวณใหม่

        Debug.Log("ShowResults called");
        
        if (timeResultText == null || coin10ResultText == null || 
            coin5ResultText == null || coin1ResultText == null || 
            totalScoreText == null)
        {
            Debug.LogError("Some Text components are not assigned!");
            return;
        }

        gameObject.SetActive(true);
        
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timeResultText.text = $"เวลาที่ใช้: {minutes}:{seconds:D2} นาที";
        coin10ResultText.text = $"เหรียญ 10 บาท: {coin10Count} เหรียญ";
        coin5ResultText.text = $"เหรียญ 5 บาท: {coin5Count} เหรียญ";
        coin1ResultText.text = $"เหรียญ 1 บาท: {coin1Count} เหรียญ";
        totalScoreText.text = $"คะแนนรวม: {totalScore} บาท";

        Debug.Log($"Result Panel - Coins: 10B={coin10Count}, 5B={coin5Count}, 1B={coin1Count}");
        Debug.Log($"Result Panel - Calculated Score: {totalScore}");

        var coinManager = FindObjectOfType<CoinManager>();
        if (coinManager != null)
        {
            int totalSpawnedCoins = coinManager.GetTotalSpawnedCoins();
            int totalCollectedCoins = coin10Count + coin5Count + coin1Count;
            
            float speedPercentage = CalculateSpeedScore(time);
            float accuracyPercentage = CalculateAccuracyScore(totalCollectedCoins, totalSpawnedCoins);
            float memoryPercentage = CalculateMemoryScore(coin10Count, coin5Count, coin1Count);
            
            resultAnalysis.UpdateResults(speedPercentage, accuracyPercentage, memoryPercentage);

            float averageScore = (speedPercentage + accuracyPercentage + memoryPercentage) / 3f;
            
            // เก็บผลลัพธ์สุดท้าย
            if (averageScore >= 60f)
            {
                finalEvaluationResult = "ไม่พบความเสี่ยง";
                finalAdviceText = "สมองของคุณทำงานได้ดี\nควรรักษาสุขภาพสมองด้วย\nการออกกำลังกายสม่ำเสมอ";
            }
            else if (averageScore >= 40f)
            {
                finalEvaluationResult = "พบความเสี่ยงต่ำ";
                finalAdviceText = "ควรเพิ่มการฝึกฝน\nความจำและการคิด\nแนะนำให้ปรึกษาแพทย์";
            }
            else
            {
                finalEvaluationResult = "พบความเสี่ยงสูง";
                finalAdviceText = "แนะนำให้พบแพทย์โดยเร็ว\nควรได้รับการตรวจประเมิน\nอย่างละเอียด";
            }

            isResultFinalized = true; // ล็อคผลลัพธ์

            // บันทึกผลทันที
            if (examManager != null)
            {
                DateTime endTime = DateTime.Now;
                DateTime startTime = endTime.AddSeconds(-time);
                
                examManager.SaveExamResult(
                    "Coin Game",
                    startTime,
                    endTime,
                    time,
                    speedPercentage,
                    accuracyPercentage,
                    memoryPercentage,
                    finalEvaluationResult,
                    finalAdviceText
                );
            }
        }
    }

    private float CalculateSpeedScore(float time)
    {
        float maxTime = 300f;
        return Mathf.Max(0, (1 - (time / maxTime)) * 100f);
    }

    private float CalculateAccuracyScore(int totalCollectedCoins, int totalSpawnedCoins)
    {
        if (totalSpawnedCoins == 0) return 0;
        
        // คำนวณเป็นเปอร์เซ็นต์จากจำนวนเหรียญที่มีจริง
        float baseAccuracy = (totalCollectedCoins / (float)totalSpawnedCoins) * 100f;
        
        // ปรับให้ยากขึ้น: ต้องเก็บได้ 100% ถึงจะได้คะแนนเต็ม
        if (baseAccuracy >= 100f) {
            return 100f;
        } else if (baseAccuracy >= 80f) {
            return 80f + ((baseAccuracy - 80f) * 0.8f); // ช่วง 80-100% จะได้คะแนน 80-96
        } else {
            return baseAccuracy * 0.8f; // ต่ำกว่า 80% จะได้คะแนนน้อยลง 20%
        }
    }

    private float CalculateMemoryScore(int coin10, int coin5, int coin1)
    {
        int totalCoins = coin10 + coin5 + coin1;
        if (totalCoins == 0) return 0;
        
        float weightedScore = (coin10 * 1.0f + coin5 * 0.7f + coin1 * 0.4f) / totalCoins;
        return weightedScore * 100f;
    }

    public void OnBackHomeButtonClick()
    {
        // ไม่ต้อง reset ค่าใดๆ เพราะบันทึกไปแล้ว
        SceneManager.LoadScene(2);
    }

    public void OnRestartButtonClick()
    {
        gameObject.SetActive(false);
        var coinManager = FindObjectOfType<CoinManager>();
        if (coinManager != null)
        {
            coinManager.StartNewGame();
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameState();
        }

        if (examManager != null)
        {
            examManager.ResetSaveStatus();
        }
    }
} 