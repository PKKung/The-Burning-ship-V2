using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SanitySystem : MonoBehaviour
{
    [Header("Sanity Settings")]
    public static float currentSanity = 100f;
    public float maxSanity = 100f;
    public float sanityDropRate = 2f;    // ลดลงกี่หน่วยต่อวินาที
    public float heatThreshold = 50f;    // จุดที่เริ่มสติหลุด (ความร้อนเกิน 50)

    [Header("UI Reference")]
    public Slider sanitySlider;          // ลาก Slider ของค่าสติมาใส่

    void Start()
    {
        currentSanity = maxSanity;
        if (sanitySlider != null)
        {
            sanitySlider.maxValue = maxSanity;
            sanitySlider.value = currentSanity;
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "lose") return;
        // 1. เช็กค่าความร้อนจาก HeatSystem (ที่เป็น Static)
        if (HeatSystem.currentHeat > heatThreshold)
        {
            DecreaseSanity();
        }

        UpdateUI();
    }

    void DecreaseSanity()
    {
        if (currentSanity > 0)
        {
            currentSanity -= sanityDropRate * Time.deltaTime;
        }
        else
        {
            currentSanity = 0;
            OnSanityZero();
        }
    }

    void UpdateUI()
    {
        if (sanitySlider != null)
        {
            sanitySlider.value = currentSanity;
        }
    }

    void OnSanityZero()
    {
        Debug.Log("สติหลุดหมดแล้ว! (Game Over หรือ หน้าจอมัวลงตรงนี้)");
        SceneManager.LoadScene("lose");
        // คุณสามารถใส่โค้ด Effect หน้าจอมัว หรือตัดเข้าฉากจบตรงนี้ได้
    }

    // ฟังก์ชันสำหรับเพิ่มสติ (เผื่อคุณอยากทำไอเทมยาดมหรือที่พักใจในอนาคต)
    public void RestoreSanity(float amount)
    {
        currentSanity += amount;
        currentSanity = Mathf.Min(currentSanity, maxSanity);
    }
}