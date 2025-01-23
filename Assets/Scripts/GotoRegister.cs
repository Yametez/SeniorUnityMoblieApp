using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoRegister : MonoBehaviour
{
    public void GoToRegisterScene()
    {
        SceneManager.LoadScene("Register");
    }
} 