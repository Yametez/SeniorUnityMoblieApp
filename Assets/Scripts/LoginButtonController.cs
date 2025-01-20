using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            AlertManager.ShowAlert("กรุณากรอกข้อมูลให้ครบ");
            return;
        }

        if (DBManager.ValidateLogin(email, password))
        {
            SceneManager.LoadScene("FistHomePage");
        }
        else
        {
            AlertManager.ShowAlert("อีเมลหรือรหัสผ่านไม่ถูกต้อง");
        }
    }
} 