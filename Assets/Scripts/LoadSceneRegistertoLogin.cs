using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RegisterManager : MonoBehaviour
{
    public Button registerButton;

    void Start()
    {
        // ใส่ Listener เมื่อกดปุ่ม
        registerButton.onClick.AddListener(ChangeScene);
    }

    void ChangeScene()
    {
        // เปลี่ยนซีนเมื่อกดปุ่ม
        SceneManager.LoadScene("Login"); // แทน "MainMenu" ด้วยชื่อซีนที่ต้องการเปลี่ยนไป
    }
}
