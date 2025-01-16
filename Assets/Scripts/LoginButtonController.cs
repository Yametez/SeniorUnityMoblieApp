using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginButtonController : MonoBehaviour
{
    private Button loginButton;

    void Start()
    {
        loginButton = GetComponent<Button>();
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(() => {
                SceneManager.LoadScene("FistHomePage");
            });
        }
    }
} 