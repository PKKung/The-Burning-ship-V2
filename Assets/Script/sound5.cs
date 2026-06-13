using UnityEngine;

public class FirstTimeAudioScene5 : MonoBehaviour
{
    public AudioSource voiceAudio; // ลาก Audio Source ของฉาก 5 มาใส่

    // ตัวแปร static จะจำค่าไว้ตลอดการเปิดเกมครั้งนั้นๆ
    public static bool hasPlayedInScene5 = false;

    void Start()
    {
        // เช็คว่ายังไม่เคยเล่นในฉาก 5 ใช่ไหม
        if (!hasPlayedInScene5)
        {
            if (voiceAudio != null)
            {
                voiceAudio.Play();
                hasPlayedInScene5 = true; // ล็อคไว้เลยว่าเล่นแล้ว
                Debug.Log("เล่นเสียงต้อนรับเข้าสู่ฉาก 5 ครั้งแรกเรียบร้อย!");
            }
        }
    }
}
