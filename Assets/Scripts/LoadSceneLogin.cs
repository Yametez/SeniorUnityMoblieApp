using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public Button loginButton;
    public Button goToRegisterButton;

    void Start()
    {
        // ใส่ Listener เมื่อกดปุ่ม Login
        loginButton.onClick.AddListener(Login);

        // ใส่ Listener เมื่อกดปุ่ม Go to Register
        goToRegisterButton.onClick.AddListener(GoToRegister);
    }

    void Login()
    {
        // ทำการ Login โดยตรง เช่น เช็คข้อมูลผู้ใช้
        Debug.Log("Logging in...");
        // ตัวอย่างเชื่อมต่อฐานข้อมูลหรือการตรวจสอบผู้ใช้
        // ตัวอย่าง: SceneManager.LoadScene("MainMenu");
    }

    void GoToRegister()
    {
        // เปลี่ยนซีนไปยังหน้า Register เมื่อกดปุ่ม Go to Register
        SceneManager.LoadScene("Register"); // แทน "RegisterScene" ด้วยชื่อซีนที่เก็บหน้า Register ของคุณ
    }
}
