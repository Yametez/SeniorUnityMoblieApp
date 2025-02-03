using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFirstHomePage2 : MonoBehaviour
{
    public GameObject logoutConfirmPanel; // อ้างอิงถึง Panel ยืนยันการ Logout

    void Start()
    {
        logoutConfirmPanel.SetActive(false); // ซ่อน Panel ตอนเริ่มต้น
    }

    public void ShowLogoutConfirm()
    {
        logoutConfirmPanel.SetActive(true);
    }

    public void HideLogoutConfirm()
    {
        logoutConfirmPanel.SetActive(false);
    }

    public void ConfirmLogout()
    {
        SceneManager.LoadScene("Login"); // กลับไปหน้า Login
    }
} 