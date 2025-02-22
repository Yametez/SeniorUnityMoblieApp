using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;  // เพิ่ม namespace สำหรับ UnityWebRequest
using System;
using System.Threading.Tasks;

public class LatestScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text scoreText;        // ตัวเลขในวงกลมสีเขียว
    [SerializeField] private Text evaluationText;   // คำแปล
    [SerializeField] private Text adviceText;       // คำแนะนำ
    [SerializeField] private Text userIdText;       // ชื่อผู้ใช้
    [SerializeField] private Text dateText;         // วันที่เล่น

    [Header("Total Progress Bar")]
    [SerializeField] private Slider totalScoreSlider;   // Progress Bar รวมคะแนน

    [Header("Time")]
    [SerializeField] private Text timeText;         // เวลาที่ใช้

    [Header("Default Values")]
    [SerializeField] private string defaultScore = "00";  // ค่าเริ่มต้นคะแนน
    [SerializeField] private string defaultEvaluation = "ไม่มีประวัติ";  // ค่าเริ่มต้นผลการประเมิน
    [SerializeField] private string defaultAdvice = "ลองทำแบบประเมินเพื่อดูคะแนนที่ได้สิ";  // ค่าเริ่มต้นคำแนะนำ

    // เพิ่ม class สำหรับเก็บข้อมูล
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
        public string Exam_ID;
        public string User_ID;
        public string id;
        public string Exame_name;
        public string Start_Time;
        public string End_Time;
        public string Time_limit;
        public ExamResult Result_Exam;
    }

    private async void Start()
    {
        try
        {
            UserData currentUser = CurrentUser.GetCurrentUser();
            if (currentUser == null || currentUser.userId <= 0)
            {
                Debug.LogWarning("User data not found!");
                return;
            }

            string userIdStr = currentUser.userId.ToString();
            Debug.Log($"Fetching data for user ID: {userIdStr}");
            string url = $"http://localhost:3000/api/exam/latest/{userIdStr}";
            
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = 10;
                var operation = request.SendWebRequest();
                
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = request.downloadHandler.text;
                    ExamData examData = JsonUtility.FromJson<ExamData>(jsonResponse);
                    
                    // ถ้าไม่พบประวัติการทดสอบ แสดงค่าเริ่มต้น
                    if (examData == null || examData.Result_Exam == null)
                    {
                        Debug.Log("No exam history found, showing default values");
                        ShowDefaultValues(currentUser);
                    }
                    else
                    {
                        UpdateUI(examData, currentUser);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in Start: {e.Message}");
        }
    }

    private void ShowDefaultValues(UserData userData)
    {
        // แสดงคะแนนเริ่มต้น
        if (scoreText != null)
            scoreText.text = defaultScore;

        // แสดงชื่อผู้ใช้
        if (userIdText != null && userData != null)
            userIdText.text = $"สวัสดี, คุณ{userData.name}";

        // แสดงวันที่ปัจจุบัน
        if (dateText != null)
            dateText.text = $"วันที่ {DateTime.Now.ToString("dd/MM/yyyy")}";

        // รีเซ็ต Progress Bar
        if (totalScoreSlider != null)
        {
            totalScoreSlider.maxValue = 100f;
            totalScoreSlider.value = 0f;
        }

        // แสดงผลการประเมินเริ่มต้น
        if (evaluationText != null)
        {
            evaluationText.text = $"ผลการประเมิน:\n{defaultEvaluation}";
            evaluationText.color = Color.black;
        }

        // แสดงคำแนะนำเริ่มต้น
        if (adviceText != null)
            adviceText.text = defaultAdvice;

        // ซ่อนเวลา
        if (timeText != null)
            timeText.text = "";
    }

    private void UpdateUI(ExamData examData, UserData userData)
    {
        if (examData != null && examData.Result_Exam != null)
        {
            // คำนวณคะแนนรวม
            float totalScore = (examData.Result_Exam.speed + 
                              examData.Result_Exam.accuracy + 
                              examData.Result_Exam.memory) / 3f;
            
            // แสดงคะแนนในวงกลม
            if (scoreText != null) 
                scoreText.text = Mathf.RoundToInt(totalScore).ToString();

            // อัพเดท Progress Bar
            if (totalScoreSlider != null)
            {
                totalScoreSlider.maxValue = 100f;
                totalScoreSlider.value = totalScore;
            }

            // แสดงชื่อผู้ใช้และวันที่
            if (userIdText != null)
                userIdText.text = $"สวัสดี, คุณ{userData.name}";
            
            if (dateText != null)
                dateText.text = $"วันที่เล่น {DateTime.Parse(examData.Start_Time).ToString("dd/MM/yyyy")}";

            // แสดงเวลา
            if (timeText != null)
            {
                float timeLimit = float.Parse(examData.Time_limit);
                int minutes = Mathf.FloorToInt(timeLimit / 60);
                int seconds = Mathf.FloorToInt(timeLimit % 60);
                timeText.text = $"เวลาที่ใช้: {minutes} นาที {seconds} วินาที";
            }

            // แสดงคำแปลและกำหนดสี
            if (evaluationText != null)
            {
                if (totalScore >= 60f)
                {
                    evaluationText.text = "ผลการประเมิน:\nไม่พบความเสี่ยง";
                    evaluationText.color = Color.green;
                }
                else if (totalScore >= 40f)
                {
                    evaluationText.text = "ผลการประเมิน:\nพบความเสี่ยงต่ำ";
                    evaluationText.color = Color.yellow;
                }
                else
                {
                    evaluationText.text = "ผลการประเมิน:\nพบความเสี่ยงสูง";
                    evaluationText.color = Color.red;
                }
            }

            // แสดงคำแนะนำ (ไม่มีการกำหนดสี)
            if (adviceText != null)
                adviceText.text = examData.Result_Exam.advice;
        }
    }
} 