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
        if (isCompleted) return;

        // --- เพิ่ม/แก้ไข: รอบสูงขึ้น สัญญาณไหลลงเร็วขึ้น ---
        int round = SignalQuestManager.instance.currentRound;
        float currentDrop=1f;
        if (round == 1) currentDrop = 1f;  // รอบ 1 ลดวิละ 1
        else if (round == 2) currentDrop = 0.5f; // รอบ 2 ลดวิละ 2 (ยากขึ้น)
        else if (round == 3) currentDrop = 0.35f; // รอบ 3 ลดวิละ 4 (ยากสุด)

        currentSignal -= currentDrop * Time.deltaTime;
        // ------------------------------------------------

        currentSignal = Mathf.Clamp(currentSignal, 0, 100);

        if (uiPanel.activeSelf && signalBar != null)
        {
            signalBar.value = currentSignal;
        }

        if (currentSignal >= 100)
        {
            Win();
        }
    }

    // ฟังก์ชันสำหรับกดปุ่มวงกลม
    public void OnButtonClick()
    {
        if (!isGameActive) return;

        // --- ส่วนที่เพิ่ม/แก้ไข: ปรับความแรงการกดตามรอบ ---
        int round = SignalQuestManager.instance.currentRound;
        float currentAddPower=1f;
        if (round == 1) currentAddPower = 3f;   // รอบ 1 กดทีละ 3
        else if (round == 2) currentAddPower = 1.8f; // รอบ 2 กดทีละ 1.8 (ยากขึ้น)
        else if (round == 3) currentAddPower = 1.2f; // รอบ 3 กดทีละ 1.2 (ยากสุด)

        currentSignal += currentAddPower;

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

        // ปรับความเร็วตอนส่งสัญญาณตามรอบ
        int round = SignalQuestManager.instance.currentRound;
        float miniGameRate = 3.0f; // รอบ 1

        if (round == 2) miniGameRate = 4.0f;
        else if (round == 3) miniGameRate = 5.0f;

        HeatSystem.SetHeatRate(miniGameRate);
    }

    // ฟังก์ชันสำหรับปุ่ม X (Exit)
    public void CloseMiniGame()
    {
        isGameActive = false;
        uiPanel.SetActive(false);

        // ใช้ฟังก์ชันใหม่ที่เราสร้างไว้เพื่อกลับไปใช้ค่าฐานของรอบนั้น
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
        SignalQuestManager.instance.OnSignalSuccess();
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