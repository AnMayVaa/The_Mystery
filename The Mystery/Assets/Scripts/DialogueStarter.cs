using UnityEngine;
using DialogueEditor;

public class DialogueStarter : MonoBehaviour
{

    [SerializeField] private NPCConversation myConversation; // บทสนทนาที่จะเริ่มเมื่อ Interaction zone โดน และกด E จาก ItemDetector

    public GameObject uiPrompt; // UI Prompt ที่จะแสดงเมื่อผู้เล่นอยู่ใกล้ NPC

    [Header("Player Control")]
    [SerializeField]
    [Tooltip("If true, player movement will be frozen during this dialogue")]
    public bool freezePlayerDuringDialogue = true; // Default to true for backward compatibility

    [Header("Cutscene")]
    [SerializeField]
    [Tooltip("If true, this dialogue will be treated as a cutscene and player control will be disabled")]
    public bool isCutscene = false; // Default to false for backward compatibility
    public bool playwhenscenestart = false; // If true, this dialogue will play when the scene starts

    private void Start()
    {
        print("DialogueStarter Start called for " + gameObject.name);
        print("DialogueStarter isCutscene: " + isCutscene);
        print("Checking... " + !GameStateManager.Instance.HasCollectedItem(gameObject.name));
        // ถ้า playwhenscenestart เป็น true และ GameStateManagerพบว่ายังไม่ได้เก็บ dialogue นี้ไป
        if (playwhenscenestart && !GameStateManager.Instance.HasCollectedItem(gameObject.name))
        {
            // เริ่มบทสนทนาเมื่อเริ่มฉาก
            StartDialogue();
        }
    }
    
    // ฟังก์ชันสำหรับเริ่มบทสนทนา    
    public void StartDialogue()
    {
        if (myConversation != null)
        {
            // เรียกใช้ DialogueManager เพื่อเริ่มบทสนทนา
            ConversationManager.Instance.StartConversation(myConversation);
            Debug.Log("Dialogue started with " + gameObject.name);

            GameStateManager.Instance.playerDuringDialogue = true;
            GameStateManager.Instance.freezePlayerDuringDialogue = freezePlayerDuringDialogue;
        }
        else
        {
            Debug.LogWarning("Dialogue is not assigned in DialogueStarter on " + gameObject.name);
        }
    }

    public void ShowPrompt()
    {
        if (uiPrompt != null && !uiPrompt.activeSelf)
        {
            uiPrompt.SetActive(true);
        }
    }

    // ฟังก์ชันสำหรับซ่อนข้อความ UI
    public void HidePrompt()
    {
        if (uiPrompt != null && uiPrompt.activeSelf)
        {
            uiPrompt.SetActive(false);
        }
    }
}
