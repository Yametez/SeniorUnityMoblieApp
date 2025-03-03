using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlertManager : MonoBehaviour
{
    [SerializeField] private GameObject alertPanel;
    [SerializeField] private Text alertText;
    private static AlertManager instance;

    void Awake()
    {
        instance = this;
        if (alertPanel != null)
        {
            alertPanel.SetActive(false);
            Canvas alertCanvas = alertPanel.GetComponentInParent<Canvas>();
            if (alertCanvas != null)
            {
                alertCanvas.sortingOrder = 5;
            }
        }
    }

    public static void ShowAlert(string message)
    {
        if (instance != null)
        {
            instance.alertText.text = message;
            instance.alertPanel.SetActive(true);
            instance.StartCoroutine(instance.HideAlertAfterDelay());
        }
    }

    private IEnumerator HideAlertAfterDelay()
    {
        yield return new WaitForSeconds(2f); // แสดง 2 วินาที
        alertPanel.SetActive(false);
    }

    public void HideAlert()
    {
        alertPanel.SetActive(false);
    }
} 