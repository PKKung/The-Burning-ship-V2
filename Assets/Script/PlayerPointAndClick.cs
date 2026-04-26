using UnityEngine;
using UnityEngine.EventSystems; // สำหรับเช็ก UI

[RequireComponent(typeof(Rigidbody2D))]
public class PointClickMaster : MonoBehaviour
{
    [Header("ตั้งค่าการเดิน")]
    public float speed = 5f;
    public float stopDistance = 0.1f;

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
    }

    void Update()
    {
        // 1. รับค่าการคลิกเมาส์ซ้าย + เพิ่มเงื่อนไขเช็ก UI
        // !EventSystem.current.IsPointerOverGameObject() หมายถึง "ถ้าไม่ได้คลิกบน UI"
        if (Input.GetMouseButtonDown(0))
        {
            // ตรวจสอบว่ามี EventSystem ใน Scene และเราไม่ได้กดทับ UI อยู่
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                // ถ้าคลิกโดน UI (เช่น ปุ่มวงกลมในมินิเกม) ให้จบฟังก์ชัน Update ตรงนี้เลย ไม่ต้องเดิน
                return;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos = new Vector2(mousePos.x, mousePos.y);

            if (Vector2.Distance(transform.position, targetPos) > stopDistance)
            {
                isMoving = true;
            }
        }

        // 2. คุยกับ Animator
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
        // 3. ระบบฟิสิกส์การเดิน
        if (isMoving)
        {
            float distance = Vector2.Distance(rb.position, targetPos);

            if (distance > stopDistance)
            {
                Vector2 direction = (targetPos - rb.position).normalized;
                rb.linearVelocity = direction * speed;
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

    private void StopMovement()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.isTrigger)
        {
            StopMovement();
            targetPos = transform.position;
        }
    }
}