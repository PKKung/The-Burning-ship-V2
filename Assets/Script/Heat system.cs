using UnityEngine;
using UnityEngine.UI;

public class HeatSystem : MonoBehaviour
{
    [Header("Heat Settings")]
    // เปลี่ยนเป็น static เพื่อให้เข้าถึงได้จากทุกสคริปต์ในทุกฉาก
    public static float currentHeat = 0f;
    public static float heatIncreasePerSecond = 0.5f;
    public float maxHeat = 100f;

    [Header("UI Reference")]
    public Slider heatSlider;

    private static float defaultHeatRate;

    void Start()
    {
        // จำค่าเริ่มต้นไว้ตอนเริ่มเกม
        defaultHeatRate = heatIncreasePerSecond;

        if (heatSlider != null)
        {
            heatSlider.maxValue = maxHeat;
            heatSlider.value = currentHeat;
        }
    }

    void Update()
    {
        IncreaseHeat();
        UpdateUI();
    }

    void IncreaseHeat()
    {
        if (currentHeat < maxHeat)
        {
            // ใช้ตัวแปร static ในการคำนวณ
            currentHeat += heatIncreasePerSecond * Time.deltaTime;
        }
        else
        {
            currentHeat = maxHeat;
            OnHeatFull();
        }
    }

    void UpdateUI()
    {
        if (heatSlider != null)
        {
            heatSlider.value = currentHeat;
        }
    }

    // ฟังก์ชัน static สำหรับเปลี่ยนความเร็ว (สั่งงานได้โดยไม่ต้องมีตัวแปรอ้างอิง)
    public static void SetHeatRate(float newRate)
    {
        heatIncreasePerSecond = newRate;
    }

    // ฟังก์ชัน static สำหรับรีเซ็ตค่า
    public static void ResetHeatRate()
    {
        heatIncreasePerSecond = defaultHeatRate;
    }

    void OnHeatFull()
    {
        Debug.Log("ความร้อนเต็มแล้ว!");
    }
}