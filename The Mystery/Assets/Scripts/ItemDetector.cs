using UnityEngine;
using TMPro;

public class ItemDetector : MonoBehaviour
{
    // อ้างอิงถึง PlayerCore ของผู้เล่น
    public PlayerCore playerCore;

    private Collider2D playerDetectionCollider; // Collider2D ที่ใช้ตรวจจับไอเท็ม

    private DialogueStarter currentInteractableThing; // Thing ที่สามารถเริ่มบทสนทนาได้

    //ปรับ offset ได้ใน Inspector
    [Header("Collider Offset Settings")]
    //when player is moving right, left, up, down
    // กำหนดค่าเริ่มต้นสำหรับ offset ของ Collider2D
    [SerializeField] private Vector2 rightOffset = new Vector2(1.2f, -0.5f);
    [SerializeField] private Vector2 leftOffset = new Vector2(-1.2f, -0.5f);
    [SerializeField] private Vector2 upOffset = new Vector2(0f, 1f);
    [SerializeField] private Vector2 downOffset = new Vector2(0f, -1.4f);

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
        // --- ส่วนนี้คือการจัดการ Thing และบทสนทนา ---
        if (currentInteractableThing != null)
        {
            // แสดง Prompt หรือข้อความ UI สำหรับ Thing
            currentInteractableThing.ShowPrompt(); // ฟังก์ชัน ShowPrompt ใน DialogueStarter

            // ถ้าผู้เล่นกด 'E' และมี Thing ที่กำลังมองอยู่
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentInteractableThing.StartDialogue(); // เริ่มบทสนทนา
                currentInteractableThing.HidePrompt(); // ซ่อน Prompt หลังจากเริ่มบทสนทนา
                currentInteractableThing = null; // ล้าง Thing ที่กำลังปฏิสัมพันธ์
            }
        }


        // --- ส่วนนี้คือการอัปเดตท offset ของ ItemDetector collider
        // อัปเดตตำแหน่งของ ItemDetector ให้ตรงกับทิศทางการเคลื่อนที่ของผู้เล่น
        if (playerCore != null && playerDetectionCollider != null)
        {
            //right x=1.2 y=-0.5
            Vector2 offset = Vector2.zero;
            if (playerCore.isMovingRight)
            {
                // ปรับตำแหน่งให้เหมาะสมกับการเคลื่อนที่ไปทางขวา
                playerDetectionCollider.offset = rightOffset; // ใช้ค่า offset ที่กำหนดไว้
            }
            else if (playerCore.isMovingLeft)
            {
                // ปรับตำแหน่งให้เหมาะสมกับการเคลื่อนที่ไปทางซ้าย
                playerDetectionCollider.offset = leftOffset; // ใช้ค่า offset ที่กำหนดไว้
            }
            else if (playerCore.isMovingUp)
            {
                // ปรับตำแหน่งให้เหมาะสมกับการเคลื่อนที่ขึ้น
                playerDetectionCollider.offset = upOffset; // ใช้ค่า offset ที่กำหนดไว้
            }
            else if (playerCore.isMovingDown)
            {
                // ปรับตำแหน่งให้เหมาะสมกับการเคลื่อนที่ลง
                playerDetectionCollider.offset = downOffset; // ใช้ค่า offset ที่กำหนดไว้
            }
        }
    }

    // --- ตรวจจับการเข้า-ออกของไอเท็มด้วย Trigger Collider2D ---

    void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่ามี Thing ที่สามารถเริ่มบทสนทนาได้หรือไม่
        DialogueStarter Thing = other.GetComponent<DialogueStarter>();
        if (Thing != null)
        {
            if (currentInteractableThing == null)
            {
                currentInteractableThing = Thing;
                // ไม่ต้อง ShowPrompt ตรงนี้ เพราะจะถูกเรียกใน Update()
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // ตรวจสอบว่าเป็น Thing ที่เรากำลังปฏิสัมพันธ์ด้วยหรือไม่
        DialogueStarter Thing = other.GetComponent<DialogueStarter>();
        if (Thing != null && Thing == currentInteractableThing)
        {
            Thing.HidePrompt(); // ซ่อน Prompt
            currentInteractableThing = null; // ล้าง Reference
        }
    }
}