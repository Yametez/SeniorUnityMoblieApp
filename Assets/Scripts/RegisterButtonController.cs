using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RegisterButtonController : MonoBehaviour
{
    private Button registerButton;

    void Start()
    {
        registerButton = GetComponent<Button>();
        if (registerButton != null)
        {
            registerButton.onClick.AddListener(() => {
                SceneManager.LoadScene("Register");
            });
        }
    }
} 