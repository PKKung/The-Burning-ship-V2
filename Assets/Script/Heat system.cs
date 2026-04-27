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
    [Header("Alert Settings")]
    public AudioSource warning75Audio; // ลาก Audio Source เสียงเตือนมาใส่
    private bool hasPlayed75Warning = false; // กันไม่ให้เสียงดังซ้ำซาก

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
        CheckGameOver();
        CheckHeatWarning();
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
    public void ResetSystemForNewGame()
    {
        currentHeat = 0f;
        heatIncreasePerSecond = 0.5f; // กลับไปค่าเริ่มต้นสุดๆ
        currentDefaultRate = 0.5f;
        // ถ้ามี UI Slider ให้รีเซ็ตด้วย
        if (heatSlider != null) heatSlider.value = 0f;
        Debug.Log("Reset Heat System Done!");
    }
    void CheckGameOver()
    {
        // เงื่อนไขที่ 1: ความร้อนเต็ม 100
        if (currentHeat >= maxHeat)
        {
            GameOver();
        }

        // เงื่อนไขที่ 2: ค่าสติเหลือ 0 (สมมติว่าคุณมีตัวแปร currentSanity)
        // ถ้าคุณแยกสคริปต์สติไว้ที่อื่น ให้เรียกผ่าน Instance เช่น SanitySystem.instance.currentSanity
        if (SanitySystem.currentSanity <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        // โหลด Scene จบเกมที่คุณเตรียมไว้
        SceneManager.LoadScene("lose");
    }
    void CheckHeatWarning()
    {
        // เช็กว่าความร้อนถึง 75 หรือยัง (สมมติ maxHeat คือ 100)
        if (currentHeat >= 75f && !hasPlayed75Warning)
        {
            if (warning75Audio != null)
            {
                warning75Audio.Play();
                hasPlayed75Warning = true; // ล็อคไว้ให้ดังครั้งเดียวในรอบนั้น
                Debug.Log("ความร้อนสูงเกิน 75%! เล่นเสียงเตือน");
            }
        }

        // ทริค: ถ้าความร้อนลดลงต่ำกว่า 60% อาจจะรีเซ็ตให้เสียงเตือนใหม่ได้ในอนาคต (ถ้าต้องการ)
        if (currentHeat < 60f)
        {
            hasPlayed75Warning = false;
        }
    }
}