using UnityEngine;

public class PickupVoiceManager : MonoBehaviour
{
    public static PickupVoiceManager instance;
    [Header("Audio Settings")]
    public AudioSource voiceRound2; // เสียงเมื่อเก็บชิ้นแรกของรอบ 2
    public AudioSource voiceRound3; // เสียงเมื่อเก็บชิ้นแรกของรอบ 3

    private bool playedR2 = false;
    private bool playedR3 = false;
    private int lastCheckedCount = 0; // เอาไว้จำว่าก่อนหน้านี้เก็บได้กี่ชิ้น

    void Awake()
    {
        // สั่งให้วัตถุนี้ไม่ถูกลบเวลาเปลี่ยน Scene
        DontDestroyOnLoad(this.gameObject);

        // ป้องกันการเกิดวัตถุซ้ำ (Singleton Pattern)
        // ถ้ากลับมาฉากเดิม แล้วมีตัวเดิมอยู่แล้ว ให้ลบตัวที่เพิ่งเกิดทิ้ง
        if (FindObjectsOfType<PickupVoiceManager>().Length > 1)
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        // ดึงข้อมูลมาจากสคริปต์เพื่อน (แค่ดึงมาดู ไม่ได้ไปแก้ไขของเขา)
        int currentRound = SignalQuestManager.instance.currentRound;
        int currentParts = SignalQuestManager.instance.partsCollected;

        // เช็คว่า "มีการเก็บชิ้นใหม่เกิดขึ้น" และ "เป็นชิ้นแรกของรอบ" หรือไม่
        if (currentParts == 1 && lastCheckedCount == 0)
        {
            if (currentRound == 2 && !playedR2)
            {
                PlayVoice(voiceRound2);
                playedR2 = true;
            }
            else if (currentRound == 3 && !playedR3)
            {
                PlayVoice(voiceRound3);
                playedR3 = true;
            }
        }

        // อัปเดตค่าที่จำไว้
        lastCheckedCount = currentParts;

        // ถ้ารอบเปลี่ยน (เช่นจาก 2 ไป 3) ให้รีเซ็ตตัวนับการเช็ค
        if (currentParts == 0) lastCheckedCount = 0;
    }

    void PlayVoice(AudioSource audio)
    {
        if (audio != null)
        {
            audio.Play();
            Debug.Log("เล่นเสียงเก็บอะไหล่ชิ้นแรกแล้ว!");
        }
    }
    public void ResetVoiceStatus()
    {
        playedR2 = false;
        playedR3 = false;
        lastCheckedCount = 0;
    }
}