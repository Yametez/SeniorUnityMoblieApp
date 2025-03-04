using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class CGHistoryManager : MonoBehaviour
{
    public GameObject historyItemPrefab;
    public Transform contentParent;
    private CGHistoryApiManager apiManager;

    void Start()
    {
        apiManager = GetComponent<CGHistoryApiManager>();
        LoadHistory();
    }

    void LoadHistory()
    {
        UserData currentUser = CurrentUser.GetCurrentUser();
        if (currentUser != null)
        {
            // โหลดข้อมูลหลายครั้งเพื่อแสดงประวัติหลายรายการ
            for (int i = 0; i < 5; i++)  // สมมติว่าต้องการแสดง 5 รายการล่าสุด
            {
                StartCoroutine(apiManager.GetUserHistory(currentUser.userId.ToString(), OnHistoryLoaded));
            }
        }
        else
        {
            Debug.LogWarning("No user data found!");
        }
    }

    void OnHistoryLoaded(CGHistoryApiManager.CardGameHistory[] histories)
    {
        if (histories == null || histories.Length == 0)
        {
            Debug.LogError("Failed to load history or no history found");
            return;
        }

        // สร้าง history item ใหม่
        foreach (var history in histories)
        {
            try
            {
                GameObject item = Instantiate(historyItemPrefab, contentParent);
                CGHistoryItem historyItem = item.GetComponent<CGHistoryItem>();
                
                if (historyItem != null)
                {
                    DateTime startTime = DateTime.Parse(history.Start_Time);
                    historyItem.SetData(
                        startTime.ToString("dd/MM/yyyy HH:mm:ss"),  // เพิ่มเวลาในการแสดงผล
                        history.Training_name,
                        history.Training_ID
                    );
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating history item: {e.Message}");
            }
        }
    }

    public void BackToHome()
    {
        SceneManager.LoadScene("FistHomePage");
    }
} 