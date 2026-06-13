using UnityEngine;

public class SparePart : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 3.5f; // กำหนดระยะการเก็บตรงนี้ (ปรับใน Inspector ได้)
    private Transform player;

    void Start()
    {
        // ค้นหาตัวละครที่มี Tag ว่า Player
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }
    }

    // เมื่อเอาเมาส์คลิกที่ตัวอะไหล่
    private void OnMouseUp()
    {
        if (player == null) return;

        // คำนวณระยะห่างระหว่าง "อะไหล่" กับ "แฮมสเตอร์"
        float distance = Vector2.Distance(transform.position, player.position);

        // เช็กว่าอยู่ในระยะที่กำหนดไหม
        if (distance <= interactRange)
        {
            CollectThisPart();
        }
        else
        {
            // ถ้าไม่อยากให้ Debug.Log บวม ให้ลบบรรทัดนี้ออกได้ครับ
            Debug.Log($"Too far! Distance: {distance:F2} / Need: {interactRange}");

            // เรียกใช้ระบบแจ้งเตือนให้ผู้เล่นรู้
            if (SignalQuestManager.instance != null)
            {
                SignalQuestManager.instance.ShowAlert("You are too far away!");
            }
        }
    }

    void CollectThisPart()
    {
        if (SignalQuestManager.instance != null)
        {
            SignalQuestManager.instance.CollectPart();
            Destroy(gameObject);
        }
    }
}