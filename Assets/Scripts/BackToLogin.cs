using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToLogin : MonoBehaviour
{
    public void GoToLoginScene()
    {
        SceneManager.LoadScene("Login");
    }
} 