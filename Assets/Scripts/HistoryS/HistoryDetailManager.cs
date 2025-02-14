using UnityEngine;
using UnityEngine.UI;
using System;

public class HistoryDetailManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text dateText;
    [SerializeField] private Text examNameText;
    [SerializeField] private Text userIdText;
    [SerializeField] private Text totalScoreText;
    [SerializeField] private Text resultText;
    [SerializeField] private Text adviceText;

    private void Start()
    {
        // ดึงข้อมูลที่ส่งมาจาก Scene ก่อนหน้า
        string examId = PlayerPrefs.GetString("SelectedExamId");
        string examName = PlayerPrefs.GetString("SelectedExamName");
        string userId = PlayerPrefs.GetString("SelectedUserId");
        string examDate = PlayerPrefs.GetString("SelectedExamDate");

        // แสดงข้อมูลพื้นฐาน
        examNameText.text = $"การทดสอบ: {examName}";
        dateText.text = $"วันที่: {examDate}";
        userIdText.text = $"รหัสผู้ใช้: {userId}";

        // โหลดข้อมูลรายละเอียดเพิ่มเติมจาก API หรือ Database
        LoadExamDetails(examId);
    }

    private async void LoadExamDetails(string examId)
    {
        // TODO: เพิ่มโค้ดสำหรับโหลดข้อมูลรายละเอียดจาก API
        // ตัวอย่างการแสดงผล
        totalScoreText.text = "คะแนนรวม: 85/100";
        resultText.text = "ผลการประเมิน: ปกติ";
        adviceText.text = "คำแนะนำ: ควรฝึกฝนต่อเนื่อง";
    }
} 