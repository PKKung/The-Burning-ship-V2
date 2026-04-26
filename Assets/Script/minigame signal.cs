using UnityEngine;
using UnityEngine.UI;

public class SignalMiniGame : MonoBehaviour
{
    [Header("UI References")]
    public GameObject uiPanel;          // ลาก Panel มินิเกมมาใส่
    public Slider signalBar;            // ลาก Slider ของมินิเกมมาใส่
    public RectTransform buttonTransform; // ลากตัวปุ่มวงกลมมาใส่

    [Header("Mini-Game Settings")]
    public float currentSignal = 0f;
    public float dropSpeed = 3f;        // สัญญาณลดลงวินาทีละเท่าไหร่ (ตอนปิดหน้าจอ)
    public float addSpeed = 8f;         // กดหนึ่งครั้งเพิ่มเท่าไหร่
    public bool isGameActive = false;
    private bool isCompleted = false;   // เช็กว่าส่งสัญญาณสำเร็จถาวรหรือยัง

    [Header("Button Move Boundary")]
    public float minX = -350f;
    public float maxX = 350f;
    public float minY = -200f;
    public float maxY = 200f;

    void Start()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
    }

    void Update()
    {
        // ถ้าภารกิจเสร็จแล้ว ไม่ต้องลดสัญญาณอีก
        if (isCompleted) return;

        // --- ระบบสัญญาณลดลงอัตโนมัติ (ทำงานตลอดเวลาแม้ปิดหน้าจอ) ---
        if (currentSignal > 0)
        {
            currentSignal -= dropSpeed * Time.deltaTime;
        }
        currentSignal = Mathf.Clamp(currentSignal, 0, 100);

        // อัปเดตแถบ Slider เฉพาะตอนที่เปิดหน้าจออยู่
        if (uiPanel.activeSelf && signalBar != null)
        {
            signalBar.value = currentSignal;
        }

        // เช็กเงื่อนไขการชนะ
        if (currentSignal >= 100)
        {
            Win();
        }
    }

    // ฟังก์ชันสำหรับกดปุ่มวงกลม
    public void OnButtonClick()
    {
        if (!isGameActive) return;

        currentSignal += addSpeed;

        if (buttonTransform != null)
        {
            MoveButtonRandomly();
            StopAllCoroutines();
            StartCoroutine(ButtonPunchEffect());
        }
    }

    // ฟังก์ชันเปิดมินิเกม (เรียกจากจุดคลิกที่เครื่องส่งสัญญาณ)
    public void StartMiniGame()
    {
        if (isCompleted) return;

        isGameActive = true;
        uiPanel.SetActive(true);
        MoveButtonRandomly();

        // --- เรียกใช้ HeatSystem แบบ Static (เร่งความร้อนเป็น 3.0) ---
        HeatSystem.SetHeatRate(3.0f);
    }

    // ฟังก์ชันสำหรับปุ่ม X (Exit)
    public void CloseMiniGame()
    {
        isGameActive = false;
        uiPanel.SetActive(false);

        // --- เรียกใช้ HeatSystem แบบ Static (รีเซ็ตความร้อนกลับค่าปกติ) ---
        HeatSystem.ResetHeatRate();
    }

    void Win()
    {
        isGameActive = false;
        isCompleted = true;
        uiPanel.SetActive(false);

        // --- เรียกใช้ HeatSystem แบบ Static (รีเซ็ตความร้อนกลับค่าปกติ) ---
        HeatSystem.ResetHeatRate();

        Debug.Log("ส่งสัญญาณสำเร็จถาวร!");
    }

    void MoveButtonRandomly()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        buttonTransform.anchoredPosition = new Vector2(randomX, randomY);
    }

    System.Collections.IEnumerator ButtonPunchEffect()
    {
        buttonTransform.localScale = Vector3.one * 1.2f;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 10f;
            buttonTransform.localScale = Vector3.Lerp(buttonTransform.localScale, Vector3.one, t);
            yield return null;
        }
        buttonTransform.localScale = Vector3.one;
    }
}