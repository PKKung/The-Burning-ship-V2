using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CoolingMiniGame : MonoBehaviour
{
    // เพิ่มตัวแปรนี้เพื่อให้สคริปต์อื่น (coldminigame) เช็กสถานะได้
    [HideInInspector] // ซ่อนไว้ไม่ให้รก Inspector เพราะเราคุมผ่าน Code
    public bool isGameActive = false;

    [Header("UI References")]
    public RectTransform needle;        // ลาก Needle มาใส่
    public RectTransform barBackground; // ลาก BarBackground มาใส่
    public RectTransform blueZone;      // ลาก BlueZone มาใส่

    [Header("Settings")]
    public float needleSpeed = 300f;    // ความเร็วเข็ม
    public float goodCooling = 15f;     // กดโดนสีฟ้า ลดความร้อนเท่าไหร่
    public float badHeating = 5f;       // กดโดนสีแดง เพิ่มความร้อนเท่าไหร่

    private float barHalfWidth;
    private int moveDirection = 1;      // 1 = ขวา, -1 = ซ้าย
    private float initialSpeed;         // เก็บค่าความเร็วเริ่มต้นไว้รีเซ็ต
    private bool canClick = false;
    void Start()
    {
        initialSpeed = needleSpeed;

        // คำนวณขอบเขตการเคลื่อนที่
        if (barBackground != null && needle != null)
        {
            barHalfWidth = (barBackground.rect.width / 2f) - (needle.rect.width / 2f);
        }

        // เริ่มต้นปิดหน้าจอไว้ก่อน
        gameObject.SetActive(false);
        isGameActive = false;
    }

    void Update()
    {
        if (!isGameActive) return;

        MoveNeedle();

        // เช็กการกดปุ่ม (Spacebar หรือ คลิกเมาส์ซ้าย)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {

            if (Input.GetMouseButtonDown(0))
            {
                // เช็กว่าเมาส์จิ้มโดน UI หรือไม่
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    // ตรงนี้สำคัญ: เราต้องเช็กว่า "สิ่งที่เมาส์จิ้มอยู่ ชื่อว่าอะไร"
                    // ถ้าชื่อไม่ใช่ปุ่ม Exit หรือปุ่มเมนู ให้ CheckResult ได้
                    GameObject clickedObj = EventSystem.current.currentSelectedGameObject;

                    // ถ้าคลิกโดนปุ่ม Exit (สมมติปุ่มคุณชื่อ ExitButton) ให้หยุดทำงาน
                    if (clickedObj != null && clickedObj.name == "ExitButton")
                    {
                        return;
                    }
                }

                // ถ้าไม่ได้กดปุ่ม Exit ให้เช็กผลการเล่นได้
                CheckResult();
            }
        }
    }

    // ฟังก์ชันเปิดมินิเกม
    public void StartGame()
    {
        isGameActive = true; // ตั้งค่าสถานะเป็นกำลังเล่น
        gameObject.SetActive(true);
        needleSpeed = initialSpeed;

        RandomizeBlueZone();

        // สุ่มตำแหน่งเข็มเริ่มต้น
        float randomX = Random.Range(-barHalfWidth, barHalfWidth);
        needle.anchoredPosition = new Vector2(randomX, 0);

        moveDirection = (Random.value > 0.5f) ? 1 : -1;
        canClick = false;
        StartCoroutine(EnableClickDelay());
    }
    System.Collections.IEnumerator EnableClickDelay()
    {
        // รอสักนิดเพื่อให้การคลิกเปิดหน้าจอผ่านพ้นไปก่อน
        yield return new WaitForSeconds(0.1f);
        canClick = true;
    }

    void MoveNeedle()
    {
        float currentX = needle.anchoredPosition.x;
        currentX += needleSpeed * moveDirection * Time.deltaTime;

        if (currentX >= barHalfWidth)
        {
            currentX = barHalfWidth;
            moveDirection = -1;
        }
        else if (currentX <= -barHalfWidth)
        {
            currentX = -barHalfWidth;
            moveDirection = 1;
        }

        needle.anchoredPosition = new Vector2(currentX, 0);

    }

    void RandomizeBlueZone()
    {
        if (barBackground == null || blueZone == null) return;

        float limitX = (barBackground.rect.width / 2f) - (blueZone.rect.width / 2f);
        float randomX = Random.Range(-limitX, limitX);

        blueZone.anchoredPosition = new Vector2(randomX, 0);
    }

    void CheckResult()
    {
        float needleX = needle.anchoredPosition.x;
        float blueZoneX = blueZone.anchoredPosition.x;
        float blueZoneHalfWidth = blueZone.rect.width / 2f;

        float blueLeftEdge = blueZoneX - blueZoneHalfWidth;
        float blueRightEdge = blueZoneX + blueZoneHalfWidth;

        if (needleX >= blueLeftEdge && needleX <= blueRightEdge)
        {
            // SUCCESS: เรียกใช้ HeatSystem แบบ Static
            HeatSystem.currentHeat -= goodCooling;

            // ป้องกันไม่ให้ค่าติดลบ (ให้น้อยสุดคือ 0)
            HeatSystem.currentHeat = Mathf.Max(HeatSystem.currentHeat, 0);

            Debug.Log("<color=blue>ลดความร้อน!</color>");

            // เมื่อสำเร็จให้สุ่มตำแหน่งใหม่
            RandomizeBlueZone();

            // ถ้าอยากให้กดโดนแล้วปิดเกมทันที ให้ลบบรรทัดบนแล้วใช้ CloseGame(); แทน
        }
        else
        {
            // FAIL: กดโดนสีแดง ความร้อนเพิ่ม
            HeatSystem.currentHeat += badHeating;
            Debug.Log("<color=red>พลาด!</color>");

            // ลงโทษโดยการเพิ่มความเร็วเข็ม
            needleSpeed *= 1.2f;
        }
    }

    public void CloseGame()
    {
        isGameActive = false; // ตั้งค่าสถานะเป็นปิดการใช้งาน
        canClick = false; // รีเซ็ตค่า
        gameObject.SetActive(false);
    }
}