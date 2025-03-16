using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine.SceneManagement;

public class RegisterButtonController : MonoBehaviour
{
    [SerializeField] private InputField emailInput;
    [SerializeField] private InputField passwordInput;
    [SerializeField] private InputField nameInput;
    [SerializeField] private InputField surnameInput;
    [SerializeField] private InputField ageInput;
    [SerializeField] private ToggleGroup genderToggle;
    [SerializeField] private Toggle acceptTermsToggle;
    [SerializeField] private TermsPopupController termsPopupController;
    private Button registerButton;

    private string apiUrl = "https://seniorunitymoblieapp.onrender.com/api/users/";

    // ย้าย UserData class มาไว้นอก method
    [System.Serializable]
    private class UserData
    {
        public string Name;
        public string Surname;
        public string Email;
        public string Password;
        public int Age;
        public string Gender;
    }

    void Start()
    {
        registerButton = GetComponent<Button>();
        if (registerButton != null)
        {
            registerButton.onClick.AddListener(HandleRegister);
        }
    }

    private void HandleRegister()
    {
        if (!ValidateInputs())
        {
            AlertManager.ShowAlert("กรุณากรอกข้อมูลให้ครบทุกช่อง");
            return;
        }

        if (!acceptTermsToggle.isOn)
        {
            AlertManager.ShowAlert("กรุณายอมรับเงื่อนไขการใช้งาน");
            return;
        }

        string jsonData = CreateJsonData();
        StartCoroutine(RegisterUser(jsonData));
    }

    private string CreateJsonData()
    {
        Toggle selectedGender = null;
        foreach (Toggle toggle in genderToggle.GetComponentsInChildren<Toggle>())
        {
            if (toggle.isOn)
            {
                selectedGender = toggle;
                break;
            }
        }

        var userData = new UserData
        {
            Name = nameInput.text.Trim(),
            Surname = surnameInput.text.Trim(),
            Email = emailInput.text.Trim(),
            Password = passwordInput.text.Trim(),
            Age = int.Parse(ageInput.text.Trim()),
            Gender = selectedGender.name == "MaleToggle" ? "Male" : "Female"
        };

        string jsonData = JsonUtility.ToJson(userData);
        Debug.Log($"Generated JSON: {jsonData}");
        return jsonData;
    }

    private IEnumerator RegisterUser(string jsonData)
    {
        Debug.Log($"Sending request to: {apiUrl}");
        Debug.Log($"With data: {jsonData}");

        var request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        
        // เพิ่ม headers ให้ครบถ้วน
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Content-Length", bodyRaw.Length.ToString());

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Success Response: {request.downloadHandler.text}");
            AlertManager.ShowAlert("สมัครสมาชิกสำเร็จ");
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene("Login");
        }
        else
        {
            string errorMessage = request.downloadHandler.text;
            Debug.LogError($"Error: {request.error}");
            Debug.LogError($"Response: {errorMessage}");
            AlertManager.ShowAlert("เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง");
        }

        request.Dispose();
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrEmpty(emailInput.text) ||
            string.IsNullOrEmpty(passwordInput.text) ||
            string.IsNullOrEmpty(nameInput.text) ||
            string.IsNullOrEmpty(surnameInput.text) ||
            string.IsNullOrEmpty(ageInput.text) ||
            !genderToggle.AnyTogglesOn())
        {
            return false;
        }

        // เช็คความยาวของข้อมูล
        if (nameInput.text.Length > 50 ||
            surnameInput.text.Length > 50 ||
            emailInput.text.Length > 255 ||
            passwordInput.text.Length > 6)
        {
            AlertManager.ShowAlert("ข้อมูลบางอย่างยาวเกินกำหนด");
            return false;
        }

        // เช็คค่าอายุเป็นตัวเลข
        if (!int.TryParse(ageInput.text, out int age))
        {
            AlertManager.ShowAlert("กรุณากรอกอายุเป็นตัวเลข");
            return false;
        }

        return true;
    }

    private void OnTermsToggleClicked(bool isOn)
    {
        if (isOn)
        {
            termsPopupController.Show();
            acceptTermsToggle.isOn = false;
        }
    }
} 