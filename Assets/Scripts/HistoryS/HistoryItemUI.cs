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
        // ดึง userId ที่เก็บไว้ตั้งแต่ login
        userId = PlayerPrefs.GetString("UserID");
        
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
        // ตรวจสอบว่ามีข้อมูลก่อนที่จะใช้
        if (historyData == null)
        {
            Debug.LogError("No history data available!");
            return;
        }

        try
        {
            // เก็บข้อมูลที่จำเป็นไว้ใน PlayerPrefs
            PlayerPrefs.SetString("SelectedExamId", historyData.Exam_ID);
            PlayerPrefs.SetString("SelectedExamName", historyData.Exame_name);
            PlayerPrefs.SetString("SelectedUserId", userId);
            PlayerPrefs.SetString("SelectedExamDate", historyData.Start_Time);
            
            // Debug log ก่อนเปลี่ยน Scene
            Debug.Log($"Saving exam data - ID: {historyData.Exam_ID}, Name: {historyData.Exame_name}");
            
            // เปลี่ยนไปยัง Scene HistoryDetail
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