using UnityEngine;
using System.Collections;

public class BrokenEventTrigger : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource brokenSoundRound1; // เสียงเครื่องพังรอบ 1
    public AudioSource brokenSoundRound2; // เสียงหลังจากเครื่องพังรอบ 2 (ก่อนหาอะไหล่)

    [Header("Settings")]
    public MonoBehaviour playerMovement;
    public float lockTimeRound1 = 10f;
    public float lockTimeRound2 = 5f;   // รอบ 2 อาจจะล็อคสั้นลงหน่อยแค่ให้ฟังเสียงจบ

    private bool isRound1Triggered = false;
    private bool isRound2Triggered = false;

    public void TriggerBrokenSequence()
    {
        int currentRound = SignalQuestManager.instance.currentRound;

        // เหตุการณ์รอบที่ 1 (ของเดิม)
        if (currentRound == 1 && !isRound1Triggered)
        {
            StartCoroutine(SequenceRoutine(brokenSoundRound1, lockTimeRound1, 1));
        }
        // เหตุการณ์รอบที่ 2 (อันใหม่ที่คุณต้องการ!)
        else if (currentRound == 2 && !isRound2Triggered)
        {
            StartCoroutine(SequenceRoutine(brokenSoundRound2, lockTimeRound2, 2));
        }
    }

    IEnumerator SequenceRoutine(AudioSource targetAudio, float waitTime, int roundNum)
    {
        if (roundNum == 1) isRound1Triggered = true;
        else isRound2Triggered = true;

        // 1. ล็อคตัวละคร
        if (playerMovement != null) playerMovement.enabled = false;

        // 2. เล่นเสียง
        if (targetAudio != null) targetAudio.Play();

        Debug.Log($"เครื่องพังรอบที่ {roundNum}! เล่นเสียงและรอ {waitTime} วิ");

        // 3. รอตามเวลาที่ตั้งไว้
        yield return new WaitForSeconds(waitTime);

        // 4. ปลดล็อค
        if (playerMovement != null) playerMovement.enabled = true;

        Debug.Log("ไปหาอะไหล่ 3 ชิ้นได้!");
    }
}