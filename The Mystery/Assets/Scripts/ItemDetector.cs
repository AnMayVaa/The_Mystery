using UnityEngine;
using TMPro; // ถ้าใช้ TextMeshPro

public class ItemDetector : MonoBehaviour
{
    // อ้างอิงถึง PlayerCore ของผู้เล่น
    public PlayerCore playerCore;

    private CollectibleItem currentInteractableItem; // ไอเท็มที่กำลังสามารถปฏิสัมพันธ์ด้วยได้
    private Collider2D playerDetectionCollider; // Collider2D ที่ใช้ตรวจจับไอเท็ม
    

    void Start()
    {
        if (playerCore == null)
        {
            playerCore = GetComponentInParent<PlayerCore>(); // ลองหาจาก Parent หรือตัวเอง
            if (playerCore == null)
            {
                Debug.LogError("PlayerCore script not found for ItemDetector.");
            }
        }

        // หาก ItemDetector อยู่บน GameObject ลูกของ Player
        // ให้หา Collider2D ที่เป็น Trigger บน GameObject นี้
        playerDetectionCollider = GetComponent<Collider2D>();
        if (playerDetectionCollider == null)
        {
            Debug.LogError("ItemDetector requires a Collider2D component (Is Trigger set to true).");
        }
        else
        {
            playerDetectionCollider.isTrigger = true; // ตรวจสอบให้แน่ใจว่าเป็น Trigger
        }
    }

    void Update()
    {
        // --- ส่วนนี้คือการจัดการ UI Prompt และการเก็บไอเท็ม ---

        if (currentInteractableItem != null)
        {
            currentInteractableItem.ShowPrompt(); // แสดง Prompt ของไอเท็มที่อยู่ในระยะ

            // ถ้าผู้เล่นกด 'E' และมีไอเท็มที่กำลังมองอยู่
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentInteractableItem.Collect(); // เรียกฟังก์ชัน Collect ของไอเท็ม
                currentInteractableItem = null; // ล้างไอเท็มที่กำลังปฏิสัมพันธ์
            }
        }
        // ถ้า currentInteractableItem เป็น null หมายความว่าไม่มีไอเท็มในระยะ หรือเก็บไปแล้ว

        // --- ส่วนนี้คือการอัปเดตท offset ของ ItemDetector collider
        // อัปเดตตำแหน่งของ ItemDetector ให้ตรงกับทิศทางการเคลื่อนที่ของผู้เล่น
        if (playerCore != null && playerDetectionCollider != null)
        {
            //right x=1.2 y=-0.5
            Vector2 offset = Vector2.zero;
            if (playerCore.isMovingRight)
            {
                playerDetectionCollider.offset = new Vector2(1.2f, -0.5f); // ปรับตำแหน่งให้เหมาะสมกับการเคลื่อนที่ไปทางขวา
            }
            else if (playerCore.isMovingLeft)
            {
                playerDetectionCollider.offset = new Vector2(-1.2f, -0.5f); // ปรับตำแหน่งให้เหมาะสมกับการเคลื่อนที่ไปทางซ้าย
            }
            else if (playerCore.isMovingUp)
            {
                playerDetectionCollider.offset = new Vector2(0f, 1.4f); // ปรับตำแหน่งให้เหมาะสมกับการเคลื่อนที่ขึ้น
            }
            else if (playerCore.isMovingDown)
            {
                playerDetectionCollider.offset = new Vector2(0f, -1.4f); // ปรับตำแหน่งให้เหมาะสมกับการเคลื่อนที่ลง
            }
        }
    }

    // --- ตรวจจับการเข้า-ออกของไอเท็มด้วย Trigger Collider2D ---

    void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่าเป็นไอเท็มที่เก็บได้หรือไม่
        CollectibleItem item = other.GetComponent<CollectibleItem>();
        if (item != null)
        {
            // ถ้ามีไอเท็มอยู่แล้ว (หมายถึงผู้เล่นยังไม่ได้เก็บอันก่อนหน้า)
            // หรือนี่เป็นไอเท็มใหม่
            if (currentInteractableItem == null)
            {
                currentInteractableItem = item;
                // ไม่ต้อง ShowPrompt ตรงนี้ เพราะจะถูกเรียกใน Update()
            }
            // ถ้ามีหลายไอเท็มในพื้นที่เดียวกัน จะเลือกอันแรกที่เข้ามา
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        CollectibleItem item = other.GetComponent<CollectibleItem>();
        // ตรวจสอบว่าเป็นไอเท็มที่เรากำลังปฏิสัมพันธ์ด้วยหรือไม่
        if (item != null && item == currentInteractableItem)
        {
            item.HidePrompt(); // ซ่อน Prompt
            currentInteractableItem = null; // ล้าง Reference
        }
    }
}