using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Linq; // เพิ่มเพื่อใช้ OrderByDescending

public class HistoryManager : MonoBehaviour
{
    [SerializeField] private GameObject historyItemPrefab;
    [SerializeField] private Transform contentParent; // ScrollView's content
    [SerializeField] private string apiUrl = "http://localhost:3000/api/exam"; // ตรวจสอบ URL ให้ถูกต้อง
    
    private string userId; // จะได้มาจาก PlayerPrefs

    void Start()
    {
        userId = PlayerPrefs.GetString("UserID");
        Debug.Log($"Current UserID from PlayerPrefs: {userId}"); // เช็คค่า UserID ที่ได้
        StartCoroutine(LoadHistoryData());
    }

    [Serializable]
    private class ExamList
    {
        public List<ExamHistory> exams;
    }

    IEnumerator LoadHistoryData()
    {
        Debug.Log($"Fetching data from: {apiUrl}");

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"API Response: {jsonResponse}");

                try
                {
                    List<ExamHistory> histories = JsonUtility.FromJson<ExamList>("{\"exams\":" + jsonResponse + "}").exams;
                    Debug.Log($"Total histories before filter: {histories.Count}");

                    // เปลี่ยนมาใช้ User_ID จาก id แทน
                    var filteredHistories = histories
                        .Where(h => h.User_ID == userId || h.id == userId)  // ลองเช็คทั้ง User_ID และ id
                        .OrderByDescending(h => DateTime.Parse(h.Start_Time))
                        .ToList();

                    Debug.Log($"Filtered histories for user {userId}: {filteredHistories.Count}");

                    // ลบ history items เก่าออก
                    foreach (Transform child in contentParent)
                    {
                        Destroy(child.gameObject);
                    }

                    // สร้าง items ใหม่
                    foreach (var history in histories)  // แสดงทั้งหมดก่อนเพื่อเช็ค
                    {
                        CreateHistoryItem(history);
                        Debug.Log($"Created item - User_ID: {history.User_ID}, id: {history.id}, Game: {history.Exame_name}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error processing data: {e.Message}\nStackTrace: {e.StackTrace}");
                }
            }
            else
            {
                Debug.LogError($"Error loading history: {request.error}");
            }
        }
    }

    void CreateHistoryItem(ExamHistory history)
    {
        Debug.Log($"Attempting to create item at position: {contentParent.position}");
        GameObject item = Instantiate(historyItemPrefab, contentParent);
        Debug.Log($"Item created: {item.name} at position: {item.transform.position}");
        
        HistoryItemUI itemUI = item.GetComponent<HistoryItemUI>();
        if (itemUI != null)
        {
            itemUI.SetData(history);
        }
        else
        {
            Debug.LogError("HistoryItemUI component missing from prefab!");
        }
    }
}

[System.Serializable]
public class ExamHistory
{
    public string Exam_ID;
    public string User_ID;  // อาจจะเป็น null
    public string id;       // ใช้ field นี้แทนถ้า User_ID เป็น null
    public string Exame_name;
    public string Start_Time;
    public string End_Time;
    public string Time_limit;
    public ResultExam Result_Exam;
}

[System.Serializable]
public class ResultExam
{
    public int score;
    public float timeSpent;
    public string difficulty;
    // เพิ่มฟิลด์อื่นๆ ตามที่มีใน JSON
} 