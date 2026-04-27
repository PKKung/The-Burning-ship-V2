using UnityEngine;

public class SanityVoiceManager : MonoBehaviour
{
    public static SanityVoiceManager instance;

    [Header("Audio Settings")]
    public AudioSource voice75; // เสียงเตือนเมื่อสติเหลือ 75%
    public AudioSource voice50; // เสียงเตือนเมื่อสติเหลือ 50%
    public AudioSource voice25; // เสียงเตือนเมื่อสติเหลือ 25%

    // ตัวแปรจำสถานะว่าเล่นไปหรือยัง
    private bool played75 = false;
    private bool played50 = false;
    private bool played25 = false;

    void Awake()
    {
        // ทำให้เป็นอมตะข้ามฉากและป้องกันตัวซ้ำ
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // ดึงค่าสติปัจจุบันมาจากสคริปต์ SanitySystem (เรียกผ่าน Static หรือ Instance)
        float currentSanity = SanitySystem.currentSanity;

        // เช็คระดับสติและเล่นเสียง
        CheckSanityThreshold(currentSanity);
    }

    void CheckSanityThreshold(float sanity)
    {
        // กรณีเหลือ 75%
        if (sanity <= 75f && !played75)
        {
            PlayVoice(voice75);
            played75 = true;
        }
        // กรณีเหลือ 50%
        else if (sanity <= 50f && !played50)
        {
            PlayVoice(voice50);
            played50 = true;
        }
        // กรณีเหลือ 25%
        else if (sanity <= 25f && !played25)
        {
            PlayVoice(voice25);
            played25 = true;
        }

        // ทริค: ถ้าเก็บไอเทมเพิ่มสติจนเกินเกณฑ์ อาจจะรีเซ็ตให้ดังใหม่ได้ (ถ้าต้องการ)
        if (sanity > 80f) played75 = false;
        if (sanity > 55f) played50 = false;
        if (sanity > 30f) played25 = false;
    }

    void PlayVoice(AudioSource audio)
    {
        if (audio != null && !audio.isPlaying)
        {
            audio.Play();
            Debug.Log("เล่นเสียงเตือนค่าสติ!");
        }
    }

    // ฟังก์ชันสำหรับเรียกใช้ตอนเริ่มเกมใหม่ (StartNewGame)
    public void ResetSanityVoices()
    {
        played75 = false;
        played50 = false;
        played25 = false;
        Debug.Log("รีเซ็ตสถานะเสียงสติเรียบร้อย");
    }
}