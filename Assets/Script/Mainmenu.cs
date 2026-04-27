using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // --- 1. รีเซ็ตระบบความร้อน (Heat System) ---
        HeatSystem.currentHeat = 0f;
        HeatSystem.heatIncreasePerSecond = 0.5f;
        if (HeatSystem.instance != null)
        {
            HeatSystem.instance.currentDefaultRate = 0.5f;
            // ถ้าคุณมีตัวแปรเช็คเสียง 75% ใน HeatSystem อย่าลืมรีเซ็ตด้วย (ถ้ามี)
            // HeatSystem.instance.hasPlayed75Warning = false;
        }

        // --- 2. รีเซ็ตรอบและเควส (Signal Quest) ---
        if (SignalQuestManager.instance != null)
        {
            SignalQuestManager.instance.ResetQuestForNewGame();
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

        // --- 5. รีเซ็ตเสียงเข้าฉากครั้งแรก (Static Variables) ---

        FirstTimeAudioScene5.hasPlayedInScene5 = false;

        // --- 6. โหลดฉากเริ่มเกม (ตรวจสอบชื่อให้ตรงกับ Build Settings) ---
        SceneManager.LoadScene("Intro");
    }
    public void QuitGame()
    {
        Debug.Log("ออกจากเกม!");
        Application.Quit();
    }
}


