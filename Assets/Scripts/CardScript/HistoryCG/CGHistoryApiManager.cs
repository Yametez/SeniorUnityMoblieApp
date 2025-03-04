using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;

public class CGHistoryApiManager : MonoBehaviour
{
    private string apiUrl = "http://localhost:3000/api/training/latest/";

    [System.Serializable]
    public class CardGameHistory
    {
        public int Training_ID;
        public string Start_Time;
        public string Training_name;
        public string Time_limit;
        public string Result_Training;
        public bool has_history;
    }

    // เพิ่ม wrapper class สำหรับ JSON array
    [System.Serializable]
    private class CardGameHistoryWrapper
    {
        public CardGameHistory[] items;
    }

    [System.Serializable]
    public class CardGameResult
    {
        public float time;
        public int matches;
    }

    [System.Serializable]
    public class CardGameHistoryList
    {
        public CardGameHistory[] histories;
    }

    public IEnumerator GetUserHistory(string userId, System.Action<CardGameHistory[]> callback)
    {
        string url = apiUrl + userId;
        Debug.Log($"Fetching history from: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
                callback(null);
            }
            else
            {
                try
                {
                    string jsonResponse = request.downloadHandler.text;
                    Debug.Log($"Received JSON: {jsonResponse}");
                    
                    // แปลง JSON เป็น single object
                    CardGameHistory history = JsonUtility.FromJson<CardGameHistory>(jsonResponse);
                    
                    if (history != null && history.has_history)
                    {
                        // สร้าง array ที่มีข้อมูลเดียว
                        callback(new CardGameHistory[] { history });
                    }
                    else
                    {
                        Debug.LogWarning("No history data found");
                        callback(null);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON parsing error: {e.Message}\nJSON: {request.downloadHandler.text}");
                    callback(null);
                }
            }
        }
    }
} 