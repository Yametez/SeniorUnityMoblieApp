using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class HistoryItemUI : MonoBehaviour
{
    [SerializeField] private Text dateText;        // ต้องมี
    [SerializeField] private Text examNameText;    // ต้องมี
    [SerializeField] private Button detailButton;  // ต้องมี
    
    private ExamHistory historyData;

    void Awake()
    {
        // เพิ่ม listener ให้กับปุ่ม
        if (detailButton != null)
        {
            detailButton.onClick.AddListener(OnDetailButtonClick);
        }
    }

    public void SetData(ExamHistory data)
    {
        if (data == null) return;

        historyData = data;
        
        // แปลงและแสดงวันที่
        DateTime startTime = DateTime.Parse(data.Start_Time);
        dateText.text = startTime.ToString("dd/MM/yyyy");
        
        // แสดงชื่อเกม
        examNameText.text = data.Exame_name;
        
        Debug.Log($"Setting UI data - Date: {dateText.text}, Game: {examNameText.text}");
    }

    public void OnDetailButtonClick()
    {
        // บันทึกข้อมูลที่จะส่งไปหน้ารายละเอียด
        PlayerPrefs.SetString("SelectedExamID", historyData.Exam_ID);
        PlayerPrefs.Save();
        
        // เปิดหน้ารายละเอียด
        SceneManager.LoadScene("ExamDetail");
    }

    void OnDestroy()
    {
        // ลบ listener เมื่อ GameObject ถูกทำลาย
        if (detailButton != null)
        {
            detailButton.onClick.RemoveListener(OnDetailButtonClick);
        }
    }
} 