using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // สำคัญมากสำหรับการเช็กชื่อฉาก

[RequireComponent(typeof(Rigidbody2D))]
public class PointClickMaster : MonoBehaviour
{
    [Header("ตั้งค่าการเดิน")]
    public float normalSpeed = 5f;    // ความเร็วปกติ
    public float slowSpeed = 2f;      // ความเร็วฉาก 4 และ 5 (Level_4, Level_5)
    public float stopDistance = 0.1f;

    private float currentSpeed;       // ความเร็วที่ใช้จริงในฉากนั้นๆ
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 targetPos;
    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        targetPos = transform.position;

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        // --- ระบบเช็กชื่อฉากเพื่อปรับความเร็วตอนเริ่มเกม ---
        UpdateSpeedBasedOnScene();
    }

    void Update()
    {
        // 0. ตรวจสอบชื่อฉาก: ถ้าอยู่ในหน้า Lose หรือ Win ให้หยุดทำงานทันที!
        // เพื่อป้องกันไม่ให้สคริปต์นี้ไปแย่งการคลิกเมาส์จากปุ่ม UI
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "lose" || sceneName == "End win")
        {
            StopMovement();
            return;
        }

        // 1. ตรวจสอบการคลิกเมาส์
        if (Input.GetMouseButtonDown(0))
        {
            // เช็กว่าคลิกโดน UI หรือไม่ (ป้องกันแฮมสเตอร์เดินเวลาจะกดปุ่มเมนู)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // เช็กว่ามินิเกมเปิดอยู่หรือไม่ (ถ้าซ่อมเครื่องอยู่ ไม่ให้เดิน)
            GameObject miniGame = GameObject.Find("CoolingMiniGame");
            if (miniGame != null && miniGame.activeInHierarchy)
            {
                return;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos = new Vector2(mousePos.x, mousePos.y);

            if (Vector2.Distance(transform.position, targetPos) > stopDistance)
            {
                isMoving = true;
            }
        }

        // 2. ระบบ Animator (หันหน้าและท่าเดิน)
        if (anim != null)
        {
            anim.SetBool("isWalking", isMoving);

            if (isMoving)
            {
                float directionX = targetPos.x - transform.position.x;
                anim.SetFloat("FaceX", directionX);
            }
        }
    }

    void FixedUpdate()
    {
        // 3. ระบบฟิสิกส์ (Rigidbody2D)
        if (isMoving)
        {
            float distance = Vector2.Distance(rb.position, targetPos);

            if (distance > stopDistance)
            {
                Vector2 direction = (targetPos - rb.position).normalized;
                // ใช้ currentSpeed ที่ถูกคำนวณมาแล้วจาก UpdateSpeedBasedOnScene()
                rb.linearVelocity = direction * currentSpeed;
            }
            else
            {
                StopMovement();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // ฟังก์ชันสำหรับปรับความเร็วตามเลเวล
    private void UpdateSpeedBasedOnScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // เช็กชื่อฉากให้ตรงกับใน Build Settings
        if (sceneName == "Level_4" || sceneName == "Level_5")
        {
            currentSpeed = slowSpeed;
            Debug.Log("<color=cyan><b>[Speed Log]:</b></color> ฉากนี้อันตราย! ปรับความเร็วเป็นสายสโลว์: " + currentSpeed);
        }
        else
        {
            currentSpeed = normalSpeed;
            Debug.Log("<color=white><b>[Speed Log]:</b></color> ฉากปกติ ความเร็วเต็มสปีด: " + currentSpeed);
        }
    }

    public void StopMovement()
    {
        isMoving = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ถ้าชนกำแพงให้หยุดเดินทันที ไม่ให้แฮมสเตอร์พยายามเดินทะลุกำแพง
        if (!collision.collider.isTrigger)
        {
            StopMovement();
            targetPos = transform.position;
        }
    }
}