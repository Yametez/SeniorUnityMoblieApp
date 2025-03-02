using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;  // เพิ่ม namespace สำหรับ UnityWebRequest
using System;
using System.Threading.Tasks;
using System.Collections;

public class LatestScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text scoreText;        // ตัวเลขในวงกลมสีเขียว
    [SerializeField] private Text evaluationText;   // คำแปล
    [SerializeField] private Text adviceText;       // คำแนะนำ
    [SerializeField] private Text userIdText;       // ชื่อผู้ใช้
    [SerializeField] private Text dateText;         // วันที่เล่น
    [SerializeField] private Image scoreCircleImage;  // เพิ่ม reference ไปยัง Image component ของวงกลม

    [Header("Total Progress Bar")]
    [SerializeField] private Slider totalScoreSlider;   // Progress Bar รวมคะแนน

    [Header("Time")]
    [SerializeField] private Text timeText;         // เวลาที่ใช้

    [Header("Default Values")]
    [SerializeField] private string defaultScore = "00";  // ค่าเริ่มต้นคะแนน
    [SerializeField] private string defaultEvaluation = "ไม่มีประวัติ";  // ค่าเริ่มต้นผลการประเมิน
    [SerializeField] private string defaultAdvice = "ลองทำแบบประเมินเพื่อดูคะแนนที่ได้สิ";  // ค่าเริ่มต้นคำแนะนำ

    [Header("Slider Animation")]
    [SerializeField] private float animationDuration = 1f;  // ระยะเวลาในการ animate (วินาที)

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
                ShowDefaultValues(currentUser);
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
                    Debug.Log($"API Response: {jsonResponse}");
                    
                    // แปลง JSON response เป็น anonymous object เพื่อตรวจสอบ has_history
                    var response = JsonUtility.FromJson<ApiResponse>(jsonResponse);
                    
                    if (response.has_history)
                    {
                        ExamData examData = JsonUtility.FromJson<ExamData>(jsonResponse);
                        if (examData != null && examData.Result_Exam != null)
                        {
                            UpdateUI(examData, currentUser);
                        }
                    }
                    else
                    {
                        Debug.Log("No exam history found, showing default values");
                        ShowDefaultValues(currentUser);
                    }
                }
                else
                {
                    Debug.LogError($"API Error: {request.error}");
                    ShowDefaultValues(currentUser);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in Start: {e.Message}");
            ShowDefaultValues(CurrentUser.GetCurrentUser());
        }
    }

    // เพิ่ม class สำหรับตรวจสอบ response
    [System.Serializable]
    private class ApiResponse
    {
        public bool has_history;
        public string message;
    }

    private void ShowDefaultValues(UserData userData)
    {
        if (scoreText != null)
            scoreText.text = defaultScore;  // "00"

        if (scoreCircleImage != null)
        {
            scoreCircleImage.color = new Color(0f, 1f, 1f);  // สีฟ้า (Cyan)
        }

        if (evaluationText != null)
        {
            evaluationText.text = "\n<color=#00FFFF>ไม่มีประวัติ</color>";  // สีดำสำหรับ "ผลการประเมิน:" และสีฟ้าสำหรับ "ไม่มีประวัติ"
        }

        if (adviceText != null)
            adviceText.text = "ลองทำแบบประเมิน\nเพื่อดูคะแนนที่ได้สิ";

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

        // ซ่อนเวลา
        if (timeText != null)
            timeText.text = "";
    }

    private void UpdateUI(ExamData examData, UserData userData)
    {
        if (examData != null && examData.Result_Exam != null)
        {
            float totalScore = (examData.Result_Exam.speed + 
                              examData.Result_Exam.accuracy + 
                              examData.Result_Exam.memory) / 3f;
            
            // แสดงคะแนนและกำหนดสีวงกลม
            if (scoreText != null && scoreCircleImage != null)
            {
                scoreText.text = Mathf.RoundToInt(totalScore).ToString();
                
                // กำหนดสีตามระดับคะแนน
                if (totalScore >= 60f)
                {
                    scoreCircleImage.color = Color.green;
                    evaluationText.color = Color.green;
                }
                else if (totalScore >= 40f)
                {
                    scoreCircleImage.color = Color.yellow;
                    evaluationText.color = Color.yellow;
                }
                else
                {
                    scoreCircleImage.color = Color.red;
                    evaluationText.color = Color.red;
                }
            }

            // แทนที่โค้ดเดิมของ Progress Bar ด้วยการเรียกใช้ animation
            if (totalScoreSlider != null)
            {
                totalScoreSlider.maxValue = 100f;
                totalScoreSlider.value = 0f;  // เริ่มที่ 0
                StartCoroutine(AnimateSlider(totalScore));
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
                string colorHex;
                string resultText;
                if (totalScore >= 60f)
                {
                    colorHex = "#00FF00";  // สีเขียว
                    resultText = "ไม่พบความเสี่ยง";
                }
                else if (totalScore >= 40f)
                {
                    colorHex = "#FFFF00";  // สีเหลือง
                    resultText = "พบความเสี่ยงต่ำ";
                }
                else
                {
                    colorHex = "#FF0000";  // สีแดง
                    resultText = "พบความเสี่ยงสูง";
                }
                
                evaluationText.text = $"\n<color={colorHex}>{resultText}</color>";
            }

            // แสดงคำแนะนำ (ไม่มีการกำหนดสี)
            if (adviceText != null)
                adviceText.text = examData.Result_Exam.advice;
        }
    }

    // เพิ่ม method ใหม่สำหรับ animate slider
    private IEnumerator AnimateSlider(float targetValue)
    {
        float elapsedTime = 0f;
        float startValue = totalScoreSlider.value;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, targetValue, elapsedTime / animationDuration);
            totalScoreSlider.value = currentValue;
            yield return null;
        }

        // ให้แน่ใจว่าค่าสุดท้ายตรงกับเป้าหมาย
        totalScoreSlider.value = targetValue;
    }
} 