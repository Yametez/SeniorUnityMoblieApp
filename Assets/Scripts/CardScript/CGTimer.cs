using UnityEngine;
using UnityEngine.UI;

public class CGTimer : MonoBehaviour
{
    public Text timerText;
    private float currentTime = 0f;
    private bool isTimerRunning = true;

    void Start()
    {
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        // แปลงเวลาเป็นนาทีและวินาที
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        
        // แสดงในรูปแบบ MM:SS
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = 0f;
        isTimerRunning = true;
        UpdateTimerDisplay();
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    public void PauseTimer()
    {
        isTimerRunning = false;
    }

    public void ResumeTimer()
    {
        isTimerRunning = true;
    }
} 