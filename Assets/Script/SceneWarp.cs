using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneWarp : MonoBehaviour
{
    [Header("ตั้งค่าวาร์ป")]
    public string sceneToLoad;

    [Header("ตั้งค่า Fade")]
    public Image fadeImage;
    public float fadeSpeed = 2f;
    
    // ใช้ static เพื่อให้ค่านี้แชร์กันได้ หรือจะใช้ private ปกติก็ได้ครับ
    private bool isFading = false;

    private void Start()
    {
        // ทุกครั้งที่เริ่มฉากใหม่ ให้รีเซ็ตค่าและทำให้จอค่อยๆ สว่าง
        isFading = false;
        if (fadeImage != null)
        {
            // ตรวจสอบให้แน่ใจว่าเปิดใช้งาน Image
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeInRoutine());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isFading)
        {
            isFading = true;
            StartCoroutine(FadeOutAndLoad());
        }
    }

    // ฟังก์ชันทำให้จอค่อยๆ สว่าง (เมื่อเริ่มฉาก)
    private IEnumerator FadeInRoutine()
    {
        Color color = fadeImage.color;
        color.a = 1f; // เริ่มที่ดำสนิท
        fadeImage.color = color;

        while (color.a > 0f)
        {
            color.a -= Time.deltaTime * fadeSpeed;
            fadeImage.color = color;
            yield return null;
        }

        // พอสว่างแล้ว ปิดตัว Image ไว้เพื่อไม่ให้บังการคลิกเมาส์ (Raycast)
        fadeImage.gameObject.SetActive(false);
    }

    // ฟังก์ชันทำให้จอค่อยๆ มืด (ก่อนเปลี่ยนฉาก)
    private IEnumerator FadeOutAndLoad()
    {
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        color.a = 0f; // เริ่มที่ใสสะอาด

        while (color.a < 1f)
        {
            color.a += Time.deltaTime * fadeSpeed;
            fadeImage.color = color;
            yield return null;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}