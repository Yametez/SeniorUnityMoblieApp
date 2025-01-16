using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFirstHomePage : MonoBehaviour
{
    public void GoToGameCoin()
    {
        SceneManager.LoadScene("Coingame");
    }
}