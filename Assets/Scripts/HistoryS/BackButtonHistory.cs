using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonHistory : MonoBehaviour
{
    public void BackToFirstHomePage()
    {
        // โหลด Scene FistHomePage
        SceneManager.LoadScene("FistHomePage");
    }
    public void BackToHistoryExam()
    {
        // โหลด Scene HistoryExam
        SceneManager.LoadScene("HistoryExam");
    }
    public void BackToHistoryCG()
    {
        // โหลด Scene HistoryCG
        SceneManager.LoadScene("HistoryCG");
    }
} 