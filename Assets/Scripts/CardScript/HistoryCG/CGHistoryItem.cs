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
        // เก็บ Training ID ไว้ใน PlayerPrefs เพื่อใช้ในหน้ารายละเอียด
        PlayerPrefs.SetInt("SelectedTrainingId", trainingId);
        // เปลี่ยน Scene ไปที่ CGDetailHistory
        SceneManager.LoadScene("CGDetailHistory");
    }
} 