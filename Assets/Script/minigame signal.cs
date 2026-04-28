using UnityEngine;
using UnityEngine.UI;

public class SignalMiniGame : MonoBehaviour
{
    [Header("UI References")]
    public GameObject uiPanel;          // ลาก Panel มินิเกมมาใส่
    public Slider signalBar;            // ลาก Slider ของมินิเกมมาใส่
    public RectTransform buttonTransform; // ลากตัวปุ่มวงกลมมาใส่

    [Header("Mini-Game Settings")]
    // --- เปลี่ยนเป็น static เพื่อให้จำค่าได้ข้ามฉาก ---
    public static float currentSignal = 0f;
    public static bool isCompleted = false; // เช็กว่าส่งสัญญาณสำเร็จถาวรหรือยัง

    public float dropSpeed = 3f;
    public float addSpeed = 8f;
    public bool isGameActive = false;

    [Header("Button Move Boundary")]
    public float minX = -350f;
    public float maxX = 350f;
    public float minY = -200f;
    public float maxY = 200f;

    void Start()
    {
        if (uiPanel != null) uiPanel.SetActive(false);

        // เมื่อเริ่มฉากใหม่ ให้เข็ม Slider ไปอยู่ที่ค่าปัจจุบันที่จำไว้ใน static
        if (signalBar != null) signalBar.value = currentSignal;
    }

    void Update()
    {
        if (isCompleted) return;

        // --- ระบบความยากตามรอบที่คุณตั้งไว้ ---
        int round = SignalQuestManager.instance.currentRound;
        float currentDrop = 1f;

        // หมายเหตุ: ตามเลขของคุณ รอบสูงขึ้น (รอบ 3) จะลดช้าลง (ง่ายขึ้น) 
        // ถ้าต้องการให้ยากขึ้น ต้องสลับตัวเลขนะครับ
        if (round == 1) currentDrop = 0.5f;
        else if (round == 2) currentDrop = 0.25f;
        else if (round == 3) currentDrop = 0.125f;

        currentSignal -= currentDrop * Time.deltaTime;
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

        int round = SignalQuestManager.instance.currentRound;
        float currentAddPower = 1f;

        if (round == 1) currentAddPower = 3f;
        else if (round == 2) currentAddPower = 1.8f;
        else if (round == 3) currentAddPower = 1.65f;

        currentSignal += currentAddPower;

        if (buttonTransform != null)
        {
            MoveButtonRandomly();
            StopAllCoroutines();
            StartCoroutine(ButtonPunchEffect());
        }
    }

    // ฟังก์ชันเปิดมินิเกม
    public void StartMiniGame()
    {
        if (isCompleted) return;
        isGameActive = true;
        uiPanel.SetActive(true);

        int round = SignalQuestManager.instance.currentRound;
        float miniGameRate = 3f;

        if (round == 2) miniGameRate = 3f;
        else if (round == 3) miniGameRate = 3.0f;

        HeatSystem.SetHeatRate(miniGameRate);
    }

    // ฟังก์ชันปิดมินิเกม (ปุ่ม X)
    public void CloseMiniGame()
    {
        isGameActive = false;
        uiPanel.SetActive(false);
        HeatSystem.ResetHeatRate();
    }

    void Win()
    {
        isGameActive = false;
        isCompleted = true; // ล็อคสถานะว่าเสร็จแล้ว
        uiPanel.SetActive(false);

        HeatSystem.ResetHeatRate();

        Debug.Log("ส่งสัญญาณสำเร็จถาวร!");
        SignalQuestManager.instance.OnSignalSuccess();
    }

    // --- ส่วนฟังก์ชันเสริม (สุ่มปุ่มและเอฟเฟกต์) ---

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

    // ฟังก์ชันพิเศษ: ใช้เรียกเมื่อกด Replay หรือเริ่มเกมใหม่จริงๆ
    public static void ResetSignalData()
    {
        currentSignal = 0f;
        isCompleted = false;
    }
}