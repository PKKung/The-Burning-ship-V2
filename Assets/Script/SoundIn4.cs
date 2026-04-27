using UnityEngine;

public class FirstTimeAudio : MonoBehaviour
{
    public AudioSource voiceAudio; // ลาก Audio Source ที่ใส่เสียงมาใส่ตรงนี้

    // ตัวแปร static จะจำค่าไว้ตลอดตราบใดที่ยังไม่ปิดเกม
    private static bool hasPlayedInScene4 = false;

    void Start()
    {
        // ถ้ายังไม่เคยเล่นเสียงนี้เลย
        if (!hasPlayedInScene4)
        {
            if (voiceAudio != null)
            {
                voiceAudio.Play();
                hasPlayedInScene4 = true; // บันทึกว่าเล่นไปแล้วนะ
                Debug.Log("เล่นเสียงครั้งแรกในฉาก 4 เรียบร้อย!");
            }
        }
    }
}