using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonHistory : MonoBehaviour
{
    public void BackToFirstHomePage()
    {
        // โหลด Scene FistHomePage
        SceneManager.LoadScene("FistHomePage");
    }
} 