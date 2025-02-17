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
    private string userId; // ID ของผู้ใช้ที่ login

    void Start()
    {
        // เก็บข้อมูลจาก Login
        userId = PlayerPrefs.GetString("UserID");
        string username = PlayerPrefs.GetString("Username");
        
        // เพิ่ม listener ให้กับปุ่ม
        if (detailButton != null)
        {
            detailButton.onClick.AddListener(OnDetailButtonClick);
        }
        else
        {
            Debug.LogError("Detail button is not assigned!");
        }
    }

    public void SetData(ExamHistory history)
    {
        if (history == null)
        {
            Debug.LogError("Trying to set null history data!");
            return;
        }

        historyData = history;
        
        // แปลง string เป็น DateTime
        DateTime startTime = DateTime.Parse(history.Start_Time);
        
        // จัดรูปแบบวันที่ให้เป็นภาษาไทย
        if (dateText != null)
        {
            dateText.text = startTime.ToString("dd/MM/yyyy");
        }
        
        if (examNameText != null)
        {
            examNameText.text = history.Exame_name;
        }
        
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

    private void OnDetailButtonClick()
    {
        if (historyData == null)
        {
            Debug.LogError("No history data available!");
            return;
        }

        try
        {
            // เก็บข้อมูลทั้งหมดที่จำเป็น
            PlayerPrefs.SetString("SelectedExamId", historyData.Exam_ID);        // ลำดับงาน
            PlayerPrefs.SetString("SelectedGameId", historyData.id);             // รหัสเกม (301)
            PlayerPrefs.SetString("SelectedExamName", historyData.Exame_name);
            PlayerPrefs.SetString("SelectedUserId", userId);
            PlayerPrefs.SetString("SelectedExamDate", historyData.Start_Time);
            
            // Debug log เพื่อตรวจสอบข้อมูล
            Debug.Log($"Sending to detail - ExamID: {historyData.Exam_ID}, GameID: {historyData.id}");
            Debug.Log($"Date: {historyData.Start_Time}, Name: {historyData.Exame_name}");
            
            SceneManager.LoadScene("HistoryDetail");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in OnDetailButtonClick: {e.Message}");
        }
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