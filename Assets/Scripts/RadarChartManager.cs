using UnityEngine;
using UnityEngine.UI;

public class RadarChartManager : MonoBehaviour
{
    [Header("Chart Settings")]
    [SerializeField] private float radius = 100f;
    [SerializeField] private Color normalRiskColor = Color.green;
    [SerializeField] private Color lowRiskColor = Color.yellow;
    [SerializeField] private Color highRiskColor = Color.red;
    
    [Header("UI References")]
    [SerializeField] private Image chartArea;
    [SerializeField] private Image[] gridLines;
    [SerializeField] private Text[] categoryLabels;
    [SerializeField] private Text[] valueLabels;
    [SerializeField] private Text resultText;
    
    [Header("Risk Thresholds")]
    [SerializeField] private float lowRiskThreshold = 60f;
    [SerializeField] private float highRiskThreshold = 40f;

    private Vector2[] points = new Vector2[3];
    private float[] values = new float[3];

    public void UpdateChartData(float speed, float accuracy, float memory)
    {
        // Normalize values (0-100)
        values[0] = Mathf.Clamp(speed, 0f, 100f);
        values[1] = Mathf.Clamp(accuracy, 0f, 100f);
        values[2] = Mathf.Clamp(memory, 0f, 100f);

        UpdateChartVisual();
        UpdateLabels();
        AssessRisk();
    }

    private void UpdateChartVisual()
    {
        // Calculate points for triangle
        for (int i = 0; i < 3; i++)
        {
            float angle = i * 120f * Mathf.Deg2Rad;
            float value = values[i] / 100f;
            points[i] = new Vector2(
                Mathf.Cos(angle) * radius * value,
                Mathf.Sin(angle) * radius * value
            );
        }

        // Update chart area shape
        Vector2[] vertices = new Vector2[3];
        for (int i = 0; i < 3; i++)
        {
            vertices[i] = points[i];
        }

        chartArea.GetComponent<PolygonCollider2D>().points = vertices;
    }

    private void UpdateLabels()
    {
        if (valueLabels != null && valueLabels.Length >= 3)
        {
            for (int i = 0; i < 3; i++)
            {
                valueLabels[i].text = $"{values[i]:F0}%";
            }
        }
    }

    private void AssessRisk()
    {
        // คำนวณค่าเฉลี่ยของทั้ง 3 ด้าน
        float averageScore = (values[0] + values[1] + values[2]) / 3f;

        string riskLevel;
        Color chartColor;

        if (averageScore >= lowRiskThreshold)
        {
            riskLevel = "ไม่มีความเสี่ยง";
            chartColor = normalRiskColor;
            resultText.text = "ผลการประเมิน: คุณมีความสามารถในการคิด จดจำ และการทำงานอยู่ในเกณฑ์ปกติ";
        }
        else if (averageScore >= highRiskThreshold)
        {
            riskLevel = "ความเสี่ยงต่ำ";
            chartColor = lowRiskColor;
            resultText.text = "ผลการประเมิน: คุณมีความเสี่ยงต่ำที่จะเป็นโรคอัลไซเมอร์ ควรฝึกฝนการใช้ความคิดและความจำอย่างสม่ำเสมอ";
        }
        else
        {
            riskLevel = "ความเสี่ยงสูง";
            chartColor = highRiskColor;
            resultText.text = "ผลการประเมิน: คุณมีความเสี่ยงสูงที่จะเป็นโรคอัลไซเมอร์ แนะนำให้ปรึกษาแพทย์เพื่อตรวจประเมินเพิ่มเติม";
        }

        // Update chart color
        chartArea.color = new Color(chartColor.r, chartColor.g, chartColor.b, 0.5f);
    }

    // Helper method to calculate cognitive scores
    public float CalculateSpeedScore(float time)
    {
        float maxTime = 300f; // 5 minutes
        return Mathf.Max(0, (1 - (time / maxTime)) * 100f);
    }

    public float CalculateAccuracyScore(int correctCoins, int totalCoins)
    {
        if (totalCoins == 0) return 0;
        return (correctCoins / (float)totalCoins) * 100f;
    }

    public float CalculateMemoryScore(int coin10Count, int coin5Count, int coin1Count)
    {
        int totalCoins = coin10Count + coin5Count + coin1Count;
        if (totalCoins == 0) return 0;

        // ให้น้ำหนักกับการจัดเรียงเหรียญแต่ละประเภท
        float weightedScore = (coin10Count * 1.0f + coin5Count * 0.7f + coin1Count * 0.4f) / totalCoins;
        return weightedScore * 100f;
    }
} 