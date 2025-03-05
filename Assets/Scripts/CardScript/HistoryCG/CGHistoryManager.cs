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
            StartCoroutine(apiManager.GetUserHistory(currentUser.userId.ToString(), OnHistoryLoaded));
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

        // ลบประวัติเก่าออกก่อน
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // ตั้งค่า Content ก่อนสร้าง items
        RectTransform contentRect = contentParent.GetComponent<RectTransform>();
        contentRect.anchoredPosition = Vector2.zero; // ตั้งตำแหน่งเริ่มต้นที่ 0,0
        contentRect.anchorMin = new Vector2(0, 1); // ยึดด้านบน
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1); // pivot อยู่ด้านบนกลาง

        // แสดงประวัติทั้งหมด
        foreach (var history in histories)
        {
            try
            {
                GameObject item = Instantiate(historyItemPrefab, contentParent);
                CGHistoryItem historyItem = item.GetComponent<CGHistoryItem>();
                
                // ตั้งค่า RectTransform ของ item
                RectTransform rectTransform = item.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 1); // ยึดด้านบน
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 1);
                rectTransform.sizeDelta = new Vector2(0, 80); // ลดความสูงลงเหลือ 80
                
                if (historyItem != null)
                {
                    DateTime startTime = DateTime.Parse(history.Start_Time);
                    historyItem.SetData(
                        startTime.ToString("dd/MM/yyyy HH:mm:ss"),
                        history.Training_name,
                        int.Parse(history.Training_ID)
                    );
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating history item: {e.Message}");
            }
        }

        // อัพเดท layout ทันที
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
    }

    public void BackToHome()
    {
        SceneManager.LoadScene("FistHomePage");
    }
} 