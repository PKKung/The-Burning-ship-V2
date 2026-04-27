using UnityEngine;
using System.Collections;

public class LevelStartManager : MonoBehaviour
{
    [Header("Settings")]
    public AudioSource introAudio;      // ลาก Audio Source ที่ใส่เสียง 6 วิมาใส่
    public float freezeDuration = 6f;   // เวลาที่ห้ามขยับ (6 วินาที)

    // ลากสคริปต์ตัวละครของคุณมาใส่ (เช่น PlayerController หรือ ThirdPersonController)
    // สมมติว่าชื่อ PlayerMovement นะครับ ให้แก้ตามชื่อสคริปต์เดินของคุณ
    private MonoBehaviour playerMovementScript;

    void Start()
    {
        // ค้นหาสคริปต์เดินของตัวละคร (แก้ชื่อ "PlayerMovement" เป็นชื่อสคริปต์เดินจริงๆ ของคุณ)
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<MonoBehaviour>();

        StartCoroutine(StartLevelRoutine());
    }

    IEnumerator StartLevelRoutine()
    {
        // 1. ปิดการเคลื่อนที่ (Disable Script เดิน)
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        Debug.Log("ตัวละครถูกล็อค: เริ่มเล่นเสียง");

        // 2. เล่นเสียง
        if (introAudio != null) introAudio.Play();

        // 3. รอจนครบเวลา (6 วินาที)
        yield return new WaitForSeconds(freezeDuration);

        // 4. เปิดการเคลื่อนที่ (Enable Script เดิน)
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        Debug.Log("ตัวละครขยับได้แล้ว!");
    }
}