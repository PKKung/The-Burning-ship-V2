using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // บังคับหยุดการทำงานที่ค้างอยู่ทั้งหมดในหน้าเมนู (ถ้ามี)
        StopAllCoroutines();

        // --- 1. รีเซ็ตรอบและเควส (Signal Quest) ---
        // ต้องทำอันนี้ก่อนเพื่อหยุด Coroutine การวาร์ปฉากจบที่อาจค้างมาจากเกมรอบที่แล้ว
        if (SignalQuestManager.instance != null)
        {
            SignalQuestManager.instance.ResetQuestForNewGame();
        }

        // --- 2. รีเซ็ตระบบความร้อน (Heat System) ---
        HeatSystem.currentHeat = 0f;
        if (HeatSystem.instance != null)
        {
            HeatSystem.instance.currentDefaultRate = 0.5f;
            // รีเซ็ตความเร็วเพิ่มความร้อนกลับไปที่ค่าเริ่มต้น
            HeatSystem.heatIncreasePerSecond = 0.5f;

            // แถม: รีเซ็ตสถานะเสียงเตือน 75% ถ้าคุณมีตัวแปรนี้
            // HeatSystem.instance.hasPlayed75Warning = false;
        }

        // --- 3. รีเซ็ตค่าสติและเสียงเตือนสติ (Sanity System) ---
        SanitySystem.currentSanity = 100f;
        if (SanityVoiceManager.instance != null)
        {
            SanityVoiceManager.instance.ResetSanityVoices();
        }

        // --- 4. รีเซ็ตเสียงประกอบอื่นๆ (Voice Managers) ---
        if (PickupVoiceManager.instance != null)
        {
            PickupVoiceManager.instance.ResetVoiceStatus();
        }

        if (SignalStartVoiceManager.instance != null)
        {
            SignalStartVoiceManager.instance.ResetSignalVoices();
        }

        // --- 5. รีเซ็ตตัวแปร Static สำหรับเสียงเข้าฉากครั้งแรก ---
        // (เพิ่มฉากอื่นๆ ให้ครบถ้ามี)
        CoolDownVoiceManager.hasPlayedFirstCoolDownSound = false;
        FirstTimeAudioScene5.hasPlayedInScene5 = false;

        // --- 6. โหลดฉากเริ่มเกม ---
        Debug.Log("Starting New Game... Resetting All Systems.");
        SceneManager.LoadScene("Intro");
    }

    public void QuitGame()
    {
        Debug.Log("ออกจากเกม!");
        Application.Quit();
    }
}