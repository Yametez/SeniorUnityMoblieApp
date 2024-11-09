using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class Register : MonoBehaviour
{
    public InputField nameInput;
    public InputField surnameInput;
    public InputField passwordInput;
    public InputField ageInput;
    public Dropdown genderDropdown;
    public Button registerButton;
    public Text messageText;

    void Start()
    {
        registerButton.onClick.AddListener(() => StartCoroutine(RegisterUser()));
    }

    IEnumerator RegisterUser()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", nameInput.text);
        form.AddField("surname", surnameInput.text);
        form.AddField("password", passwordInput.text);
        form.AddField("age", int.Parse(ageInput.text));
        form.AddField("gender", genderDropdown.options[genderDropdown.value].text);

        UnityWebRequest www = UnityWebRequest.Post("http://yourserver.com/register.php", form);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            messageText.text = "Error: " + www.error;
        }
        else
        {
            messageText.text = www.downloadHandler.text;
        }
    }
}
