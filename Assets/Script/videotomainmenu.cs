using UnityEngine;
using UnityEngine.Video; // จำเป็นต้องใช้เพื่อคุม VideoPlayer
using UnityEngine.SceneManagement; // ใช้เพื่อเปลี่ยนฉาก

public class VideoToMainMenu : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void OnEnable()
    {
        // ลงทะเบียนเหตุการณ์: เมื่อวิดีโอเล่นจบ ให้เรียกฟังก์ชัน CheckOver
        videoPlayer.loopPointReached += CheckOver;
    }

    void OnDisable()
    {
        // ยกเลิกการลงทะเบียนเมื่อ Object ถูกปิด
        videoPlayer.loopPointReached -= CheckOver;
    }

    void CheckOver(VideoPlayer vp)
    {
        Debug.Log("วิดีโอจบแล้ว! กำลังกลับไปหน้า Main Menu...");

        // เปลี่ยนชื่อ "MainMenu" ให้ตรงกับชื่อฉากเมนูของคุณ
        SceneManager.LoadScene("u");
    }

    // (แถม) เผื่อผู้เล่นอยากกด Skip วิดีโอ
 
}