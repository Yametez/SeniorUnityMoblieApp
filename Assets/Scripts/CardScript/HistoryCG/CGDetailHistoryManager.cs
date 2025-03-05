using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class CGDetailHistoryManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text dateText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text matchesText;

    private string apiUrl = "http://localhost:3000/api/training/detail/";

    [System.Serializable]
    private class CardGameResult
    {
        public string time;
        public int matches;
    }

    [System.Serializable]
    private class CardGameData
    {
        public string Training_ID;
        public string Start_Time;
        public string Time_limit;
        public string Result_Training;
    }

    private async void Start()
    {
        string trainingId = PlayerPrefs.GetString("SelectedTrainingId");
        Debug.Log($"Loading details for Training ID: {trainingId}");

        try
        {
            using (UnityWebRequest www = UnityWebRequest.Get($"{apiUrl}{trainingId}"))
            {
                var operation = www.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"API Error: {www.error}");
                    return;
                }

                string jsonResponse = www.downloadHandler.text;
                Debug.Log($"API Response: {jsonResponse}");

                var gameData = JsonUtility.FromJson<CardGameData>(jsonResponse);
                
                if (gameData != null)
                {
                    // แสดงวันที่
                    if (dateText != null)
                    {
                        DateTime startTime = DateTime.Parse(gameData.Start_Time);
                        dateText.text = $"วันที่เล่น {startTime.ToString("dd/MM/yyyy")}";
                    }

                    // แสดงเวลาที่ใช้
                    if (timeText != null)
                    {
                        timeText.text = $"เวลา : {gameData.Time_limit}";
                    }

                    // แสดงจำนวนคู่ที่จับได้
                    if (matchesText != null)
                    {
                        // แปลง Result_Training เป็น CardGameResult
                        var result = JsonUtility.FromJson<CardGameResult>(gameData.Result_Training);
                        matchesText.text = $"จับคู่สำเร็จ : {result.matches} คู่";
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
    }
} 