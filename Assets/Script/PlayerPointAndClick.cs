using UnityEngine;

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
        
        // บังคับตั้งค่าฟิสิกส์กันพลาด
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 1. รับค่าการคลิกเมาส์ซ้าย
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos = new Vector2(mousePos.x, mousePos.y);
            
            if (Vector2.Distance(transform.position, targetPos) > stopDistance)
            {
                isMoving = true;
            }
        }

        // 2. คุยกับ Animator (ส่งค่าให้ Blend Tree เล่นท่าซ้าย-ขวา)
        if (anim != null)
        {
            anim.SetBool("isWalking", isMoving);

            if (isMoving)
            {
                // ถ้าเดินไปทางขวา ค่าจะเป็นบวก / ทางซ้าย ค่าจะเป็นลบ
                float directionX = targetPos.x - transform.position.x;
                anim.SetFloat("FaceX", directionX);
            }
        }
    }

    void FixedUpdate()
    {
        // 3. ระบบฟิสิกส์การเดินแบบ "ไหลลื่น (Velocity)"
        if (isMoving)
        {
            float distance = Vector2.Distance(rb.position, targetPos);

            if (distance > stopDistance)
            {
                Vector2 direction = (targetPos - rb.position).normalized;
                rb.linearVelocity = direction * speed; // ออกแรงผลัก ทำให้เบียดกำแพงได้เนียนๆ
            }
            else
            {
                StopMovement();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // หยุดนิ่งสนิทเมื่อไม่เดิน
        }
    }

    // 4. ฟังก์ชันสั่งเบรก
    private void StopMovement()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
    }

    // 5. ระบบกันกระเด็น (ชนปุ๊บ หยุดปั๊บ)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ถ้าสิ่งที่ชนไม่ใช่จุดวาร์ปหรือไอเทมที่เดินผ่านได้ ให้หยุดเดินทันที
        if (!collision.collider.isTrigger)
        {
            StopMovement();
            targetPos = transform.position; // ล้างเป้าหมายทิ้ง เพื่อไม่ให้มันพยายามดันกำแพงต่อ
        }
    }
}