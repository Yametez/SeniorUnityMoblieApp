using UnityEngine;
using UnityEngine.UI;

public class TermsPopupController : MonoBehaviour
{
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;
    [SerializeField] private Toggle termsToggle;
    
    void Start()
    {
        if (acceptButton != null)
            acceptButton.onClick.AddListener(OnAccept);
            
        if (rejectButton != null)
            rejectButton.onClick.AddListener(OnReject);
            
        // เพิ่ม Debug Log
        Debug.Log("TermsPopupController Started");
        gameObject.SetActive(false);
    }

    public void Show()
    {
        Debug.Log("Show called");
        gameObject.SetActive(true);
    }

    private void OnAccept()
    {
        Debug.Log("Accept clicked");
        termsToggle.isOn = true;
        gameObject.SetActive(false);
    }

    private void OnReject()
    {
        Debug.Log("Reject clicked");
        termsToggle.isOn = false;
        gameObject.SetActive(false);
    }
} 