using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public Button loginButton;
    public Button goToRegisterButton;

    void Start()
    {
        loginButton.onClick.AddListener(Login);
        goToRegisterButton.onClick.AddListener(GoToRegister);
    }

    void Login()
    {
        SceneManager.LoadScene("FistHomePage");
    }

    void GoToRegister()
    {
        SceneManager.LoadScene("Register");
    }
}
