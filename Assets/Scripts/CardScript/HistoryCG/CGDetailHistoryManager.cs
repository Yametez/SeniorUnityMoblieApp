using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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
        public float time;
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
        string trainingId = PlayerPrefs.GetString("SelectedTrainingId", "");
        if (string.IsNullOrEmpty(trainingId))
        {
            Debug.LogError("No Training ID found!");
            return;
        }

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

                if (string.IsNullOrEmpty(jsonResponse))
                {
                    Debug.LogError("Empty response from API");
                    return;
                }

                var gameData = JsonUtility.FromJson<CardGameData>(jsonResponse);
                
                if (gameData != null)
                {
                    if (dateText != null && !string.IsNullOrEmpty(gameData.Start_Time))
                    {
                        DateTime startTime = DateTime.Parse(gameData.Start_Time);
                        dateText.text = $"วันที่เล่น {startTime.ToString("dd/MM/yyyy")}";
                    }
                    else
                    {
                        Debug.LogWarning("dateText is null or Start_Time is empty");
                    }

                    if (timeText != null && !string.IsNullOrEmpty(gameData.Time_limit))
                    {
                        timeText.text = $"เวลา : {gameData.Time_limit}";
                    }
                    else
                    {
                        Debug.LogWarning("timeText is null or Time_limit is empty");
                    }

                    if (matchesText != null && !string.IsNullOrEmpty(gameData.Result_Training))
                    {
                        try
                        {
                            var result = JsonUtility.FromJson<CardGameResult>(gameData.Result_Training);
                            matchesText.text = $"จับคู่สำเร็จ : {result.matches} คู่";
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Error parsing Result_Training: {e.Message}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("matchesText is null or Result_Training is empty");
                    }
                }
                else
                {
                    Debug.LogError("Failed to parse game data from JSON");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in CGDetailHistoryManager: {e.Message}");
        }
    }

    public void BackToHistory()
    {
        SceneManager.LoadScene("HistoryCG");
    }
} 