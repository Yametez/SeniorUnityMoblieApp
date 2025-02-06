using UnityEngine;
using UnityEngine.UI;

public class ResultAnalysis : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Slider accuracySlider;
    [SerializeField] private Slider memorySlider;

    [Header("Labels")]
    [SerializeField] private Text speedText;
    [SerializeField] private Text accuracyText;
    [SerializeField] private Text memoryText;
    [SerializeField] private Text resultText;
    [SerializeField] private Text adviceText;

    public void UpdateResults(float speed, float accuracy, float memory)
    {
        // อัพเดทค่า Sliders
        speedSlider.value = speed;
        accuracySlider.value = accuracy;
        memorySlider.value = memory;

        // อัพเดทข้อความ
        speedText.text = $"ความเร็ว: {speed:F0}%";
        accuracyText.text = $"ความแม่นยำ: {accuracy:F0}%";
        memoryText.text = $"ความจำ: {memory:F0}%";

        // คำนวณผลรวม
        float average = (speed + accuracy + memory) / 3f;
        UpdateResult(average);
    }

    private void UpdateResult(float score)
    {
        if (score >= 60f)
        {
            resultText.text = "ผลการประเมิน:ไม่พบความเสี่ยง";
            adviceText.text = "สมองของคุณทำงานได้ดี\nควรรักษาสุขภาพสมองด้วย\nการออกกำลังกายสม่ำเสมอ";
            resultText.color = Color.green;
        }
        else if (score >= 40f)
        {
            resultText.text = "ผลการประเมิน:พบความเสี่ยงต่ำ";
            adviceText.text = "ควรเพิ่มการฝึกฝนความจำและการคิด\nแนะนำให้ปรึกษาแพทย์";
            resultText.color = Color.yellow;
        }
        else
        {
            resultText.text = "ผลการประเมิน:พบความเสี่ยงสูง";
            adviceText.text = "แนะนำให้พบแพทย์โดยเร็ว\nควรได้รับการตรวจประเมินอย่างละเอียด";
            resultText.color = Color.red;
        }
    }
}