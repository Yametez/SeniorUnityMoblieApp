using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginButtonController : MonoBehaviour
{
    [SerializeField] private InputField emailInput;
    [SerializeField] private InputField passwordInput;
    private Button loginButton;

    void Start()
    {
        loginButton = GetComponent<Button>();
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(HandleLogin);
        }
    }

    private void HandleLogin()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        // ตรวจสอบว่ากรอกข้อมูลครบหรือไม่
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.Log("กรุณากรอกข้อมูลให้ครบ");
            return;
        }

        // ตรวจสอบการล็อกอินผ่าน DBManager
        if (DBManager.ValidateLogin(email, password))
        {
            Debug.Log("เข้าสู่ระบบสำเร็จ");
            SceneManager.LoadScene("FistHomePage");
        }
        else
        {
            Debug.Log("อีเมลหรือรหัสผ่านไม่ถูกต้อง");
        }
    }
} 