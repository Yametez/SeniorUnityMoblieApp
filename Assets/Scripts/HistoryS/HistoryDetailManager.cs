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
    [SerializeField] private Text speedText;
    [SerializeField] private Text accuracyText;
    [SerializeField] private Text memoryText;
    [SerializeField] private Text evaluationText;
    [SerializeField] private Text adviceText;
    [SerializeField] private Text timeResultText;
    
    [Header("Progress Bars")]
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Slider accuracySlider;
    [SerializeField] private Slider memorySlider;

    private string apiUrl = "http://localhost:3000/api/exam/detail/";

    [System.Serializable]
    private class ExamResult
    {
        public int speed;
        public int accuracy;
        public int memory;
        public string evaluation;
        public string advice;
    }

    [System.Serializable]
    private class ExamData
    {
        public string Exam_ID;  // ลำดับงาน
        public string User_ID;
        public string id;       // รหัสเกม (301 = Coin Game)
        public string Exame_name;
        public string Start_Time;
        public string End_Time;
        public string Time_limit;
        public ExamResult Result_Exam;  // เปลี่ยนจาก string เป็น ExamResult
    }

    private async void Start()
    {
        string examId = PlayerPrefs.GetString("SelectedExamId");
        string userId = PlayerPrefs.GetString("SelectedUserId");
        string username = PlayerPrefs.GetString("Username");

        Debug.Log($"Loading details - ExamID: {examId}");

        try
        {
            using (UnityWebRequest www = UnityWebRequest.Get($"{apiUrl}{examId}"))
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

                // แปลง JSON response เป็น ExamData
                var examData = JsonUtility.FromJson<ExamData>(jsonResponse);
                
                if (examData != null)
                {
                    // แสดงข้อมูลพื้นฐาน
                    if (dateText != null) dateText.text = $"วันที่เล่น {DateTime.Parse(examData.Start_Time).ToString("dd/MM/yyyy")}";
                    if (examNameText != null) examNameText.text = examData.Exame_name;
                    if (userIdText != null) userIdText.text = $"สวัสดี {username}";
                    
                    // แสดงเวลาที่ใช้
                    if (timeResultText != null)
                    {
                        float timeLimit = float.Parse(examData.Time_limit);
                        if (timeLimit >= 60)
                        {
                            int minutes = Mathf.FloorToInt(timeLimit / 60);
                            timeResultText.text = $"เวลาที่ใช้: {minutes} นาที";
                        }
                        else
                        {
                            timeResultText.text = $"เวลาที่ใช้: {timeLimit:F0} วินาที";
                        }
                    }

                    // ตรวจสอบและแสดงผลการทดสอบ
                    if (examData.Result_Exam != null)
                    {
                        // อัพเดทเปอร์เซ็นต์
                        if (speedText != null) speedText.text = $"ความเร็ว: {examData.Result_Exam.speed}%";
                        if (accuracyText != null) accuracyText.text = $"ความแม่นยำ: {examData.Result_Exam.accuracy}%";
                        if (memoryText != null) memoryText.text = $"ความจำ: {examData.Result_Exam.memory}%";

                        // อัพเดท Progress Bars
                        if (speedSlider != null) 
                        {
                            speedSlider.maxValue = 100f;  // กำหนดค่าสูงสุด
                            speedSlider.value = examData.Result_Exam.speed;  // ไม่ต้องหารด้วย 100
                        }
                        if (accuracySlider != null)
                        {
                            accuracySlider.maxValue = 100f;
                            accuracySlider.value = examData.Result_Exam.accuracy;
                        }
                        if (memorySlider != null)
                        {
                            memorySlider.maxValue = 100f;
                            memorySlider.value = examData.Result_Exam.memory;
                        }

                        // แสดงผลการประเมินและคำแนะนำ
                        if (evaluationText != null) evaluationText.text = $"ผลการประเมิน: {examData.Result_Exam.evaluation}";
                        if (adviceText != null) adviceText.text = examData.Result_Exam.advice;
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