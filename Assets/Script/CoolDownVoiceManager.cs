using UnityEngine;

public class CoolDownVoiceManager : MonoBehaviour
{
    public static CoolDownVoiceManager instance;

    [Header("Audio Settings")]
    public AudioSource startCoolDownAudio; // เสียงที่ให้ดัง "ทุกครั้ง" ที่เริ่มลดอุณหภูมิ

    [Header("References")]
    public GameObject coolDownUIPanel; // ลากหน้าจอ UI ลดอุณหภูมิของเพื่อนมาใส่

    private bool isUIPreviouslyActive = false;

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
        if (coolDownUIPanel == null) return;

        // เช็คว่าหน้าจอลดอุณหภูมิถูกเปิดขึ้นมาหรือยัง
        bool isUIActive = coolDownUIPanel.activeSelf;

        // ถ้าจากเดิมปิดอยู่ แล้วจู่ๆ มันเปิดขึ้นมา (แปลว่าผู้เล่นเริ่มกดลดอุณหภูมิ)
        if (isUIActive && !isUIPreviouslyActive)
        {
            PlaySound();
        }

        isUIPreviouslyActive = isUIActive;
    }

    void PlaySound()
    {
        if (startCoolDownAudio != null)
        {
            // สั่งให้เริ่มเล่นเสียงใหม่ตั้งแต่ต้นทุกครั้งที่เปิด
            startCoolDownAudio.Stop();
            startCoolDownAudio.Play();
            Debug.Log("เล่นเสียงเริ่มลดอุณหภูมิแล้ว!");
        }
    }
}