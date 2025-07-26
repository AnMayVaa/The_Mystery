using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    // ตัวแปรสำหรับทิศทาง เพื่อใช้กับ Animator (และ ItemDetector)
    public bool isMovingRight { get; private set; }
    public bool isMovingLeft { get; private set; }
    public bool isMovingUp { get; private set; }
    public bool isMovingDown { get; private set; }

    // เก็บค่าทิศทางสุดท้ายที่ผู้เล่นหันไป (ใช้สำหรับ ItemDetector และ Idle Animation)
    public Vector2 lastMoveDirection { get; private set; } = Vector2.down; // เริ่มต้นหันลง

    // อ้างอิง Animator Component (ถ้ามี)
    private Animator animator;

    void Awake()
    {
        // พยายามดึง Animator Component เมื่อเกมเริ่ม
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameStateManager.Instance.playerDuringDialogue || !GameStateManager.Instance.freezePlayerDuringDialogue)
        {
            // เก็บค่า Input ในแต่ละแกน
            float moveX = 0f;
            float moveY = 0f;

            // Move the player based on input
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
                moveY = 1f; // กำหนดค่า Input สำหรับทิศทาง
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
                moveY = -1f; // กำหนดค่า Input สำหรับทิศทาง
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
                moveX = -1f; // กำหนดค่า Input สำหรับทิศทาง
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
                moveX = 1f; // กำหนดค่า Input สำหรับทิศทาง
            }

            // รวม Input ทั้งหมดเพื่อหาทิศทางการเคลื่อนที่รวม
            Vector2 movement = new Vector2(moveX, moveY).normalized;

            // อัปเดตตัวแปรทิศทาง Boolean และ lastMoveDirection
            UpdateDirectionBooleans(movement);

            // ส่งค่าไปยัง Animator (ถ้ามี Animator Component)
            UpdateAnimatorParameters();
        }

        if (GameStateManager.Instance.playerDuringDialogue && GameStateManager.Instance.freezePlayerDuringDialogue)
        {
            //nothing
        }
    }

    /// <summary>
    /// อัปเดตตัวแปร Boolean สำหรับทิศทางการเคลื่อนที่และทิศทางสุดท้ายที่หัน
    /// </summary>
    /// <param name="movement">Vector2 ที่แสดงทิศทางการเคลื่อนที่ปัจจุบัน</param>
    void UpdateDirectionBooleans(Vector2 movement)
    {
        // รีเซ็ตค่า Boolean ทั้งหมด
        isMovingRight = false;
        isMovingLeft = false;
        isMovingUp = false;
        isMovingDown = false;

        if (movement != Vector2.zero) // ถ้ามีการเคลื่อนที่
        {
            lastMoveDirection = movement; // อัปเดตทิศทางสุดท้ายที่หัน

            // กำหนด Boolean ตามทิศทางหลักที่เคลื่อนที่
            // ตรวจสอบแกนที่มีการเคลื่อนที่มากกว่า เพื่อกำหนดทิศทางหลัก
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                if (movement.x > 0) isMovingRight = true;
                else isMovingLeft = true;
            }
            else // ถ้าเคลื่อนที่ในแนวตั้งมากกว่า หรือเคลื่อนที่แนวเฉียงเท่าๆ กัน
            {
                if (movement.y > 0) isMovingUp = true;
                else isMovingDown = true;
            }
        }
        // ถ้า movement เป็น Vector2.zero (หยุดนิ่ง)
        // เราจะไม่เปลี่ยน lastMoveDirection เพื่อให้มันยังคงทิศทางสุดท้ายที่ผู้เล่นหันไป
        // ส่วน isMovingX/Y จะเป็น false อยู่แล้วจากการ Reset
    }

    /// <summary>
    /// ส่งค่าตัวแปรทิศทางไปยัง Animator
    /// คุณต้องสร้าง Animator Parameters ที่ชื่อตรงกันใน Animator Controller
    /// </summary>
    void UpdateAnimatorParameters()
    {
        if (animator != null)
        {
            // พารามิเตอร์สำหรับบอกว่ากำลังอยู่กับที่หรือไม่
            animator.SetBool("isMoving", isMovingRight || isMovingLeft || isMovingUp || isMovingDown);

            // พารามิเตอร์สำหรับทิศทางการเคลื่อนที่ (ใช้สำหรับแอนิเมชันเดิน)
            animator.SetBool("isMovingRight", isMovingRight);
            animator.SetBool("isMovingLeft", isMovingLeft);
            animator.SetBool("isMovingUp", isMovingUp);
            animator.SetBool("isMovingDown", isMovingDown);
            // พารามิเตอร์สำหรับทิศทางสุดท้ายที่หัน
            animator.SetFloat("LastMoveX", lastMoveDirection.x);
            animator.SetFloat("LastMoveY", lastMoveDirection.y);
        }
    }
}