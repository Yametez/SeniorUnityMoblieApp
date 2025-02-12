using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFirstHomePage : MonoBehaviour
{
    public void GoToGameCoin()
    {
        SceneManager.LoadScene("Coingame");
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene("Login");
    }

    public void GoToHistoryExam()
    {
        SceneManager.LoadScene("HistoryExam");
    }
}