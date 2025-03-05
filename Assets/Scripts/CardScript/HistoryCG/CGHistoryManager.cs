using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class CGHistoryManager : MonoBehaviour
{
    public GameObject historyItemPrefab;
    public Transform contentParent;
    public Text noHistoryText;
    private CGHistoryApiManager apiManager;

    void Start()
    {
        apiManager = GetComponent<CGHistoryApiManager>();
        LoadHistory();
        
        if (noHistoryText != null)
        {
            noHistoryText.gameObject.SetActive(false);
        }
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
            ShowNoHistoryMessage("กรุณาเข้าสู่ระบบ");
        }
    }

    void OnHistoryLoaded(CGHistoryApiManager.CardGameHistory[] histories)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (histories == null || histories.Length == 0)
        {
            Debug.Log("No history found");
            ShowNoHistoryMessage("");
            return;
        }

        if (noHistoryText != null)
        {
            noHistoryText.gameObject.SetActive(false);
        }

        foreach (var history in histories)
        {
            try
            {
                GameObject item = Instantiate(historyItemPrefab, contentParent);
                CGHistoryItem historyItem = item.GetComponent<CGHistoryItem>();
                
                RectTransform rectTransform = item.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                rectTransform.pivot = new Vector2(0.5f, 0);
                rectTransform.sizeDelta = new Vector2(0, 100);
                
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

        StartCoroutine(UpdateLayoutNextFrame());
    }

    private IEnumerator UpdateLayoutNextFrame()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
    }

    private void ShowNoHistoryMessage(string message)
    {
        if (noHistoryText != null)
        {
            noHistoryText.gameObject.SetActive(true);
            noHistoryText.text = message;
        }
    }

    public void BackToHome()
    {
        SceneManager.LoadScene("FistHomePage");
    }
} 