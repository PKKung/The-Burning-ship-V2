using UnityEngine;

public class CoolDownVoiceManager : MonoBehaviour
{
    public static CoolDownVoiceManager instance;

    [Header("Audio Settings")]
    public AudioSource startCoolDownAudio;

    [Header("References")]
    public GameObject coolDownUIPanel;

    // --- ส่วนที่เพิ่มเพื่อจำสถานะ ---
    // ใช้ static เพื่อให้จำได้ข้ามฉากและไม่หายไปจนกว่าจะจบเกม
    public static bool hasPlayedFirstCoolDownSound = false;

    private bool isUIPreviouslyActive = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    void Update()
    {
        if (coolDownUIPanel == null) return;

        bool isUIActive = coolDownUIPanel.activeSelf;

        // เช็คเงื่อนไข: 
        // 1. UI เปิดขึ้นมา (isUIActive)
        // 2. ก่อนหน้านี้ปิดอยู่ (!isUIPreviouslyActive)
        // 3. และยังไม่เคยเล่นเสียงนี้เลย (!hasPlayedFirstCoolDownSound)
        if (isUIActive && !isUIPreviouslyActive && !hasPlayedFirstCoolDownSound)
        {
            PlaySound();
            hasPlayedFirstCoolDownSound = true; // ล็อคกุญแจทันทีหลังจากเล่นครั้งแรก
        }

        isUIPreviouslyActive = isUIActive;
    }

    void PlaySound()
    {
        if (startCoolDownAudio != null)
        {
            startCoolDownAudio.Play();
            Debug.Log("เล่นเสียงเริ่มลดอุณหภูมิ (ครั้งแรกและครั้งเดียว) เรียบร้อย!");
        }
    }
}