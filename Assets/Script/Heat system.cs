using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class HeatSystem : MonoBehaviour
{
    public static HeatSystem instance; // เพิ่ม instance เพื่อให้เรียกใช้ง่ายขึ้น

    [Header("Heat Settings")]
    public static float currentHeat = 0f;
    public static float heatIncreasePerSecond = 0.5f;
    public float maxHeat = 100f;

    [Header("UI Reference")]
    public Slider heatSlider;

    // ตัวแปรสำหรับจำค่าความร้อนพื้นฐานของแต่ละรอบ (Round)
    public float currentDefaultRate = 0.5f;

    private Volume globalVolume;
    private ColorAdjustments colorAdjustments;
    private WhiteBalance whiteBalance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject foundVolume = GameObject.Find("GlobalVolume");
        if (foundVolume != null)
        {
            globalVolume = foundVolume.GetComponent<Volume>();
            globalVolume.profile.TryGet(out colorAdjustments);
            globalVolume.profile.TryGet(out whiteBalance);
        }

        GameObject sliderObj = GameObject.Find("HeatSlider");
        if (sliderObj != null) heatSlider = sliderObj.GetComponent<Slider>();
    }

    void Start()
    {
        // เริ่มเกมมาให้ Default Rate เป็นค่าที่ตั้งไว้ใน Inspector (0.5)
        currentDefaultRate = heatIncreasePerSecond;

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
        UpdateVisuals();
    }

    void IncreaseHeat()
    {
        if (currentHeat < maxHeat)
        {
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

    void UpdateVisuals()
    {
        if (colorAdjustments != null && whiteBalance != null)
        {
            float heatPercent = currentHeat / maxHeat;
            whiteBalance.temperature.value = heatPercent * 80f;
            colorAdjustments.saturation.value = heatPercent * 40f;
        }
    }

    // ฟังก์ชันใหม่: สำหรับเปลี่ยนความยากพื้นฐานเมื่อจบรอบ
    public void SetNewRoundDifficulty(float newBaseRate)
    {
        currentDefaultRate = newBaseRate;
        heatIncreasePerSecond = newBaseRate;
        Debug.Log($"<color=orange>Round Difficulty Updated: {newBaseRate}</color>");
    }

    public static void SetHeatRate(float newRate)
    {
        heatIncreasePerSecond = newRate;
    }

    // แก้ไข: ให้รีเซ็ตกลับไปเป็นค่า Default ของรอบนั้นๆ (ไม่ใช่ 0.5 เสมอไป)
    public static void ResetHeatRate()
    {
        heatIncreasePerSecond = instance.currentDefaultRate;
    }

    void OnHeatFull()
    {
        if (Time.frameCount % 60 == 0) Debug.Log("ความร้อนเต็มแล้ว!");
    }
}