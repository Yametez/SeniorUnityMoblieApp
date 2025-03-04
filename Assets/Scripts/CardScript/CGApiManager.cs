using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;

public class CGApiManager : MonoBehaviour
{
    private string apiUrl = "http://localhost:3000/api/training/";

    [System.Serializable]
    private class GameResult
    {
        public string User_ID;
        public int id;
        public string Training_name;
        public string Start_Time;
        public string End_Time;
        public string Time_limit;
        public string Result_Training;
    }

    [System.Serializable]
    private class ResultData
    {
        public float time;
        public int matches;
    }

    public IEnumerator SendGameResult(string userId, float timeSpent, int matchesFound)
    {
        ResultData resultData = new ResultData
        {
            time = timeSpent,
            matches = matchesFound
        };

        GameResult requestData = new GameResult
        {
            User_ID = userId,
            id = 302,
            Training_name = "Card Matching Game",
            Start_Time = DateTime.Now.AddSeconds(-timeSpent).ToString("yyyy-MM-dd HH:mm:ss"),
            End_Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Time_limit = timeSpent.ToString(),
            Result_Training = JsonUtility.ToJson(resultData)
        };

        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"Sending data to API: {jsonData}");

        UnityWebRequest request = null;
        
        try
        {
            request = new UnityWebRequest(apiUrl, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error preparing request: {e.Message}");
            yield break;
        }

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"API Error: {request.error}");
            Debug.LogError($"Response: {request.downloadHandler.text}");
        }
        else
        {
            Debug.Log($"API Response: {request.downloadHandler.text}");
        }

        request.Dispose();
    }

    private float CalculateSpeedScore(float timeSpent)
    {
        // คำนวณคะแนนความเร็ว (0-100)
        float maxTime = 300f; // 5 นาที
        return Mathf.Max(0, 100 * (1 - (timeSpent / maxTime)));
    }

    private float CalculateMemoryScore(float timeSpent, int matchesFound)
    {
        // คำนวณคะแนนความจำ (0-100)
        float maxTime = 300f;
        float timeScore = Mathf.Max(0, 1 - (timeSpent / maxTime));
        return matchesFound * 12.5f * timeScore; // 8 คู่ = 100 คะแนน ถ้าใช้เวลาน้อย
    }

    private string GenerateAdvice(float timeSpent)
    {
        if (timeSpent < 60f)
            return "ยอดเยี่ยม! คุณมีความจำที่ดีเยี่ยม";
        else if (timeSpent < 120f)
            return "ดีมาก! ลองฝึกต่อเพื่อพัฒนาความเร็ว";
        else
            return "ดี! ลองฝึกบ่อยๆ เพื่อพัฒนาความจำ";
    }
} 