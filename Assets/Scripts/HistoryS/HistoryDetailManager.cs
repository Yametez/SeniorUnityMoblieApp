using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class HistoryDetailManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text dateText;
    [SerializeField] private Text examNameText;
    [SerializeField] private Text userIdText;
    [SerializeField] private Text resultText;
    [SerializeField] private Text adviceText;
    [SerializeField] private Slider memorySlider;
    [SerializeField] private Slider accuracySlider;
    [SerializeField] private Slider speedSlider;

    private string apiUrl = "http://localhost:3000/exam/"; // API URL

    [System.Serializable]
    private class ExamData
    {
        public string Exam_ID;      // ลำดับงาน
        public string id;           // รหัสเกม (301=Coin Game)
        public string User_ID;
        public string Exame_name;
        public string Start_Time;
        public string End_Time;
        public string Time_limit;
        public string Result_Exam;  // เป็น JSON string จาก DB
    }

    private async void Start()
    {
        // ดึงข้อมูลที่ส่งมาจากหน้า HistoryExam
        string examId = PlayerPrefs.GetString("SelectedExamId");
        string userId = PlayerPrefs.GetString("SelectedUserId");
        string username = PlayerPrefs.GetString("Username", "N/A");

        Debug.Log($"Loading exam details - ExamID: {examId}, UserID: {userId}");

        try
        {
            // เรียก API เพื่อดึงข้อมูลรายละเอียด
            ExamData examData = await FetchExamDetails(examId);
            
            if (examData != null)
            {
                // แสดงข้อมูลพื้นฐาน
                dateText.text = $"วันที่เล่น {DateTime.Parse(examData.Start_Time).ToString("dd/MM/yyyy")}";
                examNameText.text = examData.Exame_name;
                userIdText.text = $"สวัสดี {username}";

                // แปลง Result_Exam จาก JSON string เป็น object
                if (!string.IsNullOrEmpty(examData.Result_Exam))
                {
                    try
                    {
                        var resultData = JsonUtility.FromJson<ResultData>(examData.Result_Exam);
                        
                        // ตั้งค่า Sliders
                        speedSlider.value = resultData.speed;
                        accuracySlider.value = resultData.accuracy;
                        memorySlider.value = resultData.memory;

                        // แสดงผลการประเมิน
                        resultText.text = $"ผลการประเมิน: {resultData.evaluation}";
                        adviceText.text = resultData.advice;

                        Debug.Log($"Loaded result data - Speed: {resultData.speed}, Accuracy: {resultData.accuracy}, Memory: {resultData.memory}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error parsing Result_Exam: {e.Message}");
                        resultText.text = "ไม่สามารถแสดงผลการประเมินได้";
                    }
                }
                else
                {
                    Debug.LogWarning("No Result_Exam data available");
                    resultText.text = "ไม่พบข้อมูลผลการประเมิน";
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading exam details: {e.Message}");
            resultText.text = "ไม่สามารถโหลดข้อมูลได้";
        }
    }

    [System.Serializable]
    private class ResultData
    {
        public float speed;
        public float accuracy;
        public float memory;
        public string evaluation;
        public string advice;
    }

    private async Task<ExamData> FetchExamDetails(string examId)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl + examId))
        {
            try
            {
                var operation = www.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to fetch exam data: {www.error}");
                    return null;
                }

                string jsonResult = www.downloadHandler.text;
                Debug.Log($"API Response: {jsonResult}"); // เพิ่ม debug log
                return JsonUtility.FromJson<ExamData>(jsonResult);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in FetchExamDetails: {e.Message}");
                return null;
            }
        }
    }
} 