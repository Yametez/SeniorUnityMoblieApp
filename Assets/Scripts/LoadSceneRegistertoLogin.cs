using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneRegistertoLogin : MonoBehaviour
{
    public Button registerButton;

    void Start()
    {
        if (registerButton != null)
        {
            registerButton.onClick.AddListener(ChangeScene);
        }
        else
        {
            Debug.LogError("Register button is not assigned!");
        }
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("Login");
    }
}
