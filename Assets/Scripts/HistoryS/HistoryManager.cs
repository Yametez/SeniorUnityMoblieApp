using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Linq; // เพิ่มเพื่อใช้ OrderByDescending

public class HistoryManager : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private RectTransform contentParent;
    [SerializeField] private GameObject historyItemPrefab;
    [SerializeField] private string apiUrl = "http://localhost:3000/api/exam"; // ตรวจสอบ URL ให้ถูกต้อง
    
    private string userId;

    // เพิ่มตัวแปรสำหรับกำหนดค่าเริ่มต้น
    private const float INITIAL_Y_OFFSET = -335.0477f; // ตำแหน่ง Y เริ่มต้นตามที่ต้องการ -335.0477/-318.0409
    private const float ITEM_HEIGHT = 100f;
    private const float ITEM_SPACING = 10f;
    private const float BOTTOM_PADDING = 20f; // padding ด้านล่างสุด

    void Start()
    {
        // ใช้ CurrentUser แทน PlayerPrefs
        var currentUser = CurrentUser.GetCurrentUser();
        if (currentUser != null)
        {
            // แปลง userId เป็น string
            userId = currentUser.userId.ToString();
            Debug.Log($"Current UserID from CurrentUser: {userId}");
            StartCoroutine(LoadHistoryData());
        }
        else
        {
            Debug.LogError("No user currently logged in!");
        }
        
        // รีเซ็ต Content position
        RectTransform contentRT = contentParent.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(1, 1);
        contentRT.pivot = new Vector2(0.5f, 1f);
        contentRT.anchoredPosition = new Vector2(0, INITIAL_Y_OFFSET);

        // ตั้งค่า Scroll View
        if (scrollView != null)
        {
            // จำกัดการเลื่อนในแนวตั้ง
            scrollView.vertical = true;
            scrollView.horizontal = false;
            
            // ปรับ Content Size Fitter
            ContentSizeFitter sizeFitter = contentParent.GetComponent<ContentSizeFitter>();
            if (sizeFitter == null)
            {
                sizeFitter = contentParent.gameObject.AddComponent<ContentSizeFitter>();
            }
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // ปรับ Vertical Layout Group
            VerticalLayoutGroup verticalLayout = contentParent.GetComponent<VerticalLayoutGroup>();
            if (verticalLayout == null)
            {
                verticalLayout = contentParent.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            verticalLayout.spacing = 5f; // ระยะห่างระหว่างรายการ
            verticalLayout.padding = new RectOffset(10, 10, 10, 10); // ระยะขอบ
        }
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

                    // กรองข้อมูลเฉพาะของ user ที่ login โดยเทียบ User_ID
                    var filteredHistories = histories
                        .Where(h => h.User_ID == userId)  // เทียบเฉพาะ User_ID
                        .OrderByDescending(h => DateTime.Parse(h.Start_Time))
                        .ToList();

                    Debug.Log($"Filtered histories for user {userId}: {filteredHistories.Count}");

                    // ลบ history items เก่า
                    foreach (Transform child in contentParent)
                    {
                        Destroy(child.gameObject);
                    }

                    // สร้าง items
                    foreach (var history in filteredHistories)
                    {
                        CreateHistoryItem(history);
                    }

                    // อัพเดทขนาด Content หลังสร้าง items ทั้งหมด
                    UpdateContentSize();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"Error loading history: {request.error}");
            }
        }
    }

    private void CreateHistoryItem(ExamHistory history)
    {
        GameObject item = Instantiate(historyItemPrefab, contentParent);
        
        RectTransform rt = item.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1f);
        
        // คำนวณตำแหน่ง Y
        int itemCount = contentParent.childCount - 1;
        float yPosition = -(ITEM_HEIGHT + ITEM_SPACING) * itemCount;
        
        // ตั้งค่าขนาดและตำแหน่ง
        rt.sizeDelta = new Vector2(-20f, ITEM_HEIGHT);
        rt.anchoredPosition = new Vector2(0, yPosition);
        
        // ปรับขนาด Content ตามจำนวน items
        float minContentHeight = 340f; // ความสูงขั้นต่ำของ content
        float requiredHeight = (ITEM_HEIGHT + ITEM_SPACING) * (itemCount + 1) + BOTTOM_PADDING;
        float contentHeight = Mathf.Max(minContentHeight, requiredHeight);
        
        contentParent.sizeDelta = new Vector2(0, contentHeight);
        
        var historyItemUI = item.GetComponent<HistoryItemUI>();
        if (historyItemUI != null)
        {
            historyItemUI.SetData(history);
        }

        Debug.Log($"Created item at position Y: {yPosition}, Content height: {contentHeight}");
    }

    private void UpdateContentSize()
    {
        // อัพเดทขนาด Content หลังจากสร้าง items ทั้งหมด
        int totalItems = contentParent.childCount;
        float minContentHeight = 340f;
        float requiredHeight = (ITEM_HEIGHT + ITEM_SPACING) * totalItems + BOTTOM_PADDING;
        float contentHeight = Mathf.Max(minContentHeight, requiredHeight);
        
        contentParent.sizeDelta = new Vector2(0, contentHeight);
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