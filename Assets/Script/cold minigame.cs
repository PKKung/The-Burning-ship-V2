using UnityEngine;

public class coldminigame : MonoBehaviour
{
    public CoolingMiniGame miniGameManager;
    public float interactRange = 2.5f;
    public Transform player;

    private void OnMouseUp()
    {
        if (player == null || miniGameManager == null) return;

        float distance = Vector2.Distance(player.position, transform.position);

        if (distance <= interactRange)
        {
            // เช็กตัวแปรที่เราเพิ่งเพิ่มเข้าไป
            if (!miniGameManager.isGameActive)
            {
                // เปลี่ยนเป็น StartGame() ให้ตรงกับชื่อใน CoolingMiniGame
                miniGameManager.StartGame();
                Debug.Log("เปิดมินิเกมระบายความร้อน!");
            }
        }
        else
        {
            Debug.Log("ตัวละครอยู่ไกลเกินไป!");
        }
    }
}