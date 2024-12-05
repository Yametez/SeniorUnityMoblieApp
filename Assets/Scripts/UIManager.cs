using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text coinCount1Bath;
    public Text coinCount5Bath;
    public Text coinCount10Bath;
    public GameObject resultPanel;
    public Text finalTimeText;
    public Text finalScoreText;
    
    public void UpdateCoinCount(int value, int count)
    {
        switch (value)
        {
            case 1:
                coinCount1Bath.text = $"x{count}";
                break;
            case 5:
                coinCount5Bath.text = $"x{count}";
                break;
            case 10:
                coinCount10Bath.text = $"x{count}";
                break;
        }
    }

    public void ShowResults(float totalTime, int totalScore)
    {
        resultPanel.SetActive(true);
        finalTimeText.text = $"Time: {(int)totalTime/60:00}:{(int)totalTime%60:00}";
        finalScoreText.text = $"Score: {totalScore}";
    }
} 