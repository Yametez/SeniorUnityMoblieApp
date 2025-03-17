using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class LoginButtonController : MonoBehaviour
{
    [SerializeField] private InputField emailInput;
    [SerializeField] private InputField passwordInput;
    private Button loginButton;
    private string apiUrl = "https://seniorunitymoblieapp.onrender.com/api/users/login"; // เพิ่ม endpoint สำหรับ login

    [System.Serializable]
    private class LoginData
    {
        public string Email;
        public string Password;
    }

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

        StartCoroutine(LoginUser(email, password));
    }

    private IEnumerator LoginUser(string email, string password)
    {
        var loginData = new LoginData
        {
            Email = email,
            Password = password
        };

        string jsonData = JsonUtility.ToJson(loginData);
        
        var request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // แปลงข้อมูลที่ได้เป็น UserData
            UserData userData = JsonUtility.FromJson<UserData>(request.downloadHandler.text);
            
            // เก็บข้อมูล user
            CurrentUser.SetCurrentUser(userData);
            
            SceneManager.LoadScene("FistHomePage");
        }
        else
        {
            AlertManager.ShowAlert("อีเมลหรือรหัสผ่านไม่ถูกต้อง");
        }

        request.Dispose();
    }
} 