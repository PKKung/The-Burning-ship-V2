using UnityEngine;

public class HiddenSignalPoint : MonoBehaviour
{
    public SignalMiniGame miniGameManager; // ลาก Script Manager ที่เราทำไว้มาใส่
    public float interactRange = 2.5f;     // ระยะห่างที่อนุญาตให้กด
    public Transform player;                // ลากตัวผู้เล่นมาใส่

    private void OnMouseDown()
    {
        // เช็กระยะห่างระหว่างตัวละครกับจุดที่เราคลิก
        float distance = Vector2.Distance(player.position, transform.position);

        if (distance <= interactRange)
        {
            if (!miniGameManager.isGameActive)
            {
                miniGameManager.StartMiniGame();
                Debug.Log("เปิดมินิเกมจากจุดซ่อน!");
            }
        }
        else
        {
            Debug.Log("ตัวละครอยู่ไกลเกินไป!");
        }
    }
}