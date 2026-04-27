using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public VideoPlayer myVideo; // ช่องสำหรับใส่ Video Player
    public string nextScene = "GameScene"; // ชื่อฉากที่จะไป (ต้องสะกดให้ตรงกับใน Unity)

    void Start()
    {
        // สั่งว่า "เฮ้ย Video Player ถ้าเล่นจบเมื่อไหร่ ให้ไปเรียกฟังก์ชัน CheckEnd นะ"
        myVideo.loopPointReached += CheckEnd;
    }

    void Update()
    {
        // ถ้ากด Spacebar ให้ข้ามไปเลยไม่ต้องรอจบ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    void CheckEnd(VideoPlayer vp)
    {
        // วิดีโอจบแล้ว... โหลดฉากเกมเลย!
        SceneManager.LoadScene(nextScene);
    }
}