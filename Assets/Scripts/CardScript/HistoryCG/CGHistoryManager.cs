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

        // แสดงประวัติทั้งหมด
        foreach (var history in histories)
        {
            try
            {
                GameObject item = Instantiate(historyItemPrefab, contentParent);
                CGHistoryItem historyItem = item.GetComponent<CGHistoryItem>();
                
                // ตั้งค่า RectTransform ของ item
                RectTransform rectTransform = item.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                rectTransform.pivot = new Vector2(0.5f, 0);
                rectTransform.sizeDelta = new Vector2(0, 100); // ความสูงของแต่ละ item
                
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

        // รอ 1 เฟรมแล้วค่อยอัพเดท layout
        StartCoroutine(UpdateLayoutNextFrame());
    }

    private IEnumerator UpdateLayoutNextFrame()
    {
        yield return null;
        // บังคับให้ layout อัพเดท
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
    }

    public void BackToHome()
    {
        SceneManager.LoadScene("FistHomePage");
    }
} 