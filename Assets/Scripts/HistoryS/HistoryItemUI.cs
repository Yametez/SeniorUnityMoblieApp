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

    void Start()
    {
        // กำหนดขนาดของ item ให้ใหญ่ขึ้น
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 150); // เพิ่มความสูงเป็น 150
    }

    public void SetData(ExamHistory history)
    {
        // แปลง string เป็น DateTime
        DateTime startTime = DateTime.Parse(history.Start_Time);
        
        // จัดรูปแบบวันที่ให้เป็นภาษาไทย
        dateText.text = startTime.ToString("dd/MM/yyyy");
        examNameText.text = history.Exame_name;
        
        // เพิ่ม Debug
        Debug.Log($"Setting UI - Date: {dateText.text}, Game: {examNameText.text}");
        Debug.Log($"Item size: {GetComponent<RectTransform>().sizeDelta}");
        
        // ตรวจสอบว่า Text components ทำงานถูกต้อง
        if (dateText == null) Debug.LogError("dateText is null");
        if (examNameText == null) Debug.LogError("examNameText is null");
        
        // ตรวจสอบว่า parent canvas เปิดใช้งานอยู่
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            Debug.Log($"Parent canvas enabled: {parentCanvas.enabled}");
        }
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