using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CGHistoryItem : MonoBehaviour
{
    public Text dateAndGameText;
    public Button arrowButton;
    private string gameDate;
    private int trainingId;

    public void SetData(string date, string gameName, int trainingID)
    {
        gameDate = date;
        trainingId = trainingID;
        dateAndGameText.text = $"วันที่ {date} {gameName}";
        
        // เพิ่ม listener ให้กับปุ่มลูกศร
        arrowButton.onClick.AddListener(OnArrowButtonClick);
    }

    public void OnArrowButtonClick()
    {
        // แปลง trainingId เป็น string ก่อนเก็บใน PlayerPrefs
        PlayerPrefs.SetString("SelectedTrainingId", trainingId.ToString());
        
        // เปลี่ยน Scene ไปที่ CGDetailHistory
        SceneManager.LoadScene("CGDetailHistory");
    }
} 