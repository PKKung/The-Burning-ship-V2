using UnityEngine;
using System.Collections;

public class LevelStartManager : MonoBehaviour
{
    [Header("Settings")]
    public AudioSource introAudio;
    public float freezeDuration = 6f;

    // --- ส่วนที่เพิ่มเข้ามาเพื่อจดจำสถานะ ---
    // ใช้ static เพื่อให้ตัวแปรนี้ "ไม่ถูกลบ" เมื่อเปลี่ยนฉากไปมา
    public static bool hasPlayedInThisLevel = false;

    private MonoBehaviour playerMovementScript;

    void Start()
    {
        // 1. เช็กก่อนเลยว่า "เคยเล่นไปหรือยัง?"
        if (hasPlayedInThisLevel)
        {
            Debug.Log("ฉากนี้เคยเล่นเสียงไปแล้ว ไม่ล็อคตัวซ้ำ");
            return; // จบการทำงานทันที ไม่ต้องรัน Coroutine ข้างล่าง
        }

        // 2. ถ้ายังไม่เคยเล่น ให้หาตัวละครแล้วเริ่มทำงาน
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovementScript = player.GetComponent<MonoBehaviour>();
            StartCoroutine(StartLevelRoutine());
        }
    }

    IEnumerator StartLevelRoutine()
    {
        // ล็อคกุญแจทันทีเพื่อป้องกันการรันซ้ำ
        hasPlayedInThisLevel = true;

        if (playerMovementScript != null) playerMovementScript.enabled = false;

        if (introAudio != null) introAudio.Play();

        yield return new WaitForSeconds(freezeDuration);

        if (playerMovementScript != null) playerMovementScript.enabled = true;
        Debug.Log("Intro จบแล้ว แฮมสเตอร์ขยับได้!");
    }
}