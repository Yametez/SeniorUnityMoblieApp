using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;
using System.Linq;

public class CGHistoryApiManager : MonoBehaviour
{
    // เปลี่ยน URL เป็นดึงข้อมูลทั้งหมด
    private string apiUrl = "http://localhost:3000/api/training/";

    [System.Serializable]
    public class CardGameHistory
    {
        public string Training_ID;
        public string Start_Time;
        public string End_Time;
        public string Training_name;
        public string Time_limit;
        public string Result_Training;
        public string User_ID;
        public string id;
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
        string url = apiUrl;
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
                    
                    // แปลง JSON array
                    CardGameHistory[] allHistories = JsonUtility.FromJson<CardGameHistoryWrapper>("{\"items\":" + jsonResponse + "}").items;
                    
                    // กรองเฉพาะประวัติของ user นี้และเกม Card Matching
                    var userHistories = allHistories
                        .Where(h => h.User_ID == userId && h.id == "302")
                        .OrderByDescending(h => DateTime.Parse(h.Start_Time))
                        .ToArray();

                    if (userHistories != null && userHistories.Length > 0)
                    {
                        callback(userHistories);
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