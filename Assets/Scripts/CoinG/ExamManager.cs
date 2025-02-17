using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

[Serializable]
public class ResultData
{
    public float speed;
    public float accuracy;
    public float memory;
}

[Serializable]
public class ExamData
{
    public string Exame_name;
    public string Start_Time;
    public string End_Time;
    public int Time_limit;
    public ResultData Result_Exam;
}

public class ExamManager : MonoBehaviour
{
    private const string API_URL = "http://localhost:3000/api/exam/";
    private bool isSaving = false;
    private string lastSavedData = "";
    private bool hasBeenSaved = false;

    public void SaveExamResult(string examName, DateTime startTime, DateTime endTime, float actualPlayTime, float speed, float accuracy, float memory, string evaluationResult, string advice)
    {
        if (hasBeenSaved)
        {
            Debug.Log("This exam result has already been saved.");
            return;
        }

        if (isSaving)
        {
            Debug.Log("Already saving exam result, please wait...");
            return;
        }

        // ดึงข้อมูล User จาก UserManager แทน
        UserData currentUser = UserManager.Instance.GetCurrentUser();
        if (currentUser == null)
        {
            Debug.LogError("No user data available in UserManager. Cannot save exam result.");
            return;
        }

        // แปลงเวลาเป็นวินาที และปัดเศษทศนิยม
        int timeInSeconds = Mathf.RoundToInt(actualPlayTime);

        // กำหนดค่า id ตามชื่อเกม
        int gameId = examName == "Coin Game" ? 301 : 302;

        string jsonData = $@"{{
            ""User_ID"": {currentUser.userId},
            ""id"": {gameId},
            ""Exame_name"": ""{examName}"",
            ""Start_Time"": ""{startTime:yyyy-MM-dd HH:mm:ss}"",
            ""End_Time"": ""{endTime:yyyy-MM-dd HH:mm:ss}"",
            ""Time_limit"": {timeInSeconds},
            ""Result_Exam"": {{
                ""speed"": {(int)speed},
                ""accuracy"": {(int)accuracy},
                ""memory"": {(int)memory},
                ""evaluation"": ""{evaluationResult}"",
                ""advice"": ""{advice.Replace("\n", "\\n")}""
            }}
        }}";

        // เช็คว่าข้อมูลซ้ำกับครั้งล่าสุดหรือไม่
        if (jsonData == lastSavedData)
        {
            Debug.Log("Duplicate data detected, skipping save...");
            return;
        }

        StartCoroutine(CreateExamRecord(jsonData));
    }

    private IEnumerator CreateExamRecord(string jsonData)
    {
        isSaving = true;

        try
        {
            using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"Exam result saved successfully: {request.downloadHandler.text}");
                    lastSavedData = jsonData;
                    hasBeenSaved = true;
                }
                else
                {
                    Debug.LogError($"Error saving exam result: {request.error}");
                    Debug.LogError($"Response: {request.downloadHandler.text}");
                }
            }
        }
        finally
        {
            isSaving = false;
        }
    }

    public void ResetSaveStatus()
    {
        hasBeenSaved = false;
        lastSavedData = "";
    }
}

// Wrapper class สำหรับ JsonUtility
[Serializable]
public class JsonWrapper<T>
{
    public T data;

    public JsonWrapper(T data)
    {
        this.data = data;
    }
}