using UnityEngine;

public class SignalStartVoiceManager : MonoBehaviour
{
    public static SignalStartVoiceManager instance;

    [Header("Audio Settings")]
    public AudioSource voiceRound1; // เสียงตอนเริ่มกดส่งสัญญาณรอบ 1
    public AudioSource voiceRound2; // เสียงตอนเริ่มกดส่งสัญญาณรอบ 2
    public AudioSource voiceRound3; // เสียงตอนเริ่มกดส่งสัญญาณรอบ 3

    [Header("References")]
    public GameObject signalUIPanel; // ลากหน้าจอ UI Mini-game ของเพื่อนมาใส่

    private bool isUIPreviouslyActive = false;
    private bool playedR1 = false;
    private bool playedR2 = false;
    private bool playedR3 = false;

    void Awake()
    {
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
        if (signalUIPanel == null) return;

        // เช็คว่าหน้าจอ UI เพิ่งจะถูกเปิดขึ้นมาใช่หรือไม่
        bool isUIActive = signalUIPanel.activeSelf;

        if (isUIActive && !isUIPreviouslyActive)
        {
            // หน้าจอเพิ่งเปิด! เช็ครอบแล้วเล่นเสียง
            PlayStartVoice();
        }

        isUIPreviouslyActive = isUIActive;
    }

    void PlayStartVoice()
    {
        int currentRound = SignalQuestManager.instance.currentRound;

        if (currentRound == 1 && !playedR1)
        {
            PlayAudio(voiceRound1);
            playedR1 = true;
        }
        else if (currentRound == 2 && !playedR2)
        {
            PlayAudio(voiceRound2);
            playedR2 = true;
        }
        else if (currentRound == 3 && !playedR3)
        {
            PlayAudio(voiceRound3);
            playedR3 = true;
        }
    }

    void PlayAudio(AudioSource audio)
    {
        if (audio != null)
        {
            audio.Play();
            Debug.Log("เล่นเสียงเริ่มส่งสัญญาณ!");
        }
    }

    // ฟังก์ชันรีเซ็ตสำหรับปุ่ม Start Game
    public void ResetSignalVoices()
    {
        playedR1 = false;
        playedR2 = false;
        playedR3 = false;
    }
}