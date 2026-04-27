using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // เพิ่มเพื่อเช็กชื่อฉาก

[RequireComponent(typeof(Rigidbody2D))]
public class PointClickMaster : MonoBehaviour
{
    [Header("ตั้งค่าการเดิน")]
    public float normalSpeed = 5f;    // ความเร็วปกติ
    public float slowSpeed = 2f;      // ความเร็วฉาก 4 และ 5
    public float stopDistance = 0.1f;

    private float currentSpeed;       // ความเร็วที่ใช้จริง
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
        // 1. ตรวจสอบว่าคลิกเพื่อเดินหรือไม่
        if (Input.GetMouseButtonDown(0))
        {
            // ตรวจสอบ EventSystem (ถ้าคลิกโดน UI ให้ return ทันที ไม่เดิน)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // แถม: ตรวจสอบมินิเกม (ถ้าหน้าจอมินิเกมเปิดอยู่ ไม่ให้เดิน)
            // ค้นหาวัตถุชื่อ CoolingMiniGame ถ้ามัน Active อยู่ให้หยุดทำงาน
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

        // 2. ควบคุม Animator
        if (anim != null)
        {
            anim.SetBool("isWalking", isMoving);

            if (isMoving)
            {
                float directionX = targetPos.x - transform.position.x;
                // ถ้าเดินไปทางขวา FaceX จะเป็นบวก (หันขวา) ถ้าซ้ายจะเป็นลบ (หันซ้าย)
                anim.SetFloat("FaceX", directionX);
            }
        }
    }

    void FixedUpdate()
    {
        // 3. ระบบฟิสิกส์การเดิน
        if (isMoving)
        {
            float distance = Vector2.Distance(rb.position, targetPos);

            if (distance > stopDistance)
            {
                Vector2 direction = (targetPos - rb.position).normalized;
                rb.linearVelocity = direction * currentSpeed; // ใช้ currentSpeed ที่ถูกปรับตามฉาก
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

    // ฟังก์ชันช่วยปรับความเร็วตามชื่อฉาก
    private void UpdateSpeedBasedOnScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // ถ้าชื่อฉากคือ Level_4 หรือ Level_5 (แก้ชื่อให้ตรงกับของคุณใน Build Settings)
        if (sceneName == "Level_4" || sceneName == "Level_5")
        {
            currentSpeed = slowSpeed;
            Debug.Log("<color=cyan>ความเร็วถูกปรับให้ช้าลงสำหรับฉาก: </color>" + sceneName);
        }
        else
        {
            currentSpeed = normalSpeed;
            Debug.Log("<color=white>ความเร็วปกติในฉาก: </color>" + sceneName);
        }
    }

    public void StopMovement()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ถ้าชนกำแพงหรือสิ่งกีดขวาง ให้หยุดเดินทันที
        if (!collision.collider.isTrigger)
        {
            StopMovement();
            targetPos = transform.position;
        }
    }
}