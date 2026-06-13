using UnityEngine;

public class KeepUI : MonoBehaviour
{
    private static KeepUI instance;

    void Awake()
    {
        // ระบบ Singleton เพื่อป้องกันไม่ให้มี UI ซ้ำกันหลายอันเวลาเปลี่ยนฉากกลับมาที่เดิม
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
}